using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class ExpandoObjectConverterTests
    {
        private JsonSerializerOptions _options;

        [SetUp]
        public void Setup()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new ExpandoObjectConverter());
        }

        [Test]
        public void Read_ValidJson_ReturnsExpandoObject()
        {
            // Arrange
            var json = "{\"name\": \"John\", \"age\": 30, \"isDeveloper\": true}";

            // Act
            var result = JsonSerializer.Deserialize<ExpandoObject>(json, _options);

            // Assert
            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict["name"].Should().Be("John");
            dict["age"].Should().Be(30);
            dict["isDeveloper"].Should().Be(true);
        }

        [Test]
        public void Read_NonObjectJson_ThrowsJsonException()
        {
            // Arrange
            var json = "[1, 2, 3]";

            // Act
            Action act = () => JsonSerializer.Deserialize<ExpandoObject>(json, _options);

            // Assert
            act.Should().Throw<JsonException>();
        }

        [Test]
        public void Read_UnexpectedTokenInObject_ThrowsJsonException()
        {
            // Line 24: if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
            // This is inside a 'while (reader.Read())' loop that starts after a '{'.
            // To hit this, we need the next token to be something other than EndObject or PropertyName.
            // In a valid JSON object, after '{' or after a 'value,', the next token MUST be PropertyName or '}'.
            // If it's something else (like '[' or '{' without a property name), it's invalid JSON.

            // Example: "{[" is invalid. System.Text.Json might catch this before calling the converter
            // if it validates the structure during Read().

            var json = "{\"prop\": \"val\", [\"invalid\"]}";

            Action act = () => JsonSerializer.Deserialize<ExpandoObject>(json, _options);
            act.Should().Throw<JsonException>();
        }

        [Test]
        public void Read_IncompleteObject_ThrowsJsonException()
        {
            // This targets the throw at the end of the while loop (line 30):
            // throw new JsonException();
            // which is reached if reader.Read() returns false before reaching EndObject.
            var json = "{\"prop\": \"val\""; // Missing closing brace

            Action act = () => JsonSerializer.Deserialize<ExpandoObject>(json, _options);
            act.Should().Throw<JsonException>();
        }

        [Test]
        public void Read_NestedObject_ReturnsNestedExpandoObject()
        {
            var json = "{\"outer\": {\"inner\": \"value\"}}";

            var result = JsonSerializer.Deserialize<ExpandoObject>(json, _options);

            var dict = (IDictionary<string, object>)result;
            var nested = (IDictionary<string, object>)(ExpandoObject)dict["outer"];
            nested["inner"].Should().Be("value");
        }

        [Test]
        public void Read_ArrayValue_ReturnsList()
        {
            var json = "{\"items\": [1, 2, 3]}";

            var result = JsonSerializer.Deserialize<ExpandoObject>(json, _options);

            var dict = (IDictionary<string, object>)result;
            var items = (List<object>)dict["items"];
            items.Should().HaveCount(3);
            items[0].Should().Be((long)1);
        }

        [Test]
        public void Read_NullValue_ReturnsNull()
        {
            var json = "{\"key\": null}";

            var result = JsonSerializer.Deserialize<ExpandoObject>(json, _options);

            var dict = (IDictionary<string, object>)result;
            dict["key"].Should().BeNull();
        }

        [Test]
        public void Write_ExpandoObject_ProducesValidJson()
        {
            var expando = new ExpandoObject();
            var dict = (IDictionary<string, object>)expando;
            dict["name"] = "John";
            dict["age"] = 30;

            var json = JsonSerializer.Serialize(expando, _options);

            json.Should().Contain("\"name\":\"John\"");
            json.Should().Contain("\"age\":30");
        }
    }
}
