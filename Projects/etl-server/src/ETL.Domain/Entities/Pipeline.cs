using ETL.Domain.Common;

namespace ETL.Domain.Entities;

public class Pipeline : BaseEntity
{
    public string Name { get; private set; }

    // Foreign key to DataSetMetadata
    public Guid DataSourceId { get; private set; }
    public DataSetMetadata DataSource { get; private set; }
    
    public string CreatedByUserId { get; private set; }


    // Navigation to plugins
    public List<Plugin> Plugins { get; private set; } = new();

    private Pipeline() { } // Required by EF Core

    public Pipeline(string name, Guid dataSourceId, string createdByUserId)
    {
        Name = name;
        DataSourceId = dataSourceId;
        CreatedByUserId =  createdByUserId;
    }
    
    public void Rename(string newName)
    {
        Name = newName;
    }
}