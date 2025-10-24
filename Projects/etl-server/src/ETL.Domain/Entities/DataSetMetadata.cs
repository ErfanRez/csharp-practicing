using ETL.Domain.Common;

namespace ETL.Domain.Entities;

public class DataSetMetadata : BaseEntity
{
    public string TableName { get; private set; }
    public string UploadedByUserId { get; private set; }

    private DataSetMetadata() { }

    public DataSetMetadata(string tableName, string uploadedByUserId)
    {
        TableName = tableName;
        UploadedByUserId = uploadedByUserId;
    }

    public void Rename(string newName)
    {
        TableName = newName;
    }
}