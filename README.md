# Developer Store API

API REST em .NET 8 para gerenciamento de vendas da DeveloperStore, implementada com DDD, CQRS/MediatR, Entity Framework Core e PostgreSQL.

## Repositorio

Repositorio publico para avaliacao:

https://github.com/MelqueLord/developer-store-api

## Use Case

O projeto implementa uma API completa para registros de vendas, usando o padrao External Identities para referenciar entidades de outros dominios e manter descricoes denormalizadas.

A venda informa:

- Numero da venda
- Data da venda
- Cliente
- Valor total da venda
- Filial
- Produtos
- Quantidades
- Precos unitarios
- Descontos
- Valor total de cada item
- Status cancelada/nao cancelada

Tambem foram implementados eventos de dominio, registrados no log da aplicacao:

- `SaleCreated`
- `SaleModified`
- `SaleCancelled`
- `ItemCancelled`

## Regras De Negocio

- Compras com 4 a 9 unidades identicas recebem 10% de desconto.
- Compras com 10 a 20 unidades identicas recebem 20% de desconto.
- Nao e permitido vender mais de 20 unidades identicas do mesmo produto.
- Compras com menos de 4 unidades nao recebem desconto.
- O total da venda considera apenas itens nao cancelados.
- Uma venda cancelada nao pode ser alterada.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR
- AutoMapper
- FluentValidation
- Serilog
- xUnit, FluentAssertions, NSubstitute e Bogus
- Docker e Docker Compose

## Estrutura Do Projeto

```text
template/backend
|-- src
|   |-- Ambev.DeveloperEvaluation.WebApi
|   |-- Ambev.DeveloperEvaluation.Application
|   |-- Ambev.DeveloperEvaluation.Domain
|   |-- Ambev.DeveloperEvaluation.ORM
|   |-- Ambev.DeveloperEvaluation.Common
|   `-- Ambev.DeveloperEvaluation.IoC
`-- tests
    |-- Ambev.DeveloperEvaluation.Unit
    |-- Ambev.DeveloperEvaluation.Integration
    `-- Ambev.DeveloperEvaluation.Functional
```

## Pre-requisitos

- .NET SDK 8.0
- Docker Desktop
- Git

## Configuracao

Clone o repositorio:

```bash
git clone https://github.com/MelqueLord/developer-store-api.git
cd developer-store-api/template/backend
```

A connection string padrao da API esta configurada para o PostgreSQL local exposto pelo `docker-compose.yml`:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
```

Quando a API roda dentro do Docker Compose, a connection string e sobrescrita para usar o host interno do servico PostgreSQL:

```text
Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n
```

## Executando Com Docker

Na pasta `template/backend`, execute:

```bash
docker compose up --build
```

A API ficara disponivel em:

```text
http://localhost:8080
```

Swagger, em ambiente de desenvolvimento:

```text
http://localhost:8080/swagger
```

## Executando Localmente

Suba somente os servicos de infraestrutura:

```bash
docker compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
```

Restaure as dependencias:

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
```

Aplique as migrations:

```bash
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Execute a API:

```bash
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

## Testes

Na pasta `template/backend`, execute:

```bash
dotnet test Ambev.DeveloperEvaluation.sln
```

Para gerar relatorio de cobertura:

```bash
./coverage-report.sh
```

No Windows:

```powershell
.\coverage-report.bat
```

## Endpoints De Sales

- `POST /api/sales` cria uma venda.
- `GET /api/sales` lista vendas com paginacao, ordenacao e filtros.
- `GET /api/sales/{id}` busca uma venda por id.
- `PUT /api/sales/{id}` atualiza uma venda.
- `DELETE /api/sales/{id}` cancela uma venda.
- `DELETE /api/sales/{saleId}/items/{itemId}` cancela um item da venda.

Filtros disponiveis na listagem:

- `_page`
- `_size`
- `_order`
- `saleNumber`
- `customerName`
- `branchName`
- `isCancelled`
- `_minSaleDate`
- `_maxSaleDate`
- `_minTotalAmount`
- `_maxTotalAmount`

## Exemplo De Criacao De Venda

```json
{
  "saleNumber": "SALE-0001",
  "saleDate": "2026-06-22T10:00:00Z",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "customerName": "Customer Example",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Main Branch",
  "items": [
    {
      "productId": "33333333-3333-3333-3333-333333333333",
      "productName": "Product Example",
      "quantity": 10,
      "unitPrice": 100.00
    }
  ]
}
```

## Documentacao Complementar

- [Overview](./.doc/overview.md)
- [Tech Stack](./.doc/tech-stack.md)
- [Frameworks](./.doc/frameworks.md)
- [Project Structure](./.doc/project-structure.md)
