using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.WorkFlow.Pipelines;

public record DeletePipelineCommand(Guid Id) : IRequest<Result>;


public sealed class DeletePipelineHandler : IRequestHandler<DeletePipelineCommand, Result>
{
    private readonly IDeletePipeline _deletePipeline;
    private readonly IGetPipelineById _getPipelineById;

    public DeletePipelineHandler(IDeletePipeline deletePipeline, IGetPipelineById getPipelineById)
    {
        _deletePipeline = deletePipeline ?? throw new ArgumentNullException(nameof(deletePipeline));
        _getPipelineById = getPipelineById ?? throw new ArgumentNullException(nameof(getPipelineById));
    }

    public async Task<Result> Handle(DeletePipelineCommand request, CancellationToken cancellationToken)
    {
        var pipeline = await _getPipelineById.ExecuteAsync(request.Id, cancellationToken);
        if (pipeline == null)
            return Result.Failure(Error.NotFound("PipelineDelete.Failed", $"Pipeline {request.Id} not found!"));

        await _deletePipeline.ExecuteAsync(pipeline, cancellationToken);
        return Result.Success();
    }
}