using System;

namespace Utilities.Singleton.Exceptions {
    public sealed class HasPublicConstructorException : ConstructorException {
       
        /// <summary>
        /// Placeholder for strign to be formatted for exception message.
        /// </summary>
        private const string HasPublicCtor = "{0} has public constructor(s).";

        /// <summary>
        /// This exception is thrown when a class inheriting <see cref="SingletonBase{T}" /> has public constructor(s).
        /// </summary>
        /// <param name="type">Type which implements <see cref="SingletonBase{T}" /> and has public constructor(s).</param>
        public HasPublicConstructorException(Type type) : base(type, string.Format(HasPublicCtor, type.FullName)) {
        }
    }
}