using FluentValidation;

namespace WCloud.Framework.Common.Validator.FluentValidatorImpl
{
    /// <summary>
    /// https://github.com/JeremySkinner/FluentValidation/issues
    /// </summary>
    public interface IEntityFluentValidator<T> : IValidator<T>, IValidator { }

    public abstract class ModelValidator<T> : AbstractValidator<T>, IEntityFluentValidator<T> { }
}
