using System.Collections;
using UnityEngine;

namespace BodySound.Utils
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static void Run(IEnumerator routine)
        {
            if (_instance == null)
            {
                var go = new GameObject("TimeStretchCoroutineRunner");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineRunner>();
            }

            _instance.StartCoroutine(routine);
        }
    }
}