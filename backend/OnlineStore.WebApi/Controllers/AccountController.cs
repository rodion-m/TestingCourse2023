using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Services;
using OnlineStore.Models.Requests;
using OnlineStore.Models.Responses;
using OnlineStore.WebApi.Extensions;

namespace OnlineStore.WebApi.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly ITokenService _tokenService;

    public AccountController(AccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var account = await _accountService.Register(request.Name, request.Email, request.Password, cancellationToken);
        var token = _tokenService.GenerateToken(account);
        return new RegisterResponse(account.Id, account.Name, account.Email, token);
    }

    [HttpPost("authentication")]
    public async Task<ActionResult<AuthResponse>> Authentication(AuthRequest request, CancellationToken cancellationToken)
    {
        var (account, token) = await _accountService.Authentication(request.Email, request.Password, cancellationToken);
        return new AuthResponse(account.Id, account.Name, account.Email, token);
    }

    [Authorize]
    [HttpGet("get_current")]
    public async Task<ActionResult<Account>> GetCurrentAccount(CancellationToken cancellationToken) =>
        await _accountService.GetAccount(User.GetAccountId(), cancellationToken);

    [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
    [HttpGet("get_all")]
    public async Task<IReadOnlyCollection<Account>> GetAllAccounts(CancellationToken cancellationToken) =>
        await _accountService.GetAll(cancellationToken);
}