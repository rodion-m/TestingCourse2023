using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Services;

public class AccountService
{
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IPasswordHasherService passwordHasherService,
        ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _passwordHasherService =
            passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public virtual async Task<Account> Register(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(email);
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

        var existedAccount = await _unitOfWork.AccountRepository.FindByEmail(email, cancellationToken);
        var emailRegistered = existedAccount is not null;
        if (emailRegistered)
        {
            throw new EmailExistsException("Email already exists");
        }

        var hashedPassword = _passwordHasherService.HashPassword(password);
        var account = new Account(Guid.NewGuid(), name, email, hashedPassword);
        var cart = new Cart(Guid.NewGuid(), account.Id, new List<CartItem>());
        await _unitOfWork.AccountRepository.Add(account, cancellationToken);
        await _unitOfWork.CartRepository.Add(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return account;
    }

    public virtual async Task<(Account account, string token)> Authentication(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        var account = await _unitOfWork.AccountRepository.FindByEmail(email, cancellationToken);
        if (account == null)
        {
            throw new EmailNotFoundException("There is no such account");
        }

        if (!_passwordHasherService.VerifyPassword(account.PasswordHash, password))
        {
            throw new InvalidPasswordException("Invalid password");
        }

        var token = _tokenService.GenerateToken(account);
        return (account, token);
    }

    public Task<Account> GetAccount(Guid accountId, CancellationToken cancellationToken) =>
        _unitOfWork.AccountRepository.GetById(accountId, cancellationToken);

    public Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken) =>
        _unitOfWork.AccountRepository.GetAll(cancellationToken);
}