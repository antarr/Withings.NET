using System;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class WithingsApiExceptionTests
    {
        [Test]
        public void Constructor_WithStatusCode_SetsStatusCodeAndDefaultMessage()
        {
            var exception = new WithingsApiException(401);

            exception.StatusCode.Should().Be(401);
            exception.Message.Should().Be("Withings API Error: 401");
        }

        [Test]
        public void Constructor_WithStatusCodeAndMessage_SetsBoth()
        {
            var exception = new WithingsApiException(503, "Service unavailable");

            exception.StatusCode.Should().Be(503);
            exception.Message.Should().Be("Service unavailable");
        }

        [Test]
        public void Constructor_WithInnerException_PreservesInnerException()
        {
            var inner = new InvalidOperationException("inner");
            var exception = new WithingsApiException(500, "Server error", inner);

            exception.StatusCode.Should().Be(500);
            exception.Message.Should().Be("Server error");
            exception.InnerException.Should().BeSameAs(inner);
        }

        [Test]
        public void IsException_DerivedFromSystemException()
        {
            var exception = new WithingsApiException(400);

            exception.Should().BeAssignableTo<Exception>();
        }
    }
}
