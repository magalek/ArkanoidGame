using UnityEngine;
using System;

public class PlayerPrefsDataContainer<T> : IDataContainer<T> {

    public bool HasData => PlayerPrefs.HasKey(DataKey);

    private readonly string DataKey;

    public PlayerPrefsDataContainer(string key) {
        DataKey = key;
    }

    public T Load() {
        if (!HasData) {
            Debug.LogError("No data to load. Returning null");
            return default;
        }
        if (typeof(T) == typeof(int)) return (T)(object)PlayerPrefs.GetInt(DataKey);
        if (typeof(T) == typeof(float)) return (T)(object)PlayerPrefs.GetFloat(DataKey);
        if (typeof(T) == typeof(string)) return (T)(object)PlayerPrefs.GetString(DataKey);
        if (typeof(T) == typeof(bool)) return (T)(object)(PlayerPrefs.GetInt(DataKey) == 1);
        return JsonUtility.FromJson<T>(PlayerPrefs.GetString(DataKey));
    }

    public void Save(T data) {
        if (typeof(T) == typeof(int)) { PlayerPrefs.SetInt(DataKey, (int)(object)data); return; }
        if (typeof(T) == typeof(float)) { PlayerPrefs.SetFloat(DataKey, (float)(object)data); return; }
        if (typeof(T) == typeof(string)) { PlayerPrefs.SetString(DataKey, (string)(object)data); return; }
        if (typeof(T) == typeof(bool)) { PlayerPrefs.SetInt(DataKey, ((bool)(object)data) == true ? 1 : 0); return; }
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(DataKey, json);
    }

    public void Delete() => PlayerPrefs.DeleteKey(DataKey);
}

public static class PrefsKeys {
    public const string SAVE_STATE_KEY = "save_state";
    public const string HIGHSCORE_KEY = "highscore";
    public const string CONTINUE_USED__KEY = "used_continue";
}
