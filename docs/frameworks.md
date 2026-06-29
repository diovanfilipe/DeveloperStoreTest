# Frameworks

Esta solução utiliza alguns frameworks e bibliotecas com foco e propósito bem definidos.

## ASP.NET Core Web API

Usado como camada HTTP para controllers, binding de requests, validação e exposição do Swagger.

## Swashbuckle

Gera a documentação OpenAPI e a interface do Swagger para a API. Os XML comments são incluídos para facilitar a compreensão dos endpoints e contratos.

## MediatR

Usado para implementar CQRS sem introduzir uma service layer. Os controllers enviam commands e queries, e os handlers ficam responsáveis pela orquestração do caso de uso.

## Dapper

Usado na camada de Infrastructure para mapeamento SQL direto. Isso mantém a persistência leve e explícita.

## Microsoft.Data.Sqlite

Fornece o motor SQLite utilizado em memória durante a execução. A conexão é compartilhada para que schema e dados permaneçam disponíveis enquanto a aplicação estiver rodando.

## xUnit

Framework principal de testes unitários nas camadas de Domain e Application.

## Moq

Usado para isolar dependências nos testes de handlers.

## FluentAssertions

Usado para manter as asserções legíveis e focadas na intenção do teste.

## Data Annotations

Usadas nos contratos de request da API para validações simples, como obrigatoriedade e limites numéricos.

## Logging e middleware nativos

O logging do ASP.NET Core e o middleware customizado de tratamento de exceções são usados para manter o tratamento de erros consistente sem adicionar dependências extras.
