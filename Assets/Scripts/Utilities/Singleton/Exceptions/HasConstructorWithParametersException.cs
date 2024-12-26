using System;

namespace Utilities.Singleton.Exceptions {
    public sealed class HasConstructorWithParametersException : ConstructorException {
        
        /// <summary>
        /// Placeholder for string to be formatted for exception message.
        /// </summary>
        private const string HasCtorWithParameters = "{0} has constructor(s) with parametersnstructor(s).";

        /// <summary>
        /// This exception is thrown when a class which inherits <see cref="SingletonBase{T}" /> has constructors with parameters.
        /// </summary>
        /// <param name="type">Type which implements <see cref="SingletonBase{T}" /> and has constructor(s) with parameters.</param>
        public HasConstructorWithParametersException(Type type) : base(type, string.Format(HasCtorWithParameters, type)) {
        }
    }
}