namespace CommandLineLibrary.Tests.Unit
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class MethodCallValidatorProviderTests
    {
        private MethodCallValidatorProvider systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new MethodCallValidatorProvider();
        }

        [Test]
        public void ValidateMethodCallExpressionAndReturnMethodInfo_WhenMethodsDontMatch_Throws()
        {
            Assert.That(
                () =>
                    {
                        systemUnderTest.ValidateMethodCallExpressionAndReturnMethodInfo<string, string>(
                            x => x.Clone(),
                            y => y.Contains(null));
                    },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    "Method selected in expression does not have a compatible signature. Expected signature of: Boolean Contains(String) but received Object Clone()"));
        }

        [Test]
        public void ValidateMethodCallExpressionAndReturnMethodInfo_WhenMethodsMatch_ReturnsMethodInfo()
        {
            var expected = typeof(string).GetMethods()
                .First(x => x.Name == nameof(string.Contains) && x.GetParameters().Length == 1);

            var actual = systemUnderTest.ValidateMethodCallExpressionAndReturnMethodInfo<string, string>(
                x => x.Contains(null),
                y => y.Equals(null));

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ValidateMethodCallExpressionAndReturnMethodInfo_WhenNullControlExpression_Throws()
        {
            Assert.That(
                () =>
                    {
                        systemUnderTest.ValidateMethodCallExpressionAndReturnMethodInfo<string, string>(
                            x => x.Contains(null),
                            null);
                    },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Value cannot be null.{Environment.NewLine}Parameter name: knownCompatibleExpression"));
        }

        [Test]
        public void ValidateMethodCallExpressionAndReturnMethodInfo_WhenNullValidateExpression_Throws()
        {
            Assert.That(
                () =>
                    {
                        systemUnderTest.ValidateMethodCallExpressionAndReturnMethodInfo<string, string>(
                            null,
                            y => y.Contains(null));
                    },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Value cannot be null.{Environment.NewLine}Parameter name: expressionToValidate"));
        }
    }
}