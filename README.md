# ==============================================
# 🚀 SHOPPING TODO API - SETUP COMPLETO
# ==============================================

# ================================
# 📌 PRÉ-REQUISITOS
# ================================
# - .NET SDK 10
# - PostgreSQL
# - Git (opcional)

dotnet --version

# ================================
# 📥 CLONAR PROJETO
# ================================
git clone <URL_DO_REPOSITORIO>
cd shopping

# ================================
# 📦 RESTAURAR DEPENDÊNCIAS
# ================================
dotnet restore

# ================================
# 📦 INSTALAR PACOTES NECESSÁRIOS
# ================================
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.2
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.2
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.2

dotnet tool install --global dotnet-ef

# ================================
# 🗄️ CRIAR BANCO POSTGRESQL
# ================================
psql -U postgres

CREATE DATABASE testing;
\q

# ================================
# ⚙️ CONFIGURAR STRING DE CONEXÃO
# ================================
# Editar appsettings.json:

# "ConnectionStrings": {
#   "DefaultConnection": "Host=localhost;Database=testing;Username=postgres;Password=SUA_SENHA"
# }

# ================================
# 🏗️ CRIAR MIGRATION
# ================================
dotnet ef migrations add InitialCreate

# ================================
# 🗄️ APLICAR MIGRATION
# ================================
dotnet ef database update

# ================================
# ▶️ RODAR APLICAÇÃO
# ================================
dotnet run

# ================================
# 📡 TESTAR ENDPOINT
# ================================
curl http://localhost:5059/api/v1/todo

# ================================
# 🧪 VERIFICAR TABELAS
# ================================
psql -U postgres -d testing -c "\dt"

# ==============================================
# 📂 ESTRUTURA DO PROJETO
# ==============================================

shopping/
│
├── Controllers/
│   └── ToDoController.cs
│
├── Services/
│   ├── IToDoService.cs
│   └── ToDoService.cs
│
├── Repository/
│   ├── IToDoRepository.cs
│   └── ToDoRepository.cs
│
├── Data/
│   ├── ToDoContext.cs
│   └── Migrations/
│
├── Models/
│   └── ToDoItem.cs
│
├── Dto/
│   ├── CreateToDoItemDto.cs
│   ├── UpdateToDoItemDto.cs
│   └── ToDoItemSummaryDto.cs
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs
├── appsettings.json
└── shopping.csproj

# ==============================================
# 🏗️ ARQUITETURA
# ==============================================

# Controller  -> HTTP
# Service     -> Regras de negócio
# Repository  -> Acesso a dados
# Data        -> EF Core
# Models      -> Entidades
# DTO         -> Transferência de dados

# Fluxo:
# Request -> Controller -> Service -> Repository -> DbContext -> PostgreSQL
