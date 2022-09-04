using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerService : MonoBehaviour {

    public static ManagerService Instance {
        get {
            if (instance == null) {
                GameObject go = new GameObject("--Manager Service--", typeof(ManagerService));
                DontDestroyOnLoad(go);
                instance = go.GetComponent<ManagerService>();
            }
            return instance;
        }
    }
    private static ManagerService instance;

    private Dictionary<Type, IManager> managers = new Dictionary<Type, IManager>();

    public void Register(IManager manager) {
        managers[manager.GetType()] = manager;
    }

    public T Get<T>() where T : class, IManager {
        if (managers.ContainsKey(typeof(T))) {
            IManager manager = managers[typeof(T)];
            return (T)manager;
        }
        Debug.LogWarning($"No manager of type {typeof(T).Name} registered.");
        return null;
    }
}
