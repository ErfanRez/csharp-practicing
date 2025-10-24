using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using ETL.Application.DataSet.RenameTable;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class RenameTableCommandHandlerTests
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IRenameStagingTable _renameStagingTable;
    private readonly IUpdateDataSet _updateDataSet;
    private readonly RenameTableCommandHandler _sut;

    public RenameTableCommandHandlerTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _getByTableName = Substitute.For<IGetDataSetByTableName>();
        _renameStagingTable = Substitute.For<IRenameStagingTable>();
        _updateDataSet = Substitute.For<IUpdateDataSet>();

        _sut = new RenameTableCommandHandler(_uow, _getByTableName, _renameStagingTable, _updateDataSet);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UowIsNull()
    {
        // Act
        Action act = () => new RenameTableCommandHandler(null!, _getByTableName, _renameStagingTable, _updateDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("uow");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetByTableNameIsNull()
    {
        // Act
        Action act = () => new RenameTableCommandHandler(_uow, null!, _renameStagingTable, _updateDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getByTableName");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RenameStagingTableIsNull()
    {
        // Act
        Action act = () => new RenameTableCommandHandler(_uow, _getByTableName, null!, _updateDataSet);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("renameStagingTable");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UpdateDataSetIsNull()
    {
        // Act
        Action act = () => new RenameTableCommandHandler(_uow, _getByTableName, _renameStagingTable, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("updateDataSet");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_OldTableDoesNotExist()
    {
        // Arrange
        var cmd = new RenameTableCommand("old_table", "new_table");
        _getByTableName.ExecuteAsync(cmd.OldTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_NewTableAlreadyExists()
    {
        // Arrange
        var cmd = new RenameTableCommand("old_table", "new_table");
        var existing = new DataSetMetadata(cmd.OldTableName, "u1");
        var conflict = new DataSetMetadata(cmd.NewTableName, "u1");

        _getByTableName.ExecuteAsync(cmd.OldTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(existing));
        _getByTableName.ExecuteAsync(cmd.NewTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(conflict));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_When_AllChecksPass()
    {
        // Arrange
        var cmd = new RenameTableCommand("old_table", "new_table");
        var existing = new DataSetMetadata(cmd.OldTableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.OldTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(existing));
        _getByTableName.ExecuteAsync(cmd.NewTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        _uow.BeginTransaction().Returns(tx);

        _renameStagingTable.ExecuteAsync(cmd.OldTableName, cmd.NewTableName, tx, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _updateDataSet.ExecuteAsync(existing, tx, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _uow.Received(1).CommitTransaction(tx);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_When_RenameThrows()
    {
        // Arrange
        var cmd = new RenameTableCommand("old_table", "new_table");
        var existing = new DataSetMetadata(cmd.OldTableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.OldTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(existing));
        _getByTableName.ExecuteAsync(cmd.NewTableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        _uow.BeginTransaction().Returns(tx);

        _renameStagingTable.ExecuteAsync(cmd.OldTableName, cmd.NewTableName, tx, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("DB failed"));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _uow.Received(1).RollbackTransaction(tx);
    }
}
