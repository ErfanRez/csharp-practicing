namespace ETL.Application.Common.DTOs.PluginConfigurations;

public class AggregationOperation
{
    public required string Column { get; set; }
    public required string Function { get; set; }
    public required string Alias { get; set; }
}