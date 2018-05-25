namespace CommandLineProcessorLib
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using CommandLineProcessorContracts;

    public class MethodCallValidatorProvider : IMethodCallValidatorService
    {
        public MethodInfo ValidateMethodCallExpressionAndReturnMethodInfo<TValidate, TExpected>(
            Expression<Action<TValidate>> expressionToValidate,
            Expression<Action<TExpected>> knownCompatibleExpression)
        {
            if (expressionToValidate == null)
            {
                throw new ArgumentNullException(nameof(expressionToValidate));
            }

            if (knownCompatibleExpression == null)
            {
                throw new ArgumentNullException(nameof(knownCompatibleExpression));
            }

            if (!(expressionToValidate.Body is MethodCallExpression methodExpressionToValidate))
            {
                throw new ArgumentException(
                    $"Expression must be a {nameof(MethodCallExpression)}",
                    nameof(expressionToValidate));
            }

            if (!(knownCompatibleExpression.Body is MethodCallExpression knownGoodMethodExpression))
            {
                throw new ArgumentException(
                    $"Expression must be a {nameof(MethodCallExpression)}",
                    nameof(knownCompatibleExpression));
            }

            bool signatureMatches = true;
            if (methodExpressionToValidate.Arguments.Count == knownGoodMethodExpression.Arguments.Count)
            {
                for (int i = 0; i < knownGoodMethodExpression.Arguments.Count; i++)
                {
                    if (!methodExpressionToValidate.Arguments[i].Type
                            .IsAssignableFrom(knownGoodMethodExpression.Arguments[i].Type))
                    {
                        signatureMatches = false;
                        break;
                    }
                }
            }
            else
            {
                signatureMatches = false;
            }

            if (signatureMatches)
            {
                signatureMatches =
                    methodExpressionToValidate.Method.ReturnType.IsAssignableFrom(
                        knownGoodMethodExpression.Method.ReturnType)
                    || (methodExpressionToValidate.Method.ReturnType == typeof(void)
                        && knownGoodMethodExpression.Method.ReturnType == typeof(void));
            }

            if (signatureMatches)
            {
                return methodExpressionToValidate.Method;
            }

            throw new ArgumentException(
                $"Method selected in expression does not have a compatible signature. Expected signature of: {GenerateMethodSignatureText(knownGoodMethodExpression)} but received {GenerateMethodSignatureText(methodExpressionToValidate)}");
        }

        private string GenerateMethodSignatureText(MethodCallExpression methodExpression)
        {
            var builder = new StringBuilder();
            builder.Append($"{methodExpression.Method.ReturnType.Name} ");
            builder.Append($"{methodExpression.Method.Name}(");
            builder.Append(string.Join(", ", methodExpression.Arguments.Select(x => x.Type.Name)));
            builder.Append(")");
            return builder.ToString();
        }
    }
}