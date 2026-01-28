# VersionManager

RESTful API desenvolvida em **ASP.NET Core** para gerenciamento de **Softwares** e suas **Versões**, seguindo princípios de **Clean Architecture**, **DDD** e **boas práticas de design de APIs**.

## Visão Geral da Arquitetura

O projeto é organizado em camadas bem definidas, garantindo baixo acoplamento, alta coesão e facilidade de manutenção.

<img width="279" height="600" alt="image" src="https://github.com/user-attachments/assets/9f616bac-718c-4c16-ab03-99d8b1758821" />


## Princípios e padrões aplicados

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID
- Repository Pattern
- Unit of Work
- DTOs para isolamento da API
- Validações centralizadas no domínio
- Tratamento global de exceções com `ProblemDetails`
- Paginação padronizada

## Tecnologias utilizadas

- NET 8
- ASP.NET Core
- Entity Framework Core
- SQL Server
- xUnit
- FluentAssertions
- Docker

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


## Testes Automatizados

O projeto possui testes unitários focados nas regras de negócio, priorizando:
- Invariantes do domínio
- Validações críticas de negócio
- Garantia de consistência das entidades

Os testes seguem o padrão AAA (Arrange, Act, Assert) e utilizam xUnit e FluentAssertions para maior legibilidade.

Estrutura de testes

<img width="281" height="146" alt="image" src="https://github.com/user-attachments/assets/2b3573ba-1a8b-4245-9c88-2d96c85ad5b2" />

## Executando os testes
Na raiz do repositório:

```bash
dotnet test
```

## Como rodar o projeto (Local + Docker)

### Pré-requisitos
- 8 .net sdk (local)
- Docker e Docker Compose

## Subindo o banco de dados (SQL Server)

Na raiz do repositório (onde está o `.sln`):

Instale a ferramenta do EF Core e suba as Migrations

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p src/VersionManager.Infrastructure -s src/VersionManager.Api
```

depois suba o docker

```bash
docker compose up --build
```
Depois acesse o Swagger

http://localhost:8080/swagger/index.html

<img width="1485" height="485" alt="image" src="https://github.com/user-attachments/assets/06874134-2dc1-44e1-ba6b-efa3902b0272" />

