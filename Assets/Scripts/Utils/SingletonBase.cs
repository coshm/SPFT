using UnityEngine;
using System;
using System.Collections;

public abstract class SingletonBase<T> : MonoBehaviour where T : UnityEngine.Object {

    private static T instance;
    public static T Instance {
        get {
            if (!instance) {
                instance = FindObjectOfType(typeof(T)) as T;
                if (!instance) {
                    Debug.LogError($"There needs to be one active {typeof(T)} script on a GameObject in your scene.");
                }
            }
            return instance;
        }
    }
}
