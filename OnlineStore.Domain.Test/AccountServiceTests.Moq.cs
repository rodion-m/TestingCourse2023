using Bogus;
using Moq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;
using OnlineStore.Domain.Services;

namespace OnlineStore.Domain.Test;

public class AccountServiceTestsMoq
{
    private readonly Faker _faker = new Faker();
    
    [Fact]
    private async void Register_new_user_succeeded()
    {
        // Arrange: Create Mocks
        var cartRepositoryMock = new Mock<ICartRepository>(); //mock
        var accountRepositoryMock = new Mock<IAccountRepository>(); //mock
        var uowMock = new Mock<IUnitOfWork>(); //mock + stub
        var passwordHasherMock = new Mock<IPasswordHasherService>(); //stub + mock
        var tokenServiceStub = new Mock<ITokenService>(); //stub
        
        // Arrange: Setup Mocks
        // Настраиваем, чтобы UoW возвращал наши замоканные репозитории
        uowMock.Setup(u => u.AccountRepository)
            .Returns(accountRepositoryMock.Object);
        uowMock.Setup(u => u.CartRepository)
            .Returns(cartRepositoryMock.Object);
        
        // Настраиваем, чтобы метод HashPassword просто возвращал то, что ему передали
        passwordHasherMock
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns<string>(x => x);

        var accountService = new AccountService(
            passwordHasherMock.Object, tokenServiceStub.Object, uowMock.Object);
        
        var password = _faker.Internet.Password();
        
        // Act
        var account = await accountService.Register(
             _faker.Person.FullName, _faker.Person.Email, password, 
             default);

        // Assert
        // 1. Проверить, что в БД появился новый аккаунт
        // (метод AccountRepository.Add был вызван один раз и ему передали именно тот аккаунт, что вернул метод Register
        accountRepositoryMock.Verify(
            accRepo => accRepo.Add(account, default), 
            Times.Once);
        // 2. Проверить, что в БД появилась корзина
        // (метод CartRepository.Add был вызван один раз и ему передали корзину с Id аккаунта, который вернул метод Register)
        cartRepositoryMock.Verify(
            cartRepo => cartRepo.Add(It.Is<Cart>(c => c.AccountId == account.Id), default), 
            Times.Once);
        
        // 3. Проверить UnitOfWork (метод SaveChangesAsync был вызван)
        uowMock.Verify(uow => uow.SaveChangesAsync(default), Times.AtLeastOnce);
        
        //4. Проверить, что пароль был захеширован
        passwordHasherMock.Verify(x => x.HashPassword(password), Times.Once);
    }
    
    //В LogIn
    // Плохой тест, т.к. только делает вид, что проверяет регистрацию пользователя,
    // но на самом деле он проверяет только то, что новый пользователь сохранился в БД.
}

