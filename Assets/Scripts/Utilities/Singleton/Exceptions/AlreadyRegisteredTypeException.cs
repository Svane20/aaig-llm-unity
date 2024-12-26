using System;

namespace Utilities.Singleton.Exceptions {
    public class AlreadyRegisteredTypeException : Exception {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public AlreadyRegisteredTypeException(Type type):base($"{type.FullName} is already registered at {typeof(SingletonManager)}.") {
            
        }
    }
}