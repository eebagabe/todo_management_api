# Todo Management API

API RESTful para gerenciamento de projetos, tarefas, etiquetas e comentários.

Passo a passo para rodar o projeto localmente.

## Dependências

- .NET SDK 8.0
- PostgreSQL 16+
- dotnet-ef (`dotnet tool install --global dotnet-ef`)

## Configuração

### 1. Clonar o repositório

```bash
git clone git@github.com:eebagabe/todo_management_api.git
cd todo_management_api
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Configurar o ambiente

***Crie um banco de dados localmente***
- Em seu gerenciador de banco de dados, ou internamente no terminal postgres:

```sql
CREATE DATABASE todo_management
```

- Edite a connection string em `src/TodoManagement.Api/appsettings.json` com suas credenciais do PostgreSQL:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=todo_management;Username=postgres;Password=sua_senha"
}
```

Ou defina pela variável de ambiente `ConnectionStrings__Default`.

### 4. Rodar as migrations

```bash
dotnet ef database update -p src/TodoManagement.Api
```

### 5. Subir o servidor

```bash
dotnet run --project src/TodoManagement.Api
```

A API estará disponível em `http://localhost:8000`.

## Endpoints

- `GET|POST /api/projects`, `GET|PUT|DELETE /api/projects/{id}`: projetos
- `GET /api/projects/{id}/tasks`: tarefas de um projeto
- `GET|POST /api/tasks`, `GET|PUT|DELETE /api/tasks/{id}`: tarefas
- `GET|POST /api/labels`, `GET|PUT|DELETE /api/labels/{id}`: etiquetas
- `GET|POST /api/comments`, `GET|PUT|DELETE /api/comments/{id}`: comentários
- `GET /api/overview`: projetos ativos, total de tarefas e tarefas concluídas

As listagens de tarefas aceitam os filtros `status`, `created_at` (`YYYY-MM-DD`), `order_by` (`title` ou `created_at`) e `order_direction` (`asc` ou `desc`).

### Para Executar testes

```bash
dotnet test
```
