using System;

namespace Utilities.Singleton.Attributes {
    
    /// <summary>
    /// Indicates the implementation is not thread safe.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NotThreadSafeAttribute : Attribute {
      
    }
}