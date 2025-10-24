using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using ETL.Application.DataSet.RenameColumn;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class RenameColumnCommandHandlerTests
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IStagingColumnExists _columnExists;
    private readonly IRenameStagingColumn _renameStagingColumn;
    private readonly RenameColumnCommandHandler _sut;

    public RenameColumnCommandHandlerTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _getByTableName = Substitute.For<IGetDataSetByTableName>();
        _columnExists = Substitute.For<IStagingColumnExists>();
        _renameStagingColumn = Substitute.For<IRenameStagingColumn>();

        _sut = new RenameColumnCommandHandler(_uow, _getByTableName, _columnExists, _renameStagingColumn);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UowIsNull()
    {
        // Act
        Action act = () => new RenameColumnCommandHandler(null!, _getByTableName, _columnExists, _renameStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("uow");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_GetByTableNameIsNull()
    {
        // Act
        Action act = () => new RenameColumnCommandHandler(_uow, null!, _columnExists, _renameStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getByTableName");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_ColumnExistsIsNull()
    {
        // Act
        Action act = () => new RenameColumnCommandHandler(_uow, _getByTableName, null!, _renameStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("columnExists");
    }

    [Fact]
    public void Constructor_ShouldThrow_When_RenameStagingColumnIsNull()
    {
        // Act
        Action act = () => new RenameColumnCommandHandler(_uow, _getByTableName, _columnExists, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("renameStagingColumn");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_TableDoesNotExist()
    {
        // Arrange
        var cmd = new RenameColumnCommand("tbl", "Old", "New");
        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ColumnRename.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_OldColumnDoesNotExist()
    {
        // Arrange
        var cmd = new RenameColumnCommand("tbl", "Old", "New");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.OldColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_NewColumnAlreadyExists()
    {
        // Arrange
        var cmd = new RenameColumnCommand("tbl", "Old", "New");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.OldColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.NewColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_ShouldRenameColumn_When_AllChecksPass()
    {
        // Arrange
        var cmd = new RenameColumnCommand("tbl", "Old", "New");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.OldColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.NewColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        _uow.BeginTransaction().Returns(tx);
        _renameStagingColumn.ExecuteAsync(cmd.TableName, cmd.OldColumnName, cmd.NewColumnName, tx, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _renameStagingColumn.Received(1).ExecuteAsync(cmd.TableName, cmd.OldColumnName, cmd.NewColumnName, tx, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_When_RenameThrows()
    {
        // Arrange
        var cmd = new RenameColumnCommand("tbl", "Old", "New");
        var ds = new DataSetMetadata(cmd.TableName, "u1");
        var tx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(cmd.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.OldColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _columnExists.ExecuteAsync(cmd.TableName, cmd.NewColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        _uow.BeginTransaction().Returns(tx);
        _renameStagingColumn.ExecuteAsync(cmd.TableName, cmd.OldColumnName, cmd.NewColumnName, tx, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("DB failed"));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _uow.Received(1).RollbackTransaction(tx);
    }
}
