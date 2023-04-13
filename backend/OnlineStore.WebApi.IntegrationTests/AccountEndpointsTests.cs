using Bogus;
using FluentAssertions;
using OnlineStore.HttpApiClient;
using OnlineStore.Models.Requests;
using OnlineStore.Models.Responses;
using static FluentAssertions.FluentActions;

namespace OnlineStore.WebApi.IntegrationTests;

public class AccountEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Faker _faker = new("ru");

    public AccountEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    /// <summary>
    /// Тест регистрации пользователя в случае если пользователь с таким email уже существует
    /// </summary>
    [Fact]
    public async void Register_user_succeeded()
    {
        HttpClient httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email, 
            Name = _faker.Person.FullName, 
            Password = _faker.Internet.Password()
        };
        RegisterResponse registerResponse = await client.Register(registerRequest);
        registerResponse.Email.Should().Be(registerRequest.Email);
        registerResponse.AccountId.Should().NotBeEmpty();
    }

    /// <summary>
    /// Тест регистрации пользователя в случае если пользователь с таким email уже существует
    /// </summary>
    [Fact]
    public async void Register_user_with_occupied_email_gives_error()
    {
        // Arrange
        HttpClient httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email, 
            Name = _faker.Person.FullName, 
            Password = _faker.Internet.Password()
        };
        // Pre-Act (Первая попытка регистрации пользователя)
        await client.Register(registerRequest);
        
        // Act
        await Invoking(() => client.Register(registerRequest))
            .Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage("*Email already exists*");
    }
    
    
    //Create login test
    [Fact]
    public async void Login_user_succeeded()
    {
        // Arrange
        HttpClient httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email, 
            Name = _faker.Person.FullName, 
            Password = _faker.Internet.Password()
        };
        // Pre-Act
        await client.Register(registerRequest);
        
        // Act
        var loginRequest = new AuthRequest()
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };
        var loginResponse = await client.Authentication(loginRequest);
        
        // Assert
        loginResponse.Email.Should().Be(registerRequest.Email);
        loginResponse.Token.Should().NotBeEmpty();
    }
}
