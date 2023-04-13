using FluentAssertions;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Test;

public class CartTests
{
    [Fact]
    private void New_item_is_added_to_cart()
    {
        var result = 1;
        result.Should().Be(1);
        var cart = new Cart(Guid.Empty, Guid.Empty, new List<CartItem>());
        var product = new Product(Guid.Empty, "fake", 50, "img", "desc", Guid.Empty);
        var quantity = 1;
        var awaiter = Task.Delay(1000).GetAwaiter();

        cart.Add(product, 1);
        var cartItem = cart.Items.First();

        Assert.NotNull(cartItem);
        Assert.Single(cart.Items);
        Assert.Equal(product.Id, cartItem.ProductId);
        Assert.Equal(quantity, cartItem.Quantity);
        Assert.Equal(product.Price, cartItem.Price);
    }

    [Fact]
    private void Adding_existed_product_in_cart_changes_item_quantity()
    {
        // Arrange
        var cart = new Cart(Guid.Empty, Guid.Empty, new List<CartItem>());
        var product = new Product(Guid.Empty, "fake", 50, "img", "desc", Guid.Empty);
        var quantity = 2;

        // Act
        cart.Add(product, 1);
        cart.Add(product, 1);
        
        // Assert
        Assert.Single(cart.Items);
        var cartItem = cart.Items.First();
        Assert.Equal(quantity, cartItem.Quantity);
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(1000)]
    public void Quantity_of_item_added_to_cart_n_times_is_n(double n)
    {
        var cart = new Cart(Guid.Empty, Guid.Empty, new List<CartItem>());
        var product = new Product(Guid.Empty, "fake", 50, "img", "desc", Guid.Empty);
        
        for (int i = 0; i < n; i++)
        {
            cart.Add(product, 1);
        }

        var cartItem = cart.Items.First();
        Assert.Single(cart.Items);
        Assert.Equal(n, cartItem.Quantity);
    }
    
    [Fact]
    private void Adding_empty_product_is_impossible()
    {
        var cart = new Cart(Guid.Empty, Guid.Empty, new List<CartItem>());
        Assert.Throws<ArgumentNullException>(() => cart.Add(null, 1));
    }
    
}