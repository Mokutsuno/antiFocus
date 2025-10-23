// Scripts/SessionLogger.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SessionEntry
{
    public long timestamp;    // Unix ms
    public int focusSeconds;  // é¿ê—
    public int breakSeconds;  // ê›íË
    public int subjective;    // 0-10 è´óàägí£
}

[Serializable]
public class SessionData
{
    public List<SessionEntry> entries = new();
}

public class SessionLogger : MonoBehaviour
{
    public static SessionLogger Instance { get; private set; }
    private string _path;
    private SessionData _data = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _path = Path.Combine(Application.persistentDataPath, "sessions.json");
        Load();
    }

    public void AddSession(int focusSec, int breakSec)
    {
        _data.entries.Add(new SessionEntry
        {
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            focusSeconds = Mathf.Max(0, focusSec),
            breakSeconds = breakSec,
            subjective = -1
        });
        Save();
    }

    public IReadOnlyList<SessionEntry> All() => _data.entries;

    private void Load()
    {
        try
        {
            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                _data = JsonUtility.FromJson<SessionData>(json) ?? new SessionData();
            }
        }
        catch (Exception e) { Debug.LogWarning($"Load sessions failed: {e.Message}"); }
    }

    private void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(_path, json);
        }
        catch (Exception e) { Debug.LogWarning($"Save sessions failed: {e.Message}"); }
    }
}