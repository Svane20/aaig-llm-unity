#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

namespace Utilities
{
    public static class ApplicationUtility
    {
        // A generic "is focused" method that also returns true when the Unity application window
        // is active, regardless the specific editor window that is focused.
        public static bool IsActivated()
        {
#if UNITY_EDITOR
            return InternalEditorUtility.isApplicationActive;
#else
         return Application.isFocused;
#endif
        }
    }
}


