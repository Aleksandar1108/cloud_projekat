using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Properties.Commands;
using SmartGrid.Application.Features.Properties.Queries;
using SmartGrid.Application.Features.SmartMeters.Commands;
using SmartGrid.Application.Features.SmartMeters.Queries;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers;

[Route("api/properties")]
[ApiController]
[Authorize]
public class PropertiesController(IMediator mediator) : ControllerBase
{
    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst("id")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    // ── Properties ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetProperties()
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new GetPropertiesQuery(userId.Value));
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new GetPropertyByIdQuery(id, userId.Value));
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var command = new CreatePropertyCommand(userId.Value, request.Name, request.City, request.Address, request.Description, request.PropertyType);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var command = new UpdatePropertyCommand(id, userId.Value, request.Name, request.City, request.Address, request.Description, request.PropertyType);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProperty(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new DeletePropertyCommand(id, userId.Value));
        return result.ToActionResult();
    }

    // ── Smart Meters ─────────────────────────────────────────────────────────

    [HttpGet("{propertyId:guid}/smart-meters")]
    public async Task<IActionResult> GetSmartMeters(Guid propertyId)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new GetSmartMetersQuery(propertyId, userId.Value));
        return result.ToActionResult();
    }

    [HttpPost("{propertyId:guid}/smart-meters")]
    public async Task<IActionResult> AddSmartMeter(Guid propertyId, [FromBody] AddSmartMeterRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var command = new AddSmartMeterCommand(propertyId, userId.Value, request.Label, request.ConnectionType, request.Note);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{propertyId:guid}/smart-meters/{meterId:guid}")]
    public async Task<IActionResult> UpdateSmartMeter(Guid propertyId, Guid meterId, [FromBody] UpdateSmartMeterRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var command = new UpdateSmartMeterCommand(meterId, propertyId, userId.Value, request.Label, request.ConnectionType, request.Note);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{propertyId:guid}/smart-meters/{meterId:guid}")]
    public async Task<IActionResult> DeleteSmartMeter(Guid propertyId, Guid meterId)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new DeleteSmartMeterCommand(meterId, propertyId, userId.Value));
        return result.ToActionResult();
    }

    [HttpPost("{propertyId:guid}/smart-meters/{meterId:guid}/register-serial")]
    public async Task<IActionResult> RegisterSerialNumber(Guid propertyId, Guid meterId, [FromBody] RegisterSerialNumberRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var command = new RegisterSerialNumberCommand(meterId, propertyId, userId.Value, request.SerialNumber);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}
