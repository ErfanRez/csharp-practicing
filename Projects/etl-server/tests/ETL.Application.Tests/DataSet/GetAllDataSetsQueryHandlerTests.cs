using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common.DTOs;
using ETL.Application.DataSet;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class GetAllDataSetsQueryHandlerTests
{
    private readonly IGetAllDataSets _getAllDataSets;
    private readonly GetAllDataSetsQueryHandler _sut;

    public GetAllDataSetsQueryHandlerTests()
    {
        _getAllDataSets = Substitute.For<IGetAllDataSets>();
        _sut = new GetAllDataSetsQueryHandler(_getAllDataSets);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetAllDataSetsIsNull()
    {
        // Act
        Action act = () => new GetAllDataSetsQueryHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getAllDataSets");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoDataSetsExist()
    {
        // Arrange
        _getAllDataSets.ExecuteAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<DataSetMetadata>>(Array.Empty<DataSetMetadata>()));
        var query = new GetAllDataSetsQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDataSets_WhenDataSetsExist()
    {
        // Arrange
        var dataSets = new[]
        {
            new DataSetMetadata("table1", "user1"),
            new DataSetMetadata("table2", "user2")
        }.ToList();

        _getAllDataSets.ExecuteAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<DataSetMetadata>>(dataSets));

        var expected = dataSets.Select(d => new DataSetDto(d.Id, d.TableName, d.UploadedByUserId, d.CreatedAt)).ToList();
        var query = new GetAllDataSetsQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(expected);
    }
}
