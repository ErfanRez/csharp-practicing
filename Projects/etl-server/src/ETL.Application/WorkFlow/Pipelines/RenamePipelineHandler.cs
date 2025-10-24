using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.WorkFlow.Pipelines;

public record RenamePipelineCommand(Guid Id, string NewName) : IRequest<Result>;

public sealed class RenamePipelineHandler : IRequestHandler<RenamePipelineCommand, Result>
{
    private readonly IUpdatePipeline _updatePipeline;
    private readonly IGetPipelineById _getPipelineById;

    public RenamePipelineHandler(IUpdatePipeline updatePipeline, IGetPipelineById getPipelineById)
    {
        _updatePipeline = updatePipeline ?? throw new ArgumentNullException(nameof(updatePipeline));
        _getPipelineById = getPipelineById ?? throw new ArgumentNullException(nameof(getPipelineById));
    }

    public async Task<Result> Handle(RenamePipelineCommand request, CancellationToken cancellationToken)
    {
        var pipeline = await _getPipelineById.ExecuteAsync(request.Id, cancellationToken);
        if (pipeline == null)
            return Result.Failure(Error.NotFound("PipelineDelete.Failed", $"Pipeline {request.Id} not found!"));

        await _updatePipeline.ExecuteAsync(request.Id, request.NewName, cancellationToken);
        return Result.Success();
    }
}