# Stack Técnica

Este projeto usa um conjunto pequeno de tecnologias escolhidas para clareza, manutenibilidade e facilidade de execução durante a avaliação.

## Backend

- `.NET 8`
- `ASP.NET Core Web API`
- `C#`

## Documentação da API

- `Swagger / Swashbuckle`
- XML comments habilitados em controllers e DTOs

## Camada de aplicação

- `MediatR`
- commands e queries no estilo CQRS

## Domínio e persistência

- `Dapper`
- `Microsoft.Data.Sqlite`
- SQLite em memória com conexão compartilhada

## Validação e serialização

- `System.ComponentModel.DataAnnotations`
- `System.Text.Json`

## Testes

- `xUnit`
- `Moq`
- `FluentAssertions`
- `coverlet.collector`

## Por que essa stack

A stack foi mantida intencionalmente enxuta para reduzir o custo de setup e evitar excesso de abstração. Ela entrega:

- execução local rápida;
- orquestração explícita dos casos de uso;
- persistência leve e facilitada para avaliação;
- testes diretos e legíveis;
- Clareza e facilidade da API com Swagger.
