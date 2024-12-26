using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Utilities.Singleton.Attributes;
using Utilities.Singleton.Exceptions;

namespace Utilities.Singleton {
    public abstract class SingletonBase<T> {
        
        /// <summary>
        /// Instance creator for this class. <see cref="Lazy{T}" /> guarantees that <see cref="InstanceCreator" /> will only run once.
        /// </summary>
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(InstanceCreator);

        /// <summary>
        /// Counter which holds how many instances this class has.
        /// Since this is a Singleton implementation, <see cref="_instanceCount" /> can at most be one (1).
        /// </summary>
        private static int _instanceCount;

        /// <summary>
        /// </summary>
        protected SingletonBase() {
            Interlocked.Increment(ref _instanceCount);
            if (_instanceCount > 1) {
                throw new Exception($"{nameof(T)} is Singleton but it has {_instanceCount} instances");
            }
        }

        /// <summary>
        /// Access the instance of this Singleton. The first time you access, it is lazily instantiated.
        /// </summary>
        /// <remarks>This property must be thread-safe.</remarks>
        [ThreadSafe]
        public static T Instance => LazyInstance.Value;

        /// <summary>
        /// </summary>
        /// <returns>An instance of <see cref="T" /></returns>
        private static T InstanceCreator() {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var publicCtors = ctors.Where(c => c.IsPublic);
            if (publicCtors.Any()) {
                throw new HasPublicConstructorException(typeof(T));
            }

            var ctorsWithParameters = ctors.Where(c => c.GetParameters().Length > 0);
            if (ctorsWithParameters.Any()) {
                throw new HasConstructorWithParametersException(typeof(T));
            }

            return (T) Activator.CreateInstance(typeof(T), true);
        }
    }
}