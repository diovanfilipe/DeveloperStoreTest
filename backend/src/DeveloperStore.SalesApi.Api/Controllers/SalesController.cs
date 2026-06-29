using DeveloperStore.SalesApi.Api.Requests;
using DeveloperStore.SalesApi.Application.Sales.Commands.CancelSale;
using DeveloperStore.SalesApi.Application.Sales.Commands.CancelSaleItem;
using DeveloperStore.SalesApi.Application.Sales.Commands.CreateSale;
using DeveloperStore.SalesApi.Application.Sales.Commands.UpdateSale;
using DeveloperStore.SalesApi.Application.Sales.Dtos;
using DeveloperStore.SalesApi.Application.Sales.Queries.GetSaleById;
using DeveloperStore.SalesApi.Application.Sales.Queries.GetSales;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.SalesApi.Api.Controllers;

[ApiController]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registra uma nova venda.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSaleCommand(
            request.SaleDate,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName,
            request.Items.Select(MapCreateItem).ToList()), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Listar todas as vendas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SaleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSalesQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>Obter uma venda pelo identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSaleByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>Atualizar uma venda ativa.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new UpdateSaleCommand(
            id,
            request.SaleDate,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName,
            request.Items.Select(MapUpdateItem).ToList()), cancellationToken);

        return Ok(response);
    }

    /// <summary>Cancelar uma venda ativa.</summary>
    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleCommand(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>Cancelar um item de uma venda ativa.</summary>
    [HttpPatch("{saleId:guid}/items/{itemId:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelItem(Guid saleId, Guid itemId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleItemCommand(saleId, itemId), cancellationToken);
        return Ok(response);
    }

    private static SaleItemInputDto MapCreateItem(CreateSaleItemRequest item)
    {
        return new SaleItemInputDto(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
    }

    private static UpdateSaleItemInputDto MapUpdateItem(UpdateSaleItemRequest item)
    {
        return new UpdateSaleItemInputDto(item.ItemId, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
    }
}
