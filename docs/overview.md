# Visão Geral

A DeveloperStore Sales API é uma Web API em `.NET 8` construída para modelar um domínio de vendas com uma abordagem pragmática de Clean Architecture.

A solução foi mantida simples:

- `Api` expõe os endpoints HTTP e a documentação do Swagger.
- `Application` orquestra os casos de uso com MediatR e CQRS.
- `Domain` concentra o agregado de vendas e as regras de negócio.
- `Infrastructure` fornece persistência com Dapper e SQLite em memória.
- `Tests` valida o comportamento do domínio e da aplicação com xUnit.

## O que este projeto demonstra

Esta implementação foi pensada para avaliar a capacidade de:

- modelar um fluxo de vendas com regras de negócio claras;
- manter o domínio consistente durante operações de criação, consulta, atualização e cancelamento;
- organizar o código com responsabilidades separadas por camada;
- implementar handlers de CQRS sem introduzir uma service layer desnecessária;
- escrever testes que protejam os principais cenários de negócio.

## Escopo do domínio

A API gerencia registros de vendas com informações denormalizadas de cliente, filial e produtos. Os principais comportamentos cobertos são:

- criação do registro da venda com itens;
- atualização de uma venda ativa preservando a identidade dos itens quando possível;
- cancelamento de uma venda e de seus itens ativos;
- cancelamento individual de item;
- cálculo de totais e descontos com base nas regras por quantidade.

## Regras de negócio

- quantidades abaixo de 4 não recebem desconto;
- quantidades de 4 a 9 recebem 10% de desconto;
- quantidades de 10 a 20 recebem 20% de desconto;
- quantidades acima de 20 são rejeitadas;
- uma venda cancelada não pode ser atualizada nem cancelada novamente;
- um item cancelado não pode ser cancelado novamente.

## Decisões tomadas

Algumas escolhas foram feitas para manter a solução coerente com o domínio e simples de avaliar:

- o cancelamento da venda foi exposto com `PATCH` em vez de `DELETE`, porque cancelar é uma mudança de estado, não uma exclusão física;
- a rota pública de `DELETE` não foi criada, evitando sugerir remoção definitiva de vendas que precisam permanecer como histórico de negócio;
- a persistência foi baseada em SQLite em memória para facilitar a execução local durante a avaliação, sem exigir infraestrutura externa;
- a atualização da venda passou a aceitar a identificação dos itens, permitindo preservar o `Id` dos itens existentes e evitar perda de histórico ao editar;
- os itens ausentes no payload de atualização são removidos do agregado, mas o estado da venda continua consistente;

## Estratégia de persistência

O projeto não usa um servidor de banco de dados externo. Os dados ficam em SQLite em memória com uma conexão compartilhada durante a execução da aplicação. Isso mantém a configuração leve e fácil de executar durante a avaliação.

## Observações de API

- `PATCH /api/sales/{id}/cancel` é usado para cancelar uma venda.
- `PATCH /api/sales/{saleId}/items/{itemId}/cancel` cancela um item específico.
- Os XML comments estão habilitados e expostos no Swagger para facilitar a inspeção dos endpoints.

## Evoluções futuras

Algumas melhorias foram deixadas como próximos passos para uma versão mais completa do sistema:

- `Idempotency-Key` para evitar duplicidade na criação de vendas em cenários de reenvio da mesma requisição;
- `PostgreSQL` como banco relacional definitivo para persistência mais robusta;
- `Integration Tests` para validar a API completa com infraestrutura real de execução;
- `Message Broker` para publicar eventos de domínio de forma assíncrona, substituindo a publicação por log.
