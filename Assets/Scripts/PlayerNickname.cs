using UnityEngine;

/// <summary>
/// Stores and retrieves the player's nickname. Persists across sessions via PlayerPrefs.
/// </summary>
public static class PlayerNickname
{
    private const string Key = "PlayerNickname";
    private static string _cached;

    public static string Get()
    {
        if (string.IsNullOrEmpty(_cached))
            _cached = PlayerPrefs.GetString(Key, "Hero");
        return _cached;
    }

    public static void Set(string nickname)
    {
        string trimmed = string.IsNullOrWhiteSpace(nickname) ? "Hero" : nickname.Trim();
        if (trimmed.Length > 16) trimmed = trimmed.Substring(0, 16);
        _cached = trimmed;
        PlayerPrefs.SetString(Key, trimmed);
        PlayerPrefs.Save();
    }

    public static bool HasBeenSet()
    {
        return PlayerPrefs.HasKey(Key);
    }
}
