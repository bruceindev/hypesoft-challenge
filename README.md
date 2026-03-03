# Hypesoft Challenge

Projeto completo de gestão de produtos com frontend React + backend .NET 9, autenticação via Keycloak e stack containerizada.

Sem enrolação: clona, sobe com docker e já testa :)

## 🥑 Como rodar (jeito mais tranquilo)

### Pré-requisitos
- Docker Desktop
- Git

### 1) Clonar o repositório
```bash
git clone https://github.com/bruceindev/hypesoft-challenge.git
cd desafio-1
```

### 2) Subir a stack
```bash
docker compose up -d --build
```

### 3) Conferir se subiu tudo
```bash
docker compose ps
```

Serviços esperados:
- `frontend`
- `backend`
- `mongodb`
- `keycloak`
- `nginx`
- `redis`

## 🌐 URLs
- Frontend (via Nginx): `http://localhost:3000`
- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`
- Keycloak: `http://localhost:8080`
- Mongo Express: `http://localhost:8081`

## 🔐 Login (Keycloak)
- `admin / admin123`
- `manager / manager123`
- `user / user123`

> O realm do Keycloak é importado automaticamente pelo `docker compose`.

## ✅ O que está implementado

### Produtos
- Criar, listar, editar e excluir
- Campos: nome, descrição, preço, categoria, estoque
- Busca por nome
- Filtro por categoria
- Paginação

### Categorias
- Criar, listar, editar e excluir
- Associação com produtos

### Estoque
- Atualização manual de estoque
- Destaque para estoque baixo (`< 10`)

### Dashboard
- Total de produtos
- Valor total em estoque
- Lista de produtos com estoque baixo
- Gráfico de produtos por categoria

### Autenticação e autorização
- Login com Keycloak (OIDC)
- Proteção de rotas no frontend
- Permissões por role (`Admin`, `Manager`, `User`)
- Logout integrado

## 🧪 Testes

### Backend
```bash
dotnet test .\backend\tests\Hypesoft.Tests\Hypesoft.Tests.csproj
```

### Frontend
```bash
cd frontend
npm test -- --run
```

### Validação ponta a ponta (opcional)
```powershell
powershell -ExecutionPolicy Bypass -File .\validate-enterprise.ps1
```

## 🧱 Stack técnica

### Frontend
- React 18 + TypeScript
- Vite
- TailwindCSS + componentes UI
- React Query
- React Hook Form + Zod
- Recharts

### Backend
- .NET 9
- Clean Architecture + DDD + CQRS (MediatR)
- EF Core com provider MongoDB
- FluentValidation + AutoMapper
- Serilog
- Rate limiting + security headers + health checks

### Infra
- MongoDB
- Redis
- Keycloak
- Nginx
- Docker Compose

## 🏗️ Decisões arquiteturais (resumo)
- Separação clara por camadas: `Domain`, `Application`, `Infrastructure`, `API`
- CQRS com handlers de comandos e queries
- Segurança centralizada com Keycloak + políticas por role
- Observabilidade com logs estruturados + correlation id
- Deploy local simplificado com stack completa via Docker

## 📋 Checklist dos critérios Hypesoft
- Repositório GitHub público: ✅ (depende da publicação do repo)
- README detalhado: ✅
- Docker Compose funcional: ✅
- Testes automatizados: ✅
- Aplicação funcionando: ✅
- Serviços rodando via Compose: ✅
- Interface funcional e responsiva: ✅
- API com Swagger: ✅
- Guia de instalação/execução: ✅
- Documentação de decisões arquiteturais: ✅

## 🧩 Observação sobre dados de exemplo
- Usuários e roles já vêm prontos via import do Keycloak (dá uns créditos hahah).
- Produtos e categorias podem ser criados direto pela interface após login.

---

