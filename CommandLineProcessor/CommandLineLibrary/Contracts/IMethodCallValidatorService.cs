namespace CommandLineLibrary.Contracts
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IMethodCallValidatorService
    {
        MethodInfo ValidateMethodCallExpressionAndReturnMethodInfo<TValidate, TExpected>(
            Expression<Action<TValidate>> expressionToValidate,
            Expression<Action<TExpected>> knownCompatibleExpression);
    }
}