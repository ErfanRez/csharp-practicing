using ETL.Application.Abstractions.Security;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.Auth;

public record LoginCallbackCommand(string Code, string? RedirectPath) : IRequest<Result<TokenResponse>>;

public sealed class CallbackCommandHandler : IRequestHandler<LoginCallbackCommand, Result<TokenResponse>>
{
    private readonly IAuthCodeForTokenExchanger _exchanger;

    public CallbackCommandHandler(IAuthCodeForTokenExchanger exchanger)
    {
        _exchanger = exchanger ?? throw new ArgumentNullException(nameof(exchanger));
    }

    public async Task<Result<TokenResponse>> Handle(LoginCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tokenResponse = await _exchanger.ExchangeCodeForTokensAsync(request.Code, request.RedirectPath ?? string.Empty, cancellationToken);

            return Result.Success(tokenResponse);
        }
        catch (Exception ex)
        {
            return Result.Failure<TokenResponse>(Error.Problem("Token.Exchange.Failed", ex.Message));
        }
    }
}
