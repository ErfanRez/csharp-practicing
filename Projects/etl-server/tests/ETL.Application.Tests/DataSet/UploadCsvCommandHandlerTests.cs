using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.DataSet.UploadFile;
using ETL.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace ETL.Application.Tests.DataSet;

public class UploadCsvCommandHandlerTests
{
    private readonly IUnitOfWork _uow;
    private readonly ICreateTableFromCsv _createTableOp;
    private readonly IAddDataSet _addDataSetOp;
    private readonly IGetDataSetByTableName _getByTableNameOp;
    private readonly UploadCsvCommandHandler _sut;

    public UploadCsvCommandHandlerTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _createTableOp = Substitute.For<ICreateTableFromCsv>();
        _addDataSetOp = Substitute.For<IAddDataSet>();
        _getByTableNameOp = Substitute.For<IGetDataSetByTableName>();

        _sut = new UploadCsvCommandHandler(_uow, _createTableOp, _addDataSetOp, _getByTableNameOp);
    }

    [Fact]
    public void Constructor_ShouldThrow_When_UowIsNull()
    {
        // Act
        Action act = () => new UploadCsvCommandHandler(null!, _createTableOp, _addDataSetOp, _getByTableNameOp);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("uow");
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_When_TableAlreadyExists()
    {
        // Arrange
        var cmd = new UploadCsvCommand("t", Stream.Null, "u");
        _getByTableNameOp.ExecuteAsync("t", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new DataSetMetadata("t", "u") as DataSetMetadata));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("FileUpload.Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessAndCommit_When_AllOperationsSucceed()
    {
        // Arrange
        var cmd = new UploadCsvCommand("t", Stream.Null, "u");
        _getByTableNameOp.ExecuteAsync("t", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        var fakeTx = Substitute.For<IDbTransaction>();
        _uow.BeginTransaction().Returns(fakeTx);

        _createTableOp.ExecuteAsync(cmd.TableName, cmd.FileStream, fakeTx, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _addDataSetOp.ExecuteAsync(Arg.Any<DataSetMetadata>(), fakeTx, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _uow.Received(1).CommitTransaction(fakeTx);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndRollback_When_CreateTableThrows()
    {
        // Arrange
        var cmd = new UploadCsvCommand("t", Stream.Null, "u");
        _getByTableNameOp.ExecuteAsync("t", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<DataSetMetadata?>(null));

        var fakeTx = Substitute.For<IDbTransaction>();
        _uow.BeginTransaction().Returns(fakeTx);

        _createTableOp.ExecuteAsync(cmd.TableName, cmd.FileStream, fakeTx, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("boom"));

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _uow.Received(1).RollbackTransaction(fakeTx);
    }
}
