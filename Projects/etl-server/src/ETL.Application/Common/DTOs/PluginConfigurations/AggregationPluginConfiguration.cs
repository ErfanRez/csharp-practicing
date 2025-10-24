namespace ETL.Application.Common.DTOs.PluginConfigurations;

public class AggregationPluginConfiguration
{
    public List<string> GroupByColumns { get; set; } = new();
    public List<AggregationOperation> Aggregations { get; set; } = new();
}