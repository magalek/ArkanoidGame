using UnityEngine;

public abstract class MonoManager : MonoBehaviour, IManager {

    protected virtual void Awake() {
        ManagerService.Instance.Register(this);
        Debug.Log($"Registering {GetType().Name}");
    }
}
