namespace ETL.Application.Common.DTOs;

public record DataSetDto(Guid Id, string TableName, string UploadedByUserId, DateTime CreatedAt);