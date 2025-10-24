namespace ETL.Application.Common.DTOs;

public class CreatePipelineRequest
{
    public required string PipelineName { get; set; }
    public required Guid DataSourceId { get; set; }
}