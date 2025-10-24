using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using ETL.Application.DataSet.DeleteTable;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class DeleteTableCommandHandlerTests
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IDeleteStagingTable _deleteStagingTable;
    private readonly IDeleteDataSet _deleteDataSet;
    private readonly DeleteTableCommandHandler _sut;

    public DeleteTableCommandHandlerTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _getByTableName = Substitute.For<IGetDataSetByTableName>();
        _deleteStagingTable = Substitute.For<IDeleteStagingTable>();
        _deleteDataSet = Substitute.For<IDeleteDataSet>();

        _sut = new DeleteTableCommandHandler(_uow, _getByTableName, _deleteStagingTable, _deleteDataSet);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UowIsNull()
    {
        // Act
        Action act = () => new DeleteTableCommandHandler(null!, _getByTableName, _deleteStagingTable, _deleteDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("uow");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetByTableNameIsNull()
    {
        // Act
        Action act = () => new DeleteTableCommandHandler(_uow, null!, _deleteStagingTable, _deleteDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getByTableName");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_DeleteStagingTableIsNull()
    {
        // Act
        Action act = () => new DeleteTableCommandHandler(_uow, _getByTableName, null!, _deleteDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("deleteStagingTable");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_DeleteDataSetIsNull()
    {
        // Act
        Action act = () => new DeleteTableCommandHandler(_uow, _getByTableName, _deleteStagingTable, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("deleteDataSet");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_TableDoesNotExist()
    {
        // Arrange
        var cmd = new DeleteTableCommand("tbl");
        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(null));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_AllDeletesSucceed()
    {
        // Arrange
        var cmd = new DeleteTableCommand("tbl");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _uow.BeginTransaction().Returns(tx);
        _deleteStagingTable.ExecuteAsync(cmd.TableName, tx, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _deleteDataSet.ExecuteAsync(ds, tx, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _uow.Received(1).CommitTransaction(tx);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_When_DeleteThrows()
    {
        // Arrange
        var cmd = new DeleteTableCommand("tbl");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _uow.BeginTransaction().Returns(tx);
        _deleteStagingTable.ExecuteAsync(cmd.TableName, tx, Arg.Any<CancellationToken>()).Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _uow.Received(1).RollbackTransaction(tx);
    }
}
