using System;

namespace Utilities.Singleton.Exceptions {
    public class NotRegisteredTypeException : Exception {
        public NotRegisteredTypeException(Type type):base($"{type.FullName} is not registered at {typeof(SingletonManager)}. Register by calling {nameof(SingletonManager.Register)} method.") {
            
        }
    }
}