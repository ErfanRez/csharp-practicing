using ETL.Application.Abstractions.Repositories;
using ETL.Application.DataSet;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class GetTableByNameQueryHandlerTests
{
    private readonly IGetStagingTableByName _getStagingTableByName;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly GetTableByNameQueryHandler _sut;

    public GetTableByNameQueryHandlerTests()
    {
        _getStagingTableByName = Substitute.For<IGetStagingTableByName>();
        _getByTableName = Substitute.For<IGetDataSetByTableName>();
        _sut = new GetTableByNameQueryHandler(_getStagingTableByName, _getByTableName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetStagingTableByNameIsNull()
    {
        // Arrange // Act
        Action act = () => new GetTableByNameQueryHandler(null!, _getByTableName);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getStagingTableByName");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetByTableNameIsNull()
    {
        // Arrange // Act
        Action act = () => new GetTableByNameQueryHandler(_getStagingTableByName, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getByTableName");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDatasetNotFound()
    {
        // Arrange
        var table = "missing_table";
        var query = new GetTableByNameQuery(table);
        _getByTableName.ExecuteAsync(table, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(null));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Table.Get.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDatasetExistsAndStagingReturnsString()
    {
        // Arrange
        var table = "existing_table";
        var query = new GetTableByNameQuery(table);
        var dataset = new DataSetMetadata(table, "owner");
        var expected = "[{\"id\":1}]";

        _getByTableName.ExecuteAsync(table, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(dataset));
        _getStagingTableByName.ExecuteAsync(table, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
