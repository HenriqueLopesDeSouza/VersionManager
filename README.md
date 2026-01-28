# VersionManager

Sistema para Gestão de Versões de Software, composto por:

* BackEnd: RESTful API em ASP.NET Core (.NET 8)
* FrontEnd: SPA em React + Vite, servida via Nginx
* Banco de Dados: SQL Server
* Orquestração: Docker Compose

O projeto segue princípios de Clean Architecture, DDD e boas práticas de design de APIs, com foco em organização, manutenibilidade e clareza arquitetural.


## Visão Geral da Arquitetura (BackEnd)

O BackEnd é organizado em camadas bem definidas, garantindo baixo acoplamento, alta coesão e facilidade de manutenção.

<img width="279" height="600" alt="image" src="https://github.com/user-attachments/assets/9f616bac-718c-4c16-ab03-99d8b1758821" />

### Princípios e padrões aplicados (BackEnd)

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID
- Repository Pattern
- Unit of Work
- DTOs para isolamento da API
- Validações centralizadas no domínio
- Tratamento global de exceções com `ProblemDetails`
- Paginação padronizada

## Visão Geral da Arquitetura (FrontEnd)

O FrontEnd é uma Single Page Application (SPA) desenvolvida com React + Vite + TypeScript, seguindo princípios de componentização, separação de responsabilidades e comunicação desacoplada com a API.
A aplicação é buildada em tempo de deploy e servida via Nginx, que também atua como reverse proxy para a API.

<img width="355" height="599" alt="image" src="https://github.com/user-attachments/assets/e6e53c00-96b8-41a0-a54b-b93f36c99dc9" />


### Princípios e padrões aplicados (FrontEnd)

- Single Page Application (SPA)
- Componentização
- Separação de responsabilidades
- Camada dedicada para acesso à API
- Roteamento por páginas
- Imutabilidade de estado
- Configuração de proxy para evitar CORS

## Regras de Negócio

### Software

- O nome do software é obrigatório, normalizado (trim) e deve possuir no mínimo 2 caracteres.
- Não é permitido cadastrar ou atualizar um software com nome duplicado.
- A descrição é opcional e armazenada como `null` quando vazia.
- A data de criação é definida automaticamente em UTC.
- Para atualização parcial (PATCH), ao menos um campo deve ser informado.
- Não é possível atualizar ou remover um software inexistente.

---

### Versões de Software

- Versões pertencem obrigatoriamente a um software existente.
- Não é permitido cadastrar versões duplicadas para o mesmo software.
- O identificador da versão é obrigatório e normalizado (trim).
- Cada versão possui:
  - Número da versão
  - Data de lançamento
  - Indicador de descontinuação
- Ao marcar uma versão como descontinuada, a data de descontinuação é definida automaticamente em UTC.
- Atualizações parciais exigem ao menos um campo informado.
- Não é possível remover versões inexistentes.

---

### Regras Gerais

- Todas as validações de negócio são aplicadas no domínio.
- Violações de regras resultam em `DomainException`.
- As alterações são persistidas via Unit of Work.
- Operações inválidas não alteram o estado do sistema.

## Como Executar o Projeto (Passo a Passo)
 Pré-requisitos
 - Docker
 - Docker Compose
 - .NET 8 SDK (apenas para a primeira migration)

⚠️ Primeira Execução (Obrigatório)

 Na primeira execução, o banco ainda não existe, portanto é necessário executar manualmente a migration inicial.

1️⃣ Subir o SQL Server

 Na raiz do projeto:
 ```bash
docker compose up -d sqlserver
```

2️⃣ Executar as migrations

Entre na pasta do backend (onde está o .sln):
```bash
cd BackEnd
```
Instale a ferramenta do EF Core (caso ainda não tenha):
```bash
dotnet tool install --global dotnet-ef
```

Restaurar NuGet
```bash
dotnet restore VersionManager.sln
```

Crie a migration inicial e atualize o banco:
```bash
dotnet ef migrations add InitialCreate --project src\VersionManager.Infrastructure\VersionManager.Infrastructure.csproj --startup-project src\VersionManager.Api\VersionManager.Api.csproj
dotnet ef database update --project src\VersionManager.Infrastructure\VersionManager.Infrastructure.csproj --startup-project src\VersionManager.Api\VersionManager.Api.csproj
```

3️⃣ Subir toda a stack

Volte para a raiz do projeto e execute:
```bash
cd ..
docker compose up --build
```

🔁 Próximas Execuções

Após a primeira execução:
```bash
docker compose up
```
Não é necessário rodar migrations novamente, pois o banco fica persistido no volume Docker.

### URLs do Sistema

* Frontend:
http://localhost:3000
* API:
http://localhost:8080

## Observações Técnicas

* O FrontEnd consome a API através do proxy /api, configurado no Nginx, evitando problemas de CORS.
* A comunicação entre os serviços ocorre via rede interna do Docker.
* O banco de dados utiliza volume Docker para persistência.

### Para resetar completamente o ambiente:

```bash
docker compose down -v
```
