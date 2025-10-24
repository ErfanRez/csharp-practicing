namespace ETL.Application.Common.DTOs.PluginConfigurations;

public class FilterPluginConfiguration
{
    public required string Column { get; set; }
    public required string Operation { get; set; }
    public required string Value { get; set; }
}