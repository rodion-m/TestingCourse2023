using Bogus;
using FluentAssertions;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Services;
using OnlineStore.Domain.Test.FakeRepositories;

namespace OnlineStore.Domain.Test;

public class AccountServiceTests
{
    private readonly Faker _faker = new Faker();
    
    [Fact]
    private async void Register_new_user_succeeded()
    {
        // Arrange
        var uow = new UnitOfWorkInMemory();
        var accountService = new AccountService(
            new Fakes.PasswordHasherFake(), new Fakes.TokenServiceFake(), uow);

        //Генерация токена лишняя ответственность, теперь, когда мы пишем тесты,
        // это создает проблемы, поэтому мы ее убираем
        // Act
        var account = await accountService.Register(
            _faker.Person.FullName, _faker.Person.Email, _faker.Internet.Password(), 
            default);
        
        // Assert
        // 1. Проверить, что в БД появился новый аккаунт
        var accountFromDb = await uow.AccountRepository.GetById(account.Id);
        accountFromDb.Should().BeEquivalentTo(account);
        
        // 2. Проверить, что в БД появилась корзина
        var cartFromDb = await uow.CartRepository.GetByAccountId(account.Id);
        cartFromDb.Should().NotBeNull();


        // Summary:
        // От теста мало толку, т.к. он проверяет только то, что новый пользователь сохранился в БД.
        // И не проверяет корректность генерации пароля, например.
    }
    
    //В LogIn
    // Плохой тест, т.к. только делает вид, что проверяет регистрацию пользователя,
    // но на самом деле он проверяет только то, что новый пользователь сохранился в БД.
    
    private static class Fakes
    {
        public class PasswordHasherFake : IPasswordHasherService
        {
            public string HashPassword(string password) => password;

            public bool VerifyPassword(string passwordHash, string password)
                => passwordHash == password;
        }

        public class TokenServiceFake : ITokenService
        {
            public string GenerateToken(Account account) => "fake token";
        }
    }
}

