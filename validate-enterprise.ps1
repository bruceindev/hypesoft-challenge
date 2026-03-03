$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# ========================
# ConfiguraÃ§Ã£o base
# ========================
$BaseUrl = 'http://localhost:5000'
$KeycloakRealmUrl = 'http://localhost:8080/realms/hypesoft/protocol/openid-connect/token'
$KeycloakClientId = 'hypesoft-frontend'
$PerformanceLimitMs = 500

$AllResults = New-Object System.Collections.Generic.List[object]
$CriticalFailures = New-Object System.Collections.Generic.List[object]
$CreatedResourceIds = [ordered]@{
    CategoryId = $null
    Products   = New-Object System.Collections.Generic.List[string]
}

$SupportsResponseHeadersVariable = (Get-Command Invoke-RestMethod).Parameters.ContainsKey('ResponseHeadersVariable')

# ========================
# Helpers
# ========================
function New-TestResult {
    param(
        [string]$Name,
        [string]$Group,
        [string]$Category,
        [int[]]$ExpectedStatus,
        [int]$Status,
        [double]$DurationMs,
        [bool]$Passed,
        [bool]$Critical,
        [string]$Details
    )

    [pscustomobject]@{
        Name           = $Name
        Group          = $Group
        Category       = $Category
        ExpectedStatus = ($ExpectedStatus -join ',')
        Status         = $Status
        DurationMs     = [Math]::Round($DurationMs, 2)
        Passed         = $Passed
        Critical       = $Critical
        Details        = $Details
    }
}

function Get-HttpErrorInfo {
    param([System.Management.Automation.ErrorRecord]$ErrorRecord)

    $statusCode = -1
    $message = $ErrorRecord.Exception.Message

    $hasResponseProperty = $null -ne $ErrorRecord.Exception.PSObject.Properties['Response']
    if ($hasResponseProperty -and $null -ne $ErrorRecord.Exception.Response) {
        try { $statusCode = [int]$ErrorRecord.Exception.Response.StatusCode } catch {}

        try {
            $stream = $ErrorRecord.Exception.Response.GetResponseStream()
            if ($null -ne $stream) {
                $reader = New-Object System.IO.StreamReader($stream)
                $rawBody = $reader.ReadToEnd()
                if (-not [string]::IsNullOrWhiteSpace($rawBody)) {
                    $message = $rawBody
                }
            }
        } catch {}
    }

    return [pscustomobject]@{
        StatusCode = $statusCode
        Message = $message
    }
}

function Invoke-ApiTest {
    param(
        [string]$Name,
        [string]$Group,
        [string]$Category,
        [string]$Uri,
        [string]$Method = 'GET',
        [hashtable]$Headers = @{},
        [object]$Body = $null,
        [string]$ContentType = 'application/json',
        [int[]]$ExpectedStatus = @(200),
        [bool]$Critical = $true
    )

    $response = $null
    $responseHeaders = @{}
    $status = -1
    $details = 'ok'

    $elapsed = Measure-Command {
        try {
            $invokeParams = @{
                Uri     = $Uri
                Method  = $Method
                Headers = $Headers
            }

            if ($null -ne $Body) {
                if ($Body -is [string]) {
                    $invokeParams.Body = $Body
                }
                else {
                    $invokeParams.Body = ($Body | ConvertTo-Json -Depth 10)
                }

                $invokeParams.ContentType = $ContentType
            }

            if ($SupportsResponseHeadersVariable) {
                $rh = $null
                $response = Invoke-RestMethod @invokeParams -ResponseHeadersVariable rh
                if ($null -ne $rh) {
                    $responseHeaders = $rh
                }
            }
            else {
                $response = Invoke-RestMethod @invokeParams
            }

            $status = $ExpectedStatus[0]
        }
        catch {
            $errorInfo = Get-HttpErrorInfo -ErrorRecord $_
            $status = $errorInfo.StatusCode
            $details = $errorInfo.Message
        }
    }

    $passed = $ExpectedStatus -contains $status
    $result = New-TestResult -Name $Name -Group $Group -Category $Category -ExpectedStatus $ExpectedStatus -Status $status -DurationMs $elapsed.TotalMilliseconds -Passed $passed -Critical $Critical -Details $details
    $AllResults.Add($result)

    if ($Critical -and -not $passed) {
        $CriticalFailures.Add($result)
    }

    return [pscustomobject]@{
        Result = $result
        Data = $response
        Headers = $responseHeaders
    }
}

function Get-KeycloakToken {
    param(
        [string]$Username,
        [string]$Password
    )

    $body = "client_id=$KeycloakClientId&grant_type=password&username=$Username&password=$Password"
    $tokenResult = Invoke-ApiTest `
        -Name "Keycloak login ($Username)" `
        -Group 'Auth' `
        -Category 'Auth' `
        -Uri $KeycloakRealmUrl `
        -Method 'POST' `
        -Body $body `
        -ContentType 'application/x-www-form-urlencoded' `
        -ExpectedStatus @(200) `
        -Critical $true

    $token = $null
    if ($tokenResult.Result.Passed -and $null -ne $tokenResult.Data -and -not [string]::IsNullOrWhiteSpace($tokenResult.Data.access_token)) {
        $token = $tokenResult.Data.access_token
    }
    else {
        $CriticalFailures.Add((New-TestResult -Name "Token presence ($Username)" -Group 'Auth' -Category 'Auth' -ExpectedStatus @(200) -Status -1 -DurationMs 0 -Passed $false -Critical $true -Details 'Token was not returned by Keycloak'))
    }

    return $token
}

function Add-CreatedProduct {
    param([object]$ResponseData)
    if ($null -ne $ResponseData -and -not [string]::IsNullOrWhiteSpace($ResponseData.id)) {
        $CreatedResourceIds.Products.Add($ResponseData.id)
    }
}

function Cleanup-TestData {
    param([string]$AdminToken)

    if ([string]::IsNullOrWhiteSpace($AdminToken)) {
        return
    }

    $adminHeaders = @{ Authorization = "Bearer $AdminToken" }

    foreach ($productId in @($CreatedResourceIds.Products)) {
        try {
            Invoke-RestMethod -Uri "$BaseUrl/api/products/$productId" -Method Delete -Headers $adminHeaders | Out-Null
        } catch {}
    }

    if (-not [string]::IsNullOrWhiteSpace($CreatedResourceIds.CategoryId)) {
        try {
            Invoke-RestMethod -Uri "$BaseUrl/api/categories/$($CreatedResourceIds.CategoryId)" -Method Delete -Headers $adminHeaders | Out-Null
        } catch {}
    }
}

# ========================
# ExecuÃ§Ã£o da suÃ­te
# ========================

# 1) Health check
$health = Invoke-ApiTest -Name 'Health check' -Group 'Validation' -Category 'Validation' -Uri "$BaseUrl/health" -Method 'GET' -ExpectedStatus @(200) -Critical $true

# 2) Endpoint anÃ´nimo (espera 401)
$anon = Invoke-ApiTest -Name 'Anonymous GET /api/products' -Group 'Auth' -Category 'Auth' -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=1" -Method 'GET' -ExpectedStatus @(401) -Critical $true

# 3) Login no Keycloak (admin, manager, user)
$adminToken = Get-KeycloakToken -Username 'admin' -Password 'admin123'
$managerToken = Get-KeycloakToken -Username 'manager' -Password 'manager123'
$userToken = Get-KeycloakToken -Username 'user' -Password 'user123'

$adminHeaders = @{ Authorization = "Bearer $adminToken" }
$managerHeaders = @{ Authorization = "Bearer $managerToken" }
$userHeaders = @{ Authorization = "Bearer $userToken" }

# Cria categoria de suporte para os testes de produtos
$suffix = [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
$categoryCreate = Invoke-ApiTest `
    -Name 'Create category (setup)' `
    -Group 'Validation' `
    -Category 'Validation' `
    -Uri "$BaseUrl/api/categories" `
    -Method 'POST' `
    -Headers $adminHeaders `
    -Body @{ name = "CAT-ENTERPRISE-$suffix"; description = 'Validation category' } `
    -ExpectedStatus @(201) `
    -Critical $true

if ($categoryCreate.Result.Passed -and $null -ne $categoryCreate.Data -and -not [string]::IsNullOrWhiteSpace($categoryCreate.Data.id)) {
    $CreatedResourceIds.CategoryId = $categoryCreate.Data.id
}

# 4) Testes autenticados por role
$adminGet = Invoke-ApiTest -Name 'Admin GET /api/products' -Group 'Auth' -Category 'Auth' -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=5" -Method 'GET' -Headers $adminHeaders -ExpectedStatus @(200) -Critical $true

$adminPost = Invoke-ApiTest `
    -Name 'Admin POST /api/products' `
    -Group 'Auth' `
    -Category 'Auth' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $adminHeaders `
    -Body @{ name = "PROD-ADMIN-$suffix"; description = 'admin create'; price = 12; stockQuantity = 4; categoryId = $CreatedResourceIds.CategoryId } `
    -ExpectedStatus @(201) `
    -Critical $true
Add-CreatedProduct -ResponseData $adminPost.Data

$adminDelete = $null
if ($adminPost.Result.Passed -and $null -ne $adminPost.Data -and -not [string]::IsNullOrWhiteSpace($adminPost.Data.id)) {
    $adminDelete = Invoke-ApiTest `
        -Name 'Admin DELETE /api/products/{id}' `
        -Group 'Auth' `
        -Category 'Auth' `
        -Uri "$BaseUrl/api/products/$($adminPost.Data.id)" `
        -Method 'DELETE' `
        -Headers $adminHeaders `
        -ExpectedStatus @(204) `
        -Critical $true
}

$managerGet = Invoke-ApiTest -Name 'Manager GET /api/products' -Group 'Auth' -Category 'Auth' -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=5" -Method 'GET' -Headers $managerHeaders -ExpectedStatus @(200) -Critical $true

$managerPost = Invoke-ApiTest `
    -Name 'Manager POST /api/products' `
    -Group 'Auth' `
    -Category 'Auth' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $managerHeaders `
    -Body @{ name = "PROD-MANAGER-$suffix"; description = 'manager create'; price = 14; stockQuantity = 3; categoryId = $CreatedResourceIds.CategoryId } `
    -ExpectedStatus @(201) `
    -Critical $true
Add-CreatedProduct -ResponseData $managerPost.Data

$userGet = Invoke-ApiTest -Name 'User GET /api/products' -Group 'Auth' -Category 'Auth' -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=5" -Method 'GET' -Headers $userHeaders -ExpectedStatus @(200) -Critical $true

# Garante que user realmente nÃ£o consegue POST (apenas GET)
$userPostForbidden = Invoke-ApiTest `
    -Name 'User POST /api/products (forbidden)' `
    -Group 'Auth' `
    -Category 'Auth' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $userHeaders `
    -Body @{ name = "PROD-USER-$suffix"; description = 'user should not create'; price = 9; stockQuantity = 1; categoryId = $CreatedResourceIds.CategoryId } `
    -ExpectedStatus @(403) `
    -Critical $true

# 5) Teste de validaÃ§Ã£o (POST invÃ¡lido -> 400)
$validationTest = Invoke-ApiTest `
    -Name 'Validation POST invalid product -> 400' `
    -Group 'Validation' `
    -Category 'Validation' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $managerHeaders `
    -Body @{ name = ''; description = 'x'; price = -1; stockQuantity = -5; categoryId = '' } `
    -ExpectedStatus @(400) `
    -Critical $true

# 6) Teste de low-stock
$lowProduct = Invoke-ApiTest `
    -Name 'Create low-stock product' `
    -Group 'Business Rules' `
    -Category 'Business Rules' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $managerHeaders `
    -Body @{ name = "PROD-LOW-$suffix"; description = 'low'; price = 8; stockQuantity = 2; categoryId = $CreatedResourceIds.CategoryId } `
    -ExpectedStatus @(201) `
    -Critical $true
Add-CreatedProduct -ResponseData $lowProduct.Data

$highProduct = Invoke-ApiTest `
    -Name 'Create high-stock product' `
    -Group 'Business Rules' `
    -Category 'Business Rules' `
    -Uri "$BaseUrl/api/products" `
    -Method 'POST' `
    -Headers $managerHeaders `
    -Body @{ name = "PROD-HIGH-$suffix"; description = 'high'; price = 18; stockQuantity = 25; categoryId = $CreatedResourceIds.CategoryId } `
    -ExpectedStatus @(201) `
    -Critical $true
Add-CreatedProduct -ResponseData $highProduct.Data

$lowStockList = Invoke-ApiTest `
    -Name 'GET /api/products/low-stock?threshold=10' `
    -Group 'Business Rules' `
    -Category 'Business Rules' `
    -Uri "$BaseUrl/api/products/low-stock?threshold=10" `
    -Method 'GET' `
    -Headers $userHeaders `
    -ExpectedStatus @(200) `
    -Critical $true

$lowStockContainsLow = $false
$lowStockContainsHigh = $false

if ($lowStockList.Result.Passed -and $null -ne $lowStockList.Data) {
    $lowIds = @($lowStockList.Data | ForEach-Object { $_.id })
    if ($null -ne $lowProduct.Data) { $lowStockContainsLow = $lowIds -contains $lowProduct.Data.id }
    if ($null -ne $highProduct.Data) { $lowStockContainsHigh = $lowIds -contains $highProduct.Data.id }
}

if (-not $lowStockContainsLow -or $lowStockContainsHigh) {
    $criticalBusiness = New-TestResult -Name 'Low-stock business assertion' -Group 'Business Rules' -Category 'Business Rules' -ExpectedStatus @(200) -Status -1 -DurationMs 0 -Passed $false -Critical $true -Details "ContainsLow=$lowStockContainsLow; ContainsHigh=$lowStockContainsHigh"
    $AllResults.Add($criticalBusiness)
    $CriticalFailures.Add($criticalBusiness)
}
else {
    $AllResults.Add((New-TestResult -Name 'Low-stock business assertion' -Group 'Business Rules' -Category 'Business Rules' -ExpectedStatus @(200) -Status 200 -DurationMs 0 -Passed $true -Critical $true -Details 'Low-stock rule respected (<10 only)'))
}

# 7) Teste de dashboard
$dashboard = Invoke-ApiTest `
    -Name 'GET /api/dashboard/stats' `
    -Group 'Business Rules' `
    -Category 'Business Rules' `
    -Uri "$BaseUrl/api/dashboard/stats" `
    -Method 'GET' `
    -Headers $userHeaders `
    -ExpectedStatus @(200) `
    -Critical $true

if ($dashboard.Result.Passed -and $null -ne $dashboard.Data -and $lowStockList.Result.Passed) {
    $dashboardLow = [int]$dashboard.Data.lowStockCount
    $endpointLow = @($lowStockList.Data).Count
    $isConsistent = $dashboardLow -eq $endpointLow

    if (-not $isConsistent) {
        $criticalDashboard = New-TestResult -Name 'Dashboard consistency assertion' -Group 'Business Rules' -Category 'Business Rules' -ExpectedStatus @(200) -Status -1 -DurationMs 0 -Passed $false -Critical $true -Details "dashboard.lowStockCount=$dashboardLow endpoint.lowStock=$endpointLow"
        $AllResults.Add($criticalDashboard)
        $CriticalFailures.Add($criticalDashboard)
    }
    else {
        $AllResults.Add((New-TestResult -Name 'Dashboard consistency assertion' -Group 'Business Rules' -Category 'Business Rules' -ExpectedStatus @(200) -Status 200 -DurationMs 0 -Passed $true -Critical $true -Details 'Dashboard lowStockCount matches endpoint'))
    }
}

# 8) Teste de paginaÃ§Ã£o
$pagination = Invoke-ApiTest `
    -Name 'GET /api/products?pageNumber=1&pageSize=1' `
    -Group 'Validation' `
    -Category 'Validation' `
    -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=1" `
    -Method 'GET' `
    -Headers $userHeaders `
    -ExpectedStatus @(200) `
    -Critical $true

if ($pagination.Result.Passed -and $null -ne $pagination.Data) {
    $pageNumberOk = ([int]$pagination.Data.pageNumber) -eq 1
    $pageSizeOk = ([int]$pagination.Data.pageSize) -eq 1
    $itemsCountOk = (@($pagination.Data.items).Count -le 1)

    if (-not ($pageNumberOk -and $pageSizeOk -and $itemsCountOk)) {
        $criticalPagination = New-TestResult -Name 'Pagination assertion' -Group 'Validation' -Category 'Validation' -ExpectedStatus @(200) -Status -1 -DurationMs 0 -Passed $false -Critical $true -Details "pageNumberOk=$pageNumberOk; pageSizeOk=$pageSizeOk; itemsCountOk=$itemsCountOk"
        $AllResults.Add($criticalPagination)
        $CriticalFailures.Add($criticalPagination)
    }
    else {
        $AllResults.Add((New-TestResult -Name 'Pagination assertion' -Group 'Validation' -Category 'Validation' -ExpectedStatus @(200) -Status 200 -DurationMs 0 -Passed $true -Critical $true -Details 'Pagination metadata and page size are correct'))
    }
}

# 9) Teste de performance (<500ms)
$maxCriticalDuration = ($AllResults | Where-Object { $_.Critical -and $_.DurationMs -gt 0 } | Measure-Object -Property DurationMs -Maximum).Maximum
if ($null -eq $maxCriticalDuration) { $maxCriticalDuration = 0 }
$performancePass = $maxCriticalDuration -lt $PerformanceLimitMs

if (-not $performancePass) {
    $criticalPerformance = New-TestResult -Name 'Performance assertion' -Group 'Performance' -Category 'Performance' -ExpectedStatus @(200) -Status -1 -DurationMs $maxCriticalDuration -Passed $false -Critical $true -Details "Max critical latency ${maxCriticalDuration}ms >= ${PerformanceLimitMs}ms"
    $AllResults.Add($criticalPerformance)
    $CriticalFailures.Add($criticalPerformance)
}
else {
    $AllResults.Add((New-TestResult -Name 'Performance assertion' -Group 'Performance' -Category 'Performance' -ExpectedStatus @(200) -Status 200 -DurationMs $maxCriticalDuration -Passed $true -Critical $true -Details "Max critical latency ${maxCriticalDuration}ms < ${PerformanceLimitMs}ms"))
}

# 10) Verificar header X-Correlation-ID
$correlationId = [Guid]::NewGuid().ToString()
$headersWithCorrelation = @{ Authorization = "Bearer $userToken"; 'X-Correlation-ID' = $correlationId }
$correlationTest = Invoke-ApiTest `
    -Name 'Security header X-Correlation-ID' `
    -Group 'Security' `
    -Category 'Security' `
    -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=1" `
    -Method 'GET' `
    -Headers $headersWithCorrelation `
    -ExpectedStatus @(200) `
    -Critical $true

$correlationHeaderPass = $false
$correlationDetails = 'X-Correlation-ID returned correctly'
if ($correlationTest.Result.Passed -and $SupportsResponseHeadersVariable) {
    $returnedCorrelation = $correlationTest.Headers['X-Correlation-ID']
    if (-not [string]::IsNullOrWhiteSpace($returnedCorrelation)) {
        $correlationHeaderPass = $true
    }
}

if (-not $SupportsResponseHeadersVariable) {
    try {
        $webResponse = Invoke-WebRequest -Uri "$BaseUrl/api/products?pageNumber=1&pageSize=1" -Method Get -Headers $headersWithCorrelation -UseBasicParsing
        $returnedCorrelation = $webResponse.Headers['X-Correlation-ID']
        $correlationHeaderPass = -not [string]::IsNullOrWhiteSpace($returnedCorrelation)
        if (-not $correlationHeaderPass) {
            $correlationDetails = 'X-Correlation-ID was not returned in response headers (fallback via Invoke-WebRequest)'
        }
    }
    catch {
        $correlationHeaderPass = $false
        $correlationDetails = "Unable to validate correlation header via fallback: $($_.Exception.Message)"
    }
}

if (-not $correlationHeaderPass) {
    $securityResult = New-TestResult -Name 'Correlation header assertion' -Group 'Security' -Category 'Security' -ExpectedStatus @(200) -Status -1 -DurationMs 0 -Passed $false -Critical $true -Details 'X-Correlation-ID was not returned in response'
    if (-not [string]::IsNullOrWhiteSpace($correlationDetails)) {
        $securityResult.Details = $correlationDetails
    }
    $AllResults.Add($securityResult)
    $CriticalFailures.Add($securityResult)
}
else {
    $AllResults.Add((New-TestResult -Name 'Correlation header assertion' -Group 'Security' -Category 'Security' -ExpectedStatus @(200) -Status 200 -DurationMs 0 -Passed $true -Critical $true -Details $correlationDetails))
}

# Limpeza best-effort
Cleanup-TestData -AdminToken $adminToken

# ========================
# ConsolidaÃ§Ã£o do relatÃ³rio
# ========================
$validationPass = ($AllResults | Where-Object { $_.Group -eq 'Validation' -and $_.Critical } | Where-Object { -not $_.Passed } | Measure-Object).Count -eq 0
$authPass = ($AllResults | Where-Object { $_.Group -eq 'Auth' -and $_.Critical } | Where-Object { -not $_.Passed } | Measure-Object).Count -eq 0
$businessPass = ($AllResults | Where-Object { $_.Group -eq 'Business Rules' -and $_.Critical } | Where-Object { -not $_.Passed } | Measure-Object).Count -eq 0
$performanceGroupPass = ($AllResults | Where-Object { $_.Group -eq 'Performance' -and $_.Critical } | Where-Object { -not $_.Passed } | Measure-Object).Count -eq 0
$securityPass = ($AllResults | Where-Object { $_.Group -eq 'Security' -and $_.Critical } | Where-Object { -not $_.Passed } | Measure-Object).Count -eq 0

$overallGo = $validationPass -and $authPass -and $businessPass -and $performanceGroupPass -and $securityPass -and ($CriticalFailures.Count -eq 0)

Write-Host ''
Write-Host '========================'
Write-Host 'HYPESOFT ENTERPRISE VALIDATION'
Write-Host ('Validation: ' + $(if ($validationPass) { 'PASS' } else { 'FAIL' }))
Write-Host ('Auth: ' + $(if ($authPass) { 'PASS' } else { 'FAIL' }))
Write-Host ('Business Rules: ' + $(if ($businessPass) { 'PASS' } else { 'FAIL' }))
Write-Host ('Performance: ' + $(if ($performanceGroupPass) { 'PASS' } else { 'FAIL' }))
Write-Host ('Security: ' + $(if ($securityPass) { 'PASS' } else { 'FAIL' }))
Write-Host ('Overall: ' + $(if ($overallGo) { 'GO' } else { 'NO-GO' }))
Write-Host '========================'
Write-Host ''

Write-Host 'Detailed results:'
$AllResults | Sort-Object Group, Name | Format-Table Name, Group, Status, ExpectedStatus, DurationMs, Passed, Critical -AutoSize

if (-not $overallGo) {
    Write-Host ''
    Write-Host 'Critical failures:'
    $CriticalFailures | Format-Table Name, Group, Status, ExpectedStatus, Details -AutoSize
    exit 1
}

exit 0
