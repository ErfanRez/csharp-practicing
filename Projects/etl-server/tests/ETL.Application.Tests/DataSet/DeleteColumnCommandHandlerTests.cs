using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.DataSet.DeleteColumn;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class DeleteColumnCommandHandlerTests
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IStagingColumnExists _columnExists;
    private readonly IDeleteStagingColumn _deleteStagingColumn;
    private readonly DeleteColumnCommandHandler _sut;

    public DeleteColumnCommandHandlerTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _getByTableName = Substitute.For<IGetDataSetByTableName>();
        _columnExists = Substitute.For<IStagingColumnExists>();
        _deleteStagingColumn = Substitute.For<IDeleteStagingColumn>();

        _sut = new DeleteColumnCommandHandler(_uow, _getByTableName, _columnExists, _deleteStagingColumn);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
    {
        // Arrange // Act
        Action act = () => new DeleteColumnCommandHandler(null!, _getByTableName, _columnExists, _deleteStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("uow");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGetByTableNameIsNull()
    {
        // Arrange // Act
        Action act = () => new DeleteColumnCommandHandler(_uow, null!, _columnExists, _deleteStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("getByTableName");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenColumnExistsIsNull()
    {
        // Arrange // Act
        Action act = () => new DeleteColumnCommandHandler(_uow, _getByTableName, null!, _deleteStagingColumn);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("columnExists");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDeleteStagingColumnIsNull()
    {
        // Arrange // Act
        Action act = () => new DeleteColumnCommandHandler(_uow, _getByTableName, _columnExists, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("deleteStagingColumn");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTableDoesNotExist()
    {
        // Arrange
        var command = new DeleteColumnCommand("missing_table", "col");
        _getByTableName.ExecuteAsync(command.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(null));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ColumnDelete.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenColumnDoesNotExist()
    {
        // Arrange
        var command = new DeleteColumnCommand("tbl", "missing_col");
        var ds = new DataSetMetadata(command.TableName, "owner");
        _getByTableName.ExecuteAsync(command.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(command.TableName, command.ColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ColumnDelete.Failed");
    }

    [Fact]
    public async Task Handle_ShouldDeleteColumn_WhenTableAndColumnExist()
    {
        // Arrange
        var command = new DeleteColumnCommand("tbl", "col");
        var ds = new DataSetMetadata(command.TableName, "owner");
        var fakeTx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(command.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(command.TableName, command.ColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _uow.BeginTransaction().Returns(fakeTx);
        _deleteStagingColumn.ExecuteAsync(command.TableName, command.ColumnName, fakeTx, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _deleteStagingColumn.Received(1).ExecuteAsync(command.TableName, command.ColumnName, fakeTx, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_WhenDeleteThrows()
    {
        // Arrange
        var command = new DeleteColumnCommand("tbl", "failing_col");
        var ds = new DataSetMetadata(command.TableName, "owner");
        var fakeTx = Substitute.For<IDbTransaction>();

        _getByTableName.ExecuteAsync(command.TableName, Arg.Any<CancellationToken>()).Returns(Task.FromResult<DataSetMetadata?>(ds));
        _columnExists.ExecuteAsync(command.TableName, command.ColumnName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _uow.BeginTransaction().Returns(fakeTx);

        _deleteStagingColumn
            .ExecuteAsync(command.TableName, command.ColumnName, fakeTx, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _uow.Received(1).RollbackTransaction(Arg.Any<IDbTransaction>());
    }
}
