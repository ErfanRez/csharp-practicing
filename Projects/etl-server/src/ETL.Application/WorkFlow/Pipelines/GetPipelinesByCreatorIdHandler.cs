using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using ETL.Domain.Entities;
using MediatR;

namespace ETL.Application.WorkFlow.Pipelines;

public record GetPipelinesByCreatorIdQuery(string CreatorId) : IRequest<Result<IEnumerable<PipelineDto>>>;

public sealed class GetPipelinesByCreatorIdHandler : IRequestHandler<GetPipelinesByCreatorIdQuery, Result<IEnumerable<PipelineDto>>>
{
    private readonly IGetPipelinesByCreatorId _getPipelinesByCreatorId;

    public GetPipelinesByCreatorIdHandler(IGetPipelinesByCreatorId getPipelinesByCreatorId)
    {
        _getPipelinesByCreatorId =
            getPipelinesByCreatorId ?? throw new ArgumentNullException(nameof(getPipelinesByCreatorId));
    }

    public async Task<Result<IEnumerable<PipelineDto>>> Handle(GetPipelinesByCreatorIdQuery request, CancellationToken cancellationToken)
    {
        var pipelines = await _getPipelinesByCreatorId.ExecuteAsync(request.CreatorId, cancellationToken);
        
        var dtoList = pipelines.Select(p => 
            new PipelineDto(p.Id, p.Name, p.DataSourceId, p.CreatedAt)
        ).ToList();

        return Result.Success<IEnumerable<PipelineDto>>(dtoList);
    }
}