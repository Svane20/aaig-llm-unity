using System;
using UnityEngine;

namespace GameSystems.Dialogue
{
    public static class JsonHelper
    {
        public static T[] GETJsonArray<T>(string json)
        {
            var newJson = "{ \"array\": " + json + "}";
            var wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
            return wrapper.array;
        }
 
        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}