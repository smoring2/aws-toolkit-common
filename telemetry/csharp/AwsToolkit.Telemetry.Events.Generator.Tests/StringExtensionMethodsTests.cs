using Amazon.AwsToolkit.Telemetry.Events.Generator.Utils;
using Xunit;

namespace Amazon.AwsToolkit.Telemetry.Events.Generator.Tests
{
    public class StringExtensionMethodsTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("a", "A")]
        [InlineData("A", "A")]
        [InlineData("ab", "Ab")]
        [InlineData("Ab", "Ab")]
        [InlineData("AB", "AB")]
        [InlineData("pascalCase", "PascalCase")]
        [InlineData("underscore_pascalCase", "UnderscorePascalCase")]
        public void ToPascalCase(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToPascalCase());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("ValidIdentifier", "ValidIdentifier")]
        [InlineData("has spaces", "hasspaces")]
        [InlineData("has<angle>brackets", "hasanglebrackets")]
        [InlineData("has;semicolon", "hassemicolon")]
        [InlineData("has{braces}", "hasbraces")]
        [InlineData("has(parens)", "hasparens")]
        [InlineData("has\"quotes\"", "hasquotes")]
        [InlineData("dotnet2.1", "dotnet21")]
        [InlineData("with-dash", "withdash")]
        [InlineData("123StartWithDigit", "_123StartWithDigit")]
        [InlineData("normal_underscore", "normal_underscore")]
        [InlineData("inject;code{here}", "injectcodehere")]
        [InlineData("<T>Generic", "TGeneric")]
        public void ToSafeIdentifier(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToSafeIdentifier());
        }
    }
}
