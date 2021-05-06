using System;
using UnityEngine;

[Serializable]
public class AddonInfo
{
    public string Name = string.Empty;
    public string Author = string.Empty;
    public string Contact = string.Empty;
    public string[] Tags = null;
    public string Version = string.Empty;

    public static AddonInfo FromJSON(string json)
    {
        return JsonUtility.FromJson<AddonInfo>(json);
    }

    public static string ToJSON(AddonInfo info)
    {
        return JsonUtility.ToJson(info);
    }

    public void Validate()
    {
        if (Name == string.Empty)
            Name = "Unknown";

        if (Author == string.Empty)
            Author = "Unknown";

        if (Contact == string.Empty)
            Contact = "Unknown";

        if (Tags == null)
            Tags = new string[] { };

        if (Version == string.Empty)
            Version = "Unknown";
    }
}
