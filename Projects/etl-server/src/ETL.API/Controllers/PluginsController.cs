using ETL.API.Infrastructure;
using ETL.Application.Common.Constants;
using ETL.Application.WorkFlow.Plugins;
using ETL.Application.WorkFlow.Plugins.AddPlugin;
using ETL.Application.WorkFlow.Plugins.UpdatePlugin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers;

[ApiController]
[Route("api/plugins")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PluginsController(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("{pluginId}")]
    [Authorize(Policy = Policy.CanViewWorkflows)]
    public async Task<IActionResult> GetById(Guid pluginId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPluginByIdQuery(pluginId), ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("add-filter-plugin")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> AddFilterPlugin([FromBody] AddFilterPluginCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("add-aggregation-plugin")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> AddAggregationPlugin([FromBody] AddAggregationPluginCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("update-aggregation-plugin")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Update([FromBody] UpdateAggregationPluginCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "plugin has been updated." });
    }

    [HttpPut("update-filter-plugin")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Update([FromBody] UpdateFilterPluginCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "plugin has been updated." });
    }

    [HttpDelete("delete")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Delete([FromBody] DeletePluginCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "plugin has been deleted." });
    }
}
