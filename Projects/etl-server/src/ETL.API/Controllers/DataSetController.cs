using System.Security.Claims;
using ETL.API.Infrastructure;
using ETL.Application.Common.Constants;
using ETL.Application.DataSet;
using ETL.Application.DataSet.DeleteColumn;
using ETL.Application.DataSet.DeleteTable;
using ETL.Application.DataSet.RenameColumn;
using ETL.Application.DataSet.RenameTable;
using ETL.Application.DataSet.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers;

[ApiController]
[Route("api/datasets")]
public class DataSetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DataSetsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("upload")]
    [Authorize(Policy = Policy.CanUploadFile)]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string tableName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cmd = new UploadCsvCommand(tableName, file.OpenReadStream(), userId!);
        var result = await _mediator.Send(cmd, cancellationToken);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "File has been stored in database." });
    }

    [HttpGet("{tableName}")]
    [Authorize(Policy = Policy.CanReadDataSets)]
    public async Task<IActionResult> GetTable(string tableName, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTableByNameQuery(tableName), cancellationToken);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("all")]
    [Authorize(Policy = Policy.CanReadDataSets)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllDataSetsQuery(), cancellationToken);
        return Ok(result.Value);
    }

    [HttpPut("rename-table")]
    [Authorize(Policy = Policy.CanManipulateDataSets)]
    public async Task<IActionResult> RenameTable([FromBody] RenameTableCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "Table has been renamed." });
    }

    [HttpPut("rename-column")]
    [Authorize(Policy = Policy.CanManipulateDataSets)]
    public async Task<IActionResult> RenameColumn(RenameColumnCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);


        return Ok(new { message = "Column has been renamed." });
    }


    [HttpDelete("remove-table")]
    [Authorize(Policy = Policy.CanManipulateDataSets)]
    public async Task<IActionResult> DeleteTable(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "Table has been removed." });
    }

    [HttpDelete("remove-column")]
    [Authorize(Policy = Policy.CanManipulateDataSets)]
    public async Task<IActionResult> DeleteColumn(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "Column has been removed." });
    }
}