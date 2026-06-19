using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class OAuthTokenTests
    {
        [Test]
        public void Deserialize_ValidJson_ReturnsOAuthToken()
        {
            // Arrange
            var json = @"{
                ""access_token"": ""access_123"",
                ""refresh_token"": ""refresh_456"",
                ""expires_in"": 3600,
                ""scope"": ""user.metrics"",
                ""token_type"": ""Bearer"",
                ""userid"": ""789""
            }";

            // Act
            var result = JsonSerializer.Deserialize<OAuthToken>(json);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("access_123");
            result.RefreshToken.Should().Be("refresh_456");
            result.ExpiresIn.Should().Be(3600);
            result.Scope.Should().Be("user.metrics");
            result.TokenType.Should().Be("Bearer");
            result.UserId.Should().Be("789");
        }

        [Test]
        public void Deserialize_NumericUserId_ReturnsUserIdAsString()
        {
            // Arrange
            var json = @"{
                ""userid"": 12345
            }";

            // Act
            var result = JsonSerializer.Deserialize<OAuthToken>(json);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("12345");
        }

        [Test]
        public void Deserialize_StringUserId_ReturnsUserIdAsString()
        {
            // Arrange
            var json = @"{
                ""userid"": ""67890""
            }";

            // Act
            var result = JsonSerializer.Deserialize<OAuthToken>(json);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("67890");
        }

        [Test]
        public void Serialize_OAuthToken_ProducesValidJson()
        {
            // Arrange
            var token = new OAuthToken
            {
                AccessToken = "access_123",
                RefreshToken = "refresh_456",
                ExpiresIn = 3600,
                Scope = "user.metrics",
                TokenType = "Bearer",
                UserId = "789"
            };

            // Act
            var json = JsonSerializer.Serialize(token);

            // Assert
            var doc = JsonDocument.Parse(json);
            doc.RootElement.GetProperty("access_token").GetString().Should().Be("access_123");
            doc.RootElement.GetProperty("refresh_token").GetString().Should().Be("refresh_456");
            doc.RootElement.GetProperty("expires_in").GetInt32().Should().Be(3600);
            doc.RootElement.GetProperty("scope").GetString().Should().Be("user.metrics");
            doc.RootElement.GetProperty("token_type").GetString().Should().Be("Bearer");
            doc.RootElement.GetProperty("userid").GetString().Should().Be("789");
        }
    }
}
