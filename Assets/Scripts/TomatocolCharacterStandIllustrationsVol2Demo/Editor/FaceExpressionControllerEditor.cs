using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[CustomEditor(typeof(FaceExpressionController))]
public class FaceExpressionControllerEditor : Editor
{
    private Dictionary<string, ExpressionPreset> presetMap;
    private List<string> presetList = new();

    public override void OnInspectorGUI()
    {
        var ctrl = (FaceExpressionController)target;
        LoadPresetJson();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Character", ctrl.characterName);

        DrawPresetSelector(ctrl);
        ctrl.selectedCheeks = DrawDropdown(ctrl, "cheeks", ctrl.selectedCheeks);
        ctrl.selectedMouth = DrawFolderDropdown(ctrl, "mouth", ctrl.selectedMouth);
        ctrl.selectedEyes = DrawEyesDropdown(ctrl, ctrl.selectedEyes);
        DrawEyeHueAndSaturation(ctrl);
        ctrl.selectedTears = DrawFolderDropdown(ctrl, "tears", ctrl.selectedTears);
        ctrl.selectedEyebrows = DrawFolderDropdown(ctrl, "eyebrows", ctrl.selectedEyebrows);
        ctrl.showHair = EditorGUILayout.Toggle("Show Bangs", ctrl.showHair);


        // Bangs Alpha
        EditorGUI.BeginChangeCheck();
        float newAlpha = EditorGUILayout.Slider("Bangs Alpha", GetPrivateField<float>(ctrl, "bangsAlpha"), 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(ctrl, "Change Bangs Alpha");
            ctrl.SetBangsAlpha(newAlpha);
            EditorUtility.SetDirty(ctrl);
        }

        DrawGraphicSymbols(ctrl);

        if (GUILayout.Button("Apply Expressions"))
        {
            ctrl.ApplyAllExpressions();
            EditorUtility.SetDirty(ctrl);
        }

        if (GUI.changed)
        {
            ctrl.ApplyAllExpressions();
            EditorUtility.SetDirty(ctrl);
        }
    }

    private void LoadPresetJson()
    {
        if (presetMap != null) return;

        TextAsset json = Resources.Load<TextAsset>("expression_presets");
        if (json != null)
        {
            ExpressionPresetWrapper data = JsonUtility.FromJson<ExpressionPresetWrapper>(json.text);
            presetMap = new Dictionary<string, ExpressionPreset>();
            presetList.Clear();

            foreach (var entry in data.presets)
            {
                presetMap[entry.keyword] = entry.preset;
                presetList.Add(entry.keyword);
            }
        }
        else
        {
            Debug.LogWarning("Could not load expression preset JSON from Resources.");
        }
    }

    private void DrawPresetSelector(FaceExpressionController ctrl)
    {
        if (presetMap == null || presetList.Count == 0) return;

        int currentIndex = Mathf.Max(0, presetList.IndexOf(ctrl.lastAppliedPreset));
        int selectedIndex = EditorGUILayout.Popup("Expression Preset", currentIndex, presetList.ToArray());
        string selectedKeyword = presetList[selectedIndex];

        if (selectedKeyword != ctrl.lastAppliedPreset)
        {
            ctrl.ApplyPreset(selectedKeyword);
            ctrl.lastAppliedPreset = selectedKeyword;
            EditorUtility.SetDirty(ctrl);
        }
    }

    private void DrawEyeHueAndSaturation(FaceExpressionController ctrl)
    {
        EditorGUILayout.LabelField("Eye Color Adjustments", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        float newHue = EditorGUILayout.Slider("Hue", ctrl.eyeHueShift, 0f, 1f);
        float newSaturation = EditorGUILayout.Slider("Saturation", ctrl.eyeSaturation, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(ctrl, "Change Eye Hue/Saturation");
            ctrl.SetEyeHue(newHue);
            ctrl.SetEyeSaturation(newSaturation);
            EditorUtility.SetDirty(ctrl);
        }
    }

    private string DrawDropdown(FaceExpressionController ctrl, string groupName, string currentValue)
    {
        var group = FindDeepChild(GetRootTransform(ctrl), groupName);
        if (group == null) return currentValue;

        List<string> options = new();

        foreach (Transform part in group)
            options.Add(part.name);

        options = NaturalSort(options);
        options.Add("None");

        if (string.IsNullOrEmpty(currentValue))
            currentValue = groupName == "tears" ? "None" : options[0];

        int currentIndex = Mathf.Max(0, options.IndexOf(currentValue));
        int newIndex = EditorGUILayout.Popup(groupName, currentIndex, options.ToArray());
        return options[newIndex] == "None" ? "" : options[newIndex];
    }

    private string DrawFolderDropdown(FaceExpressionController ctrl, string groupName, string currentValue)
    {
        var group = FindDeepChild(GetRootTransform(ctrl), groupName);
        if (group == null) return currentValue;

        List<string> options = new();

        foreach (Transform folder in group)
            foreach (Transform part in folder)
                options.Add(folder.name + "_" + part.name);

        options = NaturalSort(options);
        options.Add("None");

        if (string.IsNullOrEmpty(currentValue))
            currentValue = options[0];

        int currentIndex = Mathf.Max(0, options.IndexOf(currentValue));
        int newIndex = EditorGUILayout.Popup(groupName, currentIndex, options.ToArray());
        return options[newIndex] == "None" ? "" : options[newIndex];
    }

    private string DrawEyesDropdown(FaceExpressionController ctrl, string currentValue)
    {
        var group = FindDeepChild(GetRootTransform(ctrl), "eyes");
        if (group == null) return currentValue;

        List<string> options = new();
        HashSet<string> baseFolders = new() { "a", "b", "c1", "c2" };
        Dictionary<string, int> maxNumberMap = new();

        foreach (Transform folder in group)
        {
            string folderName = folder.name.ToLower();
            foreach (Transform part in folder)
            {
                string partName = part.name.ToLower();
                if (partName == "white" || partName == "eyelids") continue;

                string key = $"{folder.name}_{part.name}";
                options.Add(key);

                if (baseFolders.Contains(folderName) && int.TryParse(part.name, out int num))
                {
                    if (!maxNumberMap.ContainsKey(folderName) || maxNumberMap[folderName] < num)
                        maxNumberMap[folderName] = num;
                }
            }
        }

        foreach (var kvp in maxNumberMap)
        {
            string folder = kvp.Key;
            int next = kvp.Value + 1;
            options.Add($"{folder}_{next}");
        }

        options = NaturalSort(options);
        options.Add("None");

        int currentIndex = Mathf.Max(0, options.IndexOf(currentValue));
        int newIndex = EditorGUILayout.Popup("eyes", currentIndex, options.ToArray());
        return options[newIndex] == "None" ? "" : options[newIndex];
    }

    private void DrawGraphicSymbols(FaceExpressionController ctrl)
    {
        var group = FindDeepChild(GetRootTransform(ctrl), "graphic_symbols");
        if (group == null) return;

        EditorGUILayout.LabelField("Graphic Symbols", EditorStyles.boldLabel);

        List<string> current = new(ctrl.activeGraphicSymbols);
        List<string> allNames = group.Cast<Transform>().Select(child => child.name).ToList();
        allNames = NaturalSort(allNames);

        foreach (string name in allNames)
        {
            bool isOn = current.Contains(name);
            bool newOn = EditorGUILayout.ToggleLeft(name, isOn);
            if (newOn && !current.Contains(name)) current.Add(name);
            else if (!newOn && current.Contains(name)) current.Remove(name);
        }

        ctrl.SetEffect(string.Join(",", current));
    }

    private Transform GetRootTransform(FaceExpressionController ctrl)
    {
        if (ctrl.transform.name == ctrl.characterName)
            return ctrl.transform;

        var found = ctrl.transform.Find(ctrl.characterName);
        return found != null ? found : ctrl.transform;
    }

    private List<string> NaturalSort(List<string> input)
    {
        return input.OrderBy(s => Regex.Replace(s, @"\d+", m => m.Value.PadLeft(10, '0'))).ToList();
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return child;

            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null && field.GetValue(obj) is T value)
            return value;
        return default;
    }


    [System.Serializable] private class ExpressionPresetWrapper { public List<ExpressionEntry> presets; }
    [System.Serializable] private class ExpressionEntry { public string keyword; public ExpressionPreset preset; }
    [System.Serializable]
    private class ExpressionPreset
    {
        public string cheeks;
        public string mouth;
        public string eyes;
        public string tears;
        public string eyebrows;
        public List<string> graphic_symbols;
    }
}