using System;
using UnityEngine;

namespace Utils {
    
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();
    
    public static T Instance
    {
        get {
            if (!Application.isPlaying) return null;
            if (_isApplicationQuitting) return null;
            if (_instance != null) return _instance;

            lock (_lock)
            {
                var instances = FindObjectsOfType(typeof(T));
                if (instances == null || instances.Length == 0)
                {
                    var delimited = typeof(T).ToString().Split(new char[]{'.'});
                    var gameObject = new GameObject(delimited[delimited.Length - 1]);
                    _instance = gameObject.AddComponent<T>();
                }
                else if (instances.Length == 1)
                {
                    _instance = instances[0] as T;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("[MonoSingleton] Something went really wrong " +
                                   " - there should never be more than 1 singleton!" +
                                   " Reopen the scene might fix it.");
                }
                
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance)
        {
            DestroyImmediate(gameObject);
            _isApplicationQuitting = false;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = gameObject.GetComponent<T>();
        }
    }

    private static bool _isApplicationQuitting = false;

    protected void OnDestroy()
    {
        _isApplicationQuitting = true;
    }
}

} // Utils
