namespace ETL.Application.Common.DTOs;

public record PluginDto(Guid Id, Guid PipelineId, int StepOrder, string PluginType, string Configuration, DateTime CreatedAt);