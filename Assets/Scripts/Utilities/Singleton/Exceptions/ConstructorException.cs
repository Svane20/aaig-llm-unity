using System;

namespace Utilities.Singleton.Exceptions {
    public abstract class ConstructorException : Exception {
        
        /// <summary>
        /// Placeholder for string to be formatted for exception message.
        /// </summary>
        private const string ExpectedTohave = "{0} {1} is expected to have only non-public parameterless constructor.";

        /// <summary>
        /// Base exception for constructor related exceptions inheriting from <see cref="SingletonBase{T}"/>.
        /// </summary>
        /// <param name="type">Type which has constructor related exception.</param>
        /// <param name="message">Exception message passed from inheriting exception.</param>
        protected ConstructorException(Type type, string message) : base(string.Format(ExpectedTohave, message, type)) {
        }
    }
}