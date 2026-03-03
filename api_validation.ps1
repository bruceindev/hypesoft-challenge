$ErrorActionPreference='Stop'
$base='http://localhost:5000'
$results=@()

function Add-Result($name,$status,$ms,$ok,$details){
  $script:results += [pscustomobject]@{name=$name;status=$status;ms=[math]::Round($ms,2);ok=$ok;details=$details}
}

function Invoke-TimedRequest {
  param([string]$Name,[scriptblock]$Request)
  $sw=[System.Diagnostics.Stopwatch]::StartNew()
  try {
    $resp=& $Request
    $sw.Stop()
    Add-Result $Name $resp.StatusCode $sw.Elapsed.TotalMilliseconds $true "ok"
    return $resp
  } catch {
    $sw.Stop()
    $status = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { -1 }
    Add-Result $Name $status $sw.Elapsed.TotalMilliseconds $false $_.Exception.Message
    return $null
  }
}

$correlation=[guid]::NewGuid().ToString()
$headers=@{ 'X-Correlation-ID'=$correlation }

$beforeStats = Invoke-RestMethod -Uri "$base/api/dashboard/stats" -Headers $headers -Method Get
$beforeProducts = [int]$beforeStats.totalProducts
$beforeStock = [decimal]$beforeStats.totalStockValue
$beforeLow = [int]$beforeStats.lowStockCount

