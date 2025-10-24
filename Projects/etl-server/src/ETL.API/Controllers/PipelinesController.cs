using System.Security.Claims;
using ETL.API.Infrastructure;
using ETL.Application.Common.Constants;
using ETL.Application.Common.DTOs;
using ETL.Application.WorkFlow.Pipelines;
using ETL.Application.WorkFlow.Plugins;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers;

[ApiController]
[Route("api/pipelines")]
public class PipelinesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PipelinesController(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [Authorize(Policy = Policy.CanViewWorkflows)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllPipelinesQuery(), ct);
        return Ok(result.Value);
    }
    [HttpGet("my-pipelines")]
    [Authorize(Policy = Policy.CanViewWorkflows)]
    public async Task<IActionResult> GetMyPipelines(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var result = await _mediator.Send(new GetPipelinesByCreatorIdQuery(userId), ct);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{pipelineId}")]
    [Authorize(Policy = Policy.CanViewWorkflows)]
    public async Task<IActionResult> GetById(Guid pipelineId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPipelineByIdQuery(pipelineId), ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{pipelineId}/get-plugins")]
    [Authorize(Policy = Policy.CanViewWorkflows)]
    public async Task<IActionResult> GetAll(Guid pipelineId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPluginsByPipelineIdQuery(pipelineId), ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("create-pipeline")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Create([FromBody] CreatePipelineRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var command = new CreatePipelineCommand(request.PipelineName, request.DataSourceId, userId!);

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);
        return Ok(result.Value);
    }

    [HttpPut("rename-pipeline")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Rename(RenamePipelineCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);
        return Ok(new { message = "pipeline has been renamed." });
    }

    [HttpDelete("delete-pipeline")]
    [Authorize(Policy = Policy.CanManageWorkflows)]
    public async Task<IActionResult> Delete(DeletePipelineCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);
        return Ok(new { message = "Pipeline has been removed." });
    }
}