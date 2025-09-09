using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls facial expressions and related visual features for a character.
/// </summary>
public class FaceExpressionController : MonoBehaviour
{
    [Header("Character Settings")]
    public string characterName;

    [Header("Expression Selections")]
    public string selectedMouth = "";
    public string selectedEyebrows = "";
    public string selectedTears = "";
    public string selectedCheeks = "";
    public string selectedEyes = "";
    public List<string> activeGraphicSymbols = new();
    public bool showHair = false;

    [Header("Eye Color Adjustments")]
    [Range(0f, 1f)] public float eyeHueShift = 0f;
    [Range(0f, 1f)] public float eyeSaturation = 1f;

    public string lastAppliedPreset = "default";

    private static readonly string[] hueTargetFolders = { "a", "b", "c1", "c2", "d", "e1", "e2" };

    public void ApplyAllExpressions()
    {
        ApplySingleExpression("mouth", selectedMouth);
        ApplySingleExpression("eyebrows", selectedEyebrows);
        ApplySingleExpression("tears", selectedTears);
        ApplySingleExpression("cheeks", selectedCheeks);
        ApplyEyes();
        ApplyGraphicSymbols();
        ApplyHair();
        ApplyEyeHueShift();
    }

    public void ApplySingleExpression(string groupName, string selected)
    {
        var group = FindDeepChild(transform, groupName);
        if (group == null) return;

        bool isNone = string.IsNullOrEmpty(selected) || selected.ToUpper() == "NONE";

        if (group.childCount > 0 && group.GetChild(0).childCount == 0)
        {
            foreach (Transform part in group)
                part.gameObject.SetActive(!isNone && part.name == selected);
        }
        else
        {
            foreach (Transform folder in group)
            {
                foreach (Transform part in folder)
                {
                    string key = folder.name + "_" + part.name;
                    part.gameObject.SetActive(!isNone && key == selected);
                }
            }
        }
    }


    public void ApplyEyes()
    {
        var group = FindDeepChild(transform, "eyes");
        if (group == null) return;

        string folderName = null, partName = null;

        if (!string.IsNullOrEmpty(selectedEyes) && selectedEyes.ToUpper() != "NONE")
        {
            var parts = selectedEyes.Split('_');
            if (parts.Length == 2)
            {
                folderName = parts[0].ToLower();
                partName = parts[1];
            }
        }

        foreach (Transform folder in group)
        {
            string currentFolderName = folder.name.ToLower();

            foreach (Transform part in folder)
            {
                bool isActive = false;
                string partNameLower = part.name.ToLower();
                bool isOverlay = partNameLower == "white" || partNameLower == "eyelids";

                if (currentFolderName == folderName)
                {
                    if (isOverlay || part.name == partName)
                        isActive = true;
                }

                part.gameObject.SetActive(isActive);
            }
        }
    }

    public void ApplyGraphicSymbols()
    {
        var group = FindDeepChild(transform, "graphic_symbols");
        if (group == null) return;

        foreach (Transform child in group)
            child.gameObject.SetActive(activeGraphicSymbols.Contains(child.name));
    }

    public void ApplyHair()
    {
        var hair = FindDeepChild(transform, "hair");
        if (hair != null) hair.gameObject.SetActive(showHair);
    }

    public void ApplyEyeHueShift()
    {
        var group = FindDeepChild(transform, "eyes");
        if (group == null) return;

        Material sharedMat = null;

        foreach (Transform folder in group)
        {
            if (!System.Array.Exists(hueTargetFolders, f => f == folder.name.ToLower())) continue;

            foreach (Transform part in folder)
            {
                if (!int.TryParse(part.name, out _)) continue;

                var sr = part.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sharedMaterial == null) continue;

                sharedMat ??= sr.sharedMaterial;
                sr.sharedMaterial = sharedMat;
            }
        }

        if (sharedMat != null)
        {
            if (sharedMat.HasProperty("_HueShift")) sharedMat.SetFloat("_HueShift", eyeHueShift);
            if (sharedMat.HasProperty("_Saturation")) sharedMat.SetFloat("_Saturation", eyeSaturation);
        }
    }

    public void ApplyPreset(string keyword)
    {
        var json = Resources.Load<TextAsset>("expression_presets");
        if (json == null)
        {
            Debug.LogError("expression_presets not found in Resources.");
            return;
        }

        var wrapper = JsonUtility.FromJson<ExpressionPresetList>(json.text);
        if (wrapper?.presets == null)
        {
            Debug.LogError("Failed to parse expression_presets.json");
            return;
        }

        var map = new Dictionary<string, ExpressionEntry>();
        foreach (var entry in wrapper.presets)
            map[entry.keyword] = entry;

        if (!map.TryGetValue(keyword, out var preset))
        {
            Debug.LogWarning($"Preset not found: {keyword}");
            return;
        }

        selectedCheeks = preset.cheeks;
        selectedMouth = preset.mouth;
        selectedEyes = preset.eyes;
        selectedTears = preset.tears;
        selectedEyebrows = preset.eyebrows;
        activeGraphicSymbols = new List<string>(preset.graphic_symbols);

        ApplyAllExpressions();
        lastAppliedPreset = keyword;
    }

    public void SetEyes(string name) { selectedEyes = name; ApplyEyes(); }
    public void SetEyebrows(string name) { selectedEyebrows = name; ApplySingleExpression("eyebrows", name); }
    public void SetTears(string name) { selectedTears = name; ApplySingleExpression("tears", name); }
    public void SetMouth(string name) { selectedMouth = name; ApplySingleExpression("mouth", name); }
    public void SetCheeks(string name) { selectedCheeks = name; ApplySingleExpression("cheeks", name); }

    public void SetEffect(string commaSeparatedIds)
    {
        activeGraphicSymbols.Clear();

        if (string.IsNullOrWhiteSpace(commaSeparatedIds) || commaSeparatedIds.ToUpper() == "NONE")
        {
            ApplyGraphicSymbols();
            return;
        }

        var ids = commaSeparatedIds.Split(',');
        foreach (var id in ids)
        {
            string trimmed = id.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                activeGraphicSymbols.Add(trimmed);
        }

        ApplyGraphicSymbols();
    }

    public void SetEyeHue(float value)
    {
        eyeHueShift = Mathf.Clamp01(value);
        ApplyEyeHueShift();
    }

    public void SetEyeSaturation(float value)
    {
        eyeSaturation = Mathf.Clamp01(value);
        ApplyEyeHueShift();
    }

    public void SetBangs(bool visible)
    {
        showHair = visible;
        ApplyHair();
    }


    private float bangsAlpha = 1f;

    /// <summary>
    /// Bangsの透明度を設定（0.0～1.0）
    /// </summary>
    public void SetBangsAlpha(float alpha)
    {
        bangsAlpha = Mathf.Clamp01(alpha);

        var hair = FindDeepChild(transform, "hair");
        if (hair == null) return;

        var sr = hair.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        var color = sr.color;
        color.a = bangsAlpha;
        sr.color = color;
    }

    public static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return child;

            var found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }
}

[System.Serializable]
public class ExpressionPresetList
{
    public List<ExpressionEntry> presets;
}

[System.Serializable]
public class ExpressionEntry
{
    public string keyword;
    
    public string cheeks;
    public string mouth;
    public string eyes;
    public string tears;
    public string eyebrows;
    public List<string> graphic_symbols;
}
