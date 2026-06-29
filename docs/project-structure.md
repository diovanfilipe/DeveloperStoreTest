# Estrutura do Projeto

O repositório mantém um projeto por camada e um projeto dedicado para testes.

```text
README.md
docs/
backend/
  DeveloperStore.SalesApi.sln
  src/
    DeveloperStore.SalesApi.Api/
    DeveloperStore.SalesApi.Application/
    DeveloperStore.SalesApi.Domain/
    DeveloperStore.SalesApi.Infrastructure/
    DeveloperStore.SalesApi.Tests/
```

## Api

Contém a entrada HTTP:

- `Controllers/` com os endpoints de vendas;
- `Requests/` com contratos da API e atributos de validação;
- `Middleware/` com o tratamento de exceções;
- `appsettings.json` com a connection string do SQLite.

## Application

Contém os casos de uso:

- `Sales/Commands/` para operações de escrita;
- `Sales/Queries/` para operações de leitura;
- `Sales/Dtos/` para respostas e modelos de entrada da aplicação;
- `Abstractions/` para contratos compartilhados entre camadas, como publicação de eventos.

## Domain

Contém o modelo de negócio:

- `Entities/` para `Sale` e `SaleItem`;
- `Enums/` para `SaleStatus`;
- `Repositories/` para os contratos de repositório;
- `Exceptions/` para violações de regra de negócio.

## Infrastructure

Contém as implementações técnicas:

- `Repository/` para persistência com Dapper e SQLite;
- `Logging/` para publicação de eventos por meio de logs;
- configuração de injeção de dependência da camada.

## Tests

Contém os testes automatizados:

- `Domain/` para testes do agregado e das regras de negócio;
- `Application/` para testes de handlers com mocks.

## Observações

Os projetos ficam organizados abaixo de `backend/src` para seguir um padrão comum em soluções .NET. A pasta `docs` permanece na raiz para manter a documentação facilmente acessível.