$health = Invoke-TimedRequest -Name 'GET /health' -Request { Invoke-WebRequest -Uri "$base/health" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$productsGet = Invoke-TimedRequest -Name 'GET /api/products' -Request { Invoke-WebRequest -Uri "$base/api/products?pageNumber=1&pageSize=10" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$categoriesGet = Invoke-TimedRequest -Name 'GET /api/categories' -Request { Invoke-WebRequest -Uri "$base/api/categories" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$dashboardGet = Invoke-TimedRequest -Name 'GET /api/dashboard/stats' -Request { Invoke-WebRequest -Uri "$base/api/dashboard/stats" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }

$null = Invoke-TimedRequest -Name 'POST /api/categories (invalid)' -Request {
  Invoke-WebRequest -Uri "$base/api/categories" -Headers $headers -UseBasicParsing -Method Post -TimeoutSec 30 -ContentType 'application/json' -Body (@{ name=''; description='' } | ConvertTo-Json)
}

$catName="CAT-E2E-$(Get-Date -Format 'yyyyMMddHHmmssfff')"
$catBody=@{ name=$catName; description='Categoria de validacao final' } | ConvertTo-Json
$catResp = Invoke-TimedRequest -Name 'POST /api/categories' -Request { Invoke-WebRequest -Uri "$base/api/categories" -Headers $headers -UseBasicParsing -Method Post -TimeoutSec 30 -ContentType 'application/json' -Body $catBody }
$catObj = if($catResp){ $catResp.Content | ConvertFrom-Json } else { $null }
$catId = if($catObj){ $catObj.id } else { $null }

$null = Invoke-TimedRequest -Name 'POST /api/products (invalid)' -Request {
  Invoke-WebRequest -Uri "$base/api/products" -Headers $headers -UseBasicParsing -Method Post -TimeoutSec 30 -ContentType 'application/json' -Body (@{ name=''; description='x'; price=-1; categoryId=''; stockQuantity=-5 } | ConvertTo-Json)
}

$p1Name="PROD-LOW-$(Get-Date -Format 'yyyyMMddHHmmssfff')"
$p1Body=@{ name=$p1Name; description='produto low stock'; price=10; categoryId=$catId; stockQuantity=5 } | ConvertTo-Json
$p1Resp = Invoke-TimedRequest -Name 'POST /api/products (low stock)' -Request { Invoke-WebRequest -Uri "$base/api/products" -Headers $headers -UseBasicParsing -Method Post -TimeoutSec 30 -ContentType 'application/json' -Body $p1Body }
$p1Obj = if($p1Resp){ $p1Resp.Content | ConvertFrom-Json } else { $null }
$p1Id = if($p1Obj){ $p1Obj.id } else { $null }

$p2Name="PROD-HIGH-$(Get-Date -Format 'yyyyMMddHHmmssfff')"
$p2Body=@{ name=$p2Name; description='produto high stock'; price=2; categoryId=$catId; stockQuantity=20 } | ConvertTo-Json
$p2Resp = Invoke-TimedRequest -Name 'POST /api/products (high stock)' -Request { Invoke-WebRequest -Uri "$base/api/products" -Headers $headers -UseBasicParsing -Method Post -TimeoutSec 30 -ContentType 'application/json' -Body $p2Body }
$p2Obj = if($p2Resp){ $p2Resp.Content | ConvertFrom-Json } else { $null }
$p2Id = if($p2Obj){ $p2Obj.id } else { $null }

if($p1Id){
  $updBody=@{ name="$p1Name-UPD"; description='produto low stock atualizado'; price=12; categoryId=$catId; imageUrl=$null } | ConvertTo-Json
  $null = Invoke-TimedRequest -Name 'PUT /api/products/{id}' -Request { Invoke-WebRequest -Uri "$base/api/products/$p1Id" -Headers $headers -UseBasicParsing -Method Put -TimeoutSec 30 -ContentType 'application/json' -Body $updBody }
}

$searchName = if($p1Id){ [uri]::EscapeDataString("$p1Name-UPD") } else { '' }
$null = Invoke-TimedRequest -Name 'GET /api/products?searchTerm=' -Request { Invoke-WebRequest -Uri "$base/api/products?pageNumber=1&pageSize=10&searchTerm=$searchName" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$null = Invoke-TimedRequest -Name 'GET /api/products?categoryId=' -Request { Invoke-WebRequest -Uri "$base/api/products?pageNumber=1&pageSize=10&categoryId=$catId" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$lowResp = Invoke-TimedRequest -Name 'GET /api/products/low-stock' -Request { Invoke-WebRequest -Uri "$base/api/products/low-stock?threshold=10" -Headers $headers -UseBasicParsing -Method Get -TimeoutSec 30 }
$lowObj = if($lowResp){ $lowResp.Content | ConvertFrom-Json } else { @() }
$lowIds = @($lowObj | ForEach-Object { $_.id })

$afterStatsObj = Invoke-RestMethod -Uri "$base/api/dashboard/stats" -Headers $headers -Method Get
$afterProducts=[int]$afterStatsObj.totalProducts
$afterStock=[decimal]$afterStatsObj.totalStockValue
$afterLow=[int]$afterStatsObj.lowStockCount

$expectedDeltaProducts=2
$expectedDeltaStock=([decimal](12*5 + 2*20))
$actualDeltaProducts=$afterProducts-$beforeProducts
$actualDeltaStock=$afterStock-$beforeStock
$actualDeltaLow=$afterLow-$beforeLow

if($p1Id){ $null = Invoke-TimedRequest -Name 'DELETE /api/products/{id} (p1)' -Request { Invoke-WebRequest -Uri "$base/api/products/$p1Id" -Headers $headers -UseBasicParsing -Method Delete -TimeoutSec 30 } }
if($p2Id){ $null = Invoke-TimedRequest -Name 'DELETE /api/products/{id} (p2)' -Request { Invoke-WebRequest -Uri "$base/api/products/$p2Id" -Headers $headers -UseBasicParsing -Method Delete -TimeoutSec 30 } }

$authCheck = Invoke-TimedRequest -Name 'GET /api/products (no auth check)' -Request { Invoke-WebRequest -Uri "$base/api/products?pageNumber=1&pageSize=1" -UseBasicParsing -Method Get -TimeoutSec 30 }

$secHeaders = @{}
if($productsGet){
  foreach($k in @('X-Content-Type-Options','X-Frame-Options','X-XSS-Protection','Referrer-Policy','Cache-Control','X-Correlation-ID')){
    $secHeaders[$k] = $productsGet.Headers[$k]
  }
}

$summary = [pscustomobject]@{
  beforeProducts=$beforeProducts; afterProducts=$afterProducts; deltaProducts=$actualDeltaProducts; expectedDeltaProducts=$expectedDeltaProducts;
  beforeStock=$beforeStock; afterStock=$afterStock; deltaStock=$actualDeltaStock; expectedDeltaStock=$expectedDeltaStock;
  beforeLow=$beforeLow; afterLow=$afterLow; deltaLow=$actualDeltaLow;
  lowStockContainsLowProduct = ($lowIds -contains $p1Id);
  lowStockContainsHighProduct = ($lowIds -contains $p2Id);
  securityHeaders=$secHeaders;
  correlationRequested=$correlation;
  authWithoutTokenStatus = if($authCheck){ [int]$authCheck.StatusCode } else { -1 }
}

Write-Output "VALIDATION_RESULTS_START"
$results | ConvertTo-Json -Depth 5
Write-Output "VALIDATION_RESULTS_END"
Write-Output "ASSERTIONS_START"
$summary | ConvertTo-Json -Depth 7
Write-Output "ASSERTIONS_END"
