using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ExpressionPreset
{
    public string keyword;
    public string cheeks;
    public string mouth;
    public string eyes;
    public string tears;
    public string eyebrows;
    public List<string> graphic_symbols;
}

[System.Serializable]
public class PresetList
{
    public List<ExpressionPreset> presets;
}

public class CharacterLoader : MonoBehaviour
{
    [Header("UI Elements")]
    // public Dropdown characterDropdown;
    // public TMP_Dropdown presetDropdown;

    // [Header("Command Labels")]
    // public TMP_Text eyebrowsCommandLabel;
    // public TMP_Text eyesCommandLabel;
    // public TMP_Text mouthCommandLabel;
    // public TMP_Text tearsCommandLabel;
    // public TMP_Text cheeksCommandLabel;
    // public TMP_Text effectCommandLabel;
    // public TMP_Text presetCommandLabel;

    [Header("Character Spawning")]
    public Transform spawnRoot;
    public List<GameObject> characterPrefabs;

    [Header("Character Adjuster")]
    public CharacterAdjuster characterAdjuster;

    // [Header("Toggle Groups")]
    // public List<Toggle> cheekToggles;
    // public List<Toggle> mouthToggles;
    // public List<Toggle> tearToggles;
    // public List<Toggle> eyebrowToggles;
    // public List<Toggle> eyeToggles;
    // public List<Toggle> graphicSymbolToggles;

    private FaceExpressionController faceController;
    private GameObject currentCharacter;
    private List<ExpressionPreset> allPresets;

    private void OnEnable()
    {
        DecorateEventHandler.DecorateChooseUIDChanged += HandleChooseUIDChanged;
    }

    private void OnDisable()
    {
        DecorateEventHandler.DecorateChooseUIDChanged -= HandleChooseUIDChanged;
    }

    public void HandleChooseUIDChanged(string uid)
    {
        DecorateLocalItem item = DecorateController.Instance.GetDecorateLocalItemByUId(uid);
        DecorateTableItem decorate = DecorateController.Instance.GetDecorateItemById((DecorateType)item.type, item.id);
        if (decorate != null)
        {
            Debug.Log("type: " + decorate.type + "id: " + decorate.id);
            UpdataCharacter(decorate.type, decorate.name);
        }
    }

    void UpdataCharacter(int type, string name)
    {
        switch (type)
        {
            case 0:
                faceController?.SetMouth(name);
                break;
            case 1:
                faceController?.SetEyes(name);
                break;
        }
    }

    void Start()
    {

        LoadPresetsFromJson();

        // presetDropdown.ClearOptions();
        // presetDropdown.AddOptions(allPresets.Select(p => p.keyword).ToList());
        // presetDropdown.onValueChanged.AddListener(OnPresetSelected);


        // LoadCharacter(characterDropdown.value);
        LoadCharacter(0);

        // BindToggles(cheekToggles, "SetCheeks", val => faceController?.SetCheeks(val), cheeksCommandLabel);
        // BindToggles(mouthToggles, "SetMouth", val => faceController?.SetMouth(val), mouthCommandLabel);
        // BindToggles(tearToggles, "SetTears", val => faceController?.SetTears(val), tearsCommandLabel);
        // BindToggles(eyebrowToggles, "SetEyebrows", val => faceController?.SetEyebrows(val), eyebrowsCommandLabel);
        // BindToggles(eyeToggles, "SetEyes", val => faceController?.SetEyes(val), eyesCommandLabel);

        // foreach (var toggle in graphicSymbolToggles)
        // {
        //     toggle.onValueChanged.AddListener(_ =>
        //     {
        //         if (faceController == null) return;
        //         var selected = graphicSymbolToggles
        //             .Where(t => t.isOn)
        //             .Select(t => t.gameObject.name)
        //             .ToList();

        //         faceController.SetEffect(string.Join(",", selected));
        //         effectCommandLabel.text = $"SetEffect(\"{string.Join(",", selected)}\");";
        //         presetCommandLabel.text = string.Empty;
        //     });
        // }

        // characterDropdown.onValueChanged.AddListener(OnCharacterSelected);
    }


    void LoadPresetsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("expression_presets");
        if (jsonFile != null)
        {
            allPresets = JsonUtility.FromJson<PresetList>(jsonFile.text).presets;
        }
        else
        {
            Debug.LogError("Preset file not found in Resources: expression_presets");
            allPresets = new List<ExpressionPreset>();
        }
    }

    // void OnPresetSelected(int index)
    // {
    //     if (faceController == null || allPresets == null || index < 0 || index >= allPresets.Count)
    //         return;

    //     var preset = allPresets[index];

    //     faceController.SetEyebrows(preset.eyebrows);
    //     faceController.SetEyes(preset.eyes);
    //     faceController.SetMouth(preset.mouth);
    //     faceController.SetTears(preset.tears);
    //     faceController.SetCheeks(preset.cheeks);
    //     faceController.SetEffect(string.Join(",", preset.graphic_symbols));

    //     ApplyToggleGroup(eyebrowToggles, preset.eyebrows);
    //     ApplyToggleGroup(eyeToggles, preset.eyes);
    //     ApplyToggleGroup(mouthToggles, preset.mouth);
    //     ApplyToggleGroup(tearToggles, preset.tears);
    //     ApplyToggleGroup(cheekToggles, preset.cheeks);
    //     ApplyToggleGroup(graphicSymbolToggles, preset.graphic_symbols);

    //     UpdateCommandLabels(preset.keyword);
    // }

    void ApplyToggleGroup(List<Toggle> toggles, string name)
    {
        string trimmed = name?.Trim().ToUpper();

        foreach (var toggle in toggles)
        {
            string toggleName = toggle.gameObject.name.Trim().ToUpper();
            if (string.IsNullOrEmpty(trimmed) || trimmed == "NONE")
            {
                toggle.SetIsOnWithoutNotify(toggleName == "NONE");
            }
            else
            {
                toggle.SetIsOnWithoutNotify(toggleName == trimmed);
            }
        }
    }

    void ApplyToggleGroup(List<Toggle> toggles, List<string> names)
    {
        var cleanNames = names.Select(n => n?.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToList();

        foreach (var toggle in toggles)
        {
            string toggleName = toggle.gameObject.name.Trim();
            bool isMatch = cleanNames.Any(name => toggleName == name || toggleName.EndsWith("_" + name));
            toggle.SetIsOnWithoutNotify(isMatch);
        }
    }

    // void BindToggles(List<Toggle> toggles, string command, System.Action<string> setter, TMP_Text label)
    // {
    //     foreach (var toggle in toggles)
    //     {
    //         string value = toggle.gameObject.name;
    //         toggle.onValueChanged.AddListener(isOn =>
    //         {
    //             if (!isOn || faceController == null) return;

    //             string actualValue = value == "NONE" ? string.Empty : value;
    //             setter(actualValue);
    //             label.text = $"{command}(\"{actualValue}\");";
    //             presetCommandLabel.text = string.Empty;
    //         });
    //     }
    // }

    // void UpdateCommandLabels(string presetKeyword = "")
    // {
    //     if (faceController == null) return;

    //     eyebrowsCommandLabel.text = $"SetEyebrows(\"{faceController.selectedEyebrows}\");";
    //     eyesCommandLabel.text = $"SetEyes(\"{faceController.selectedEyes}\");";
    //     mouthCommandLabel.text = $"SetMouth(\"{faceController.selectedMouth}\");";
    //     tearsCommandLabel.text = $"SetTears(\"{faceController.selectedTears}\");";
    //     cheeksCommandLabel.text = $"SetCheeks(\"{faceController.selectedCheeks}\");";
    //     effectCommandLabel.text = $"SetEffect(\"{string.Join(",", faceController.activeGraphicSymbols)}\");";
    //     presetCommandLabel.text = string.IsNullOrEmpty(presetKeyword) ? string.Empty : $"ApplyPreset(\"{presetKeyword}\");";
    // }

    void OnCharacterSelected(int index)
    {
        LoadCharacter(index);
    }

    void LoadCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Count) return;

        if (currentCharacter != null)
            Destroy(currentCharacter);

        GameObject prefab = characterPrefabs[index];
        currentCharacter = Instantiate(prefab, spawnRoot);
        currentCharacter.transform.localPosition = Vector3.zero;

        faceController = currentCharacter.GetComponentInChildren<FaceExpressionController>();

        // if (faceController != null && allPresets != null)
        // {
        //     int defaultIndex = allPresets.FindIndex(p => p.keyword == "default");
        //     if (defaultIndex >= 0)
        //     {
        //         presetDropdown.SetValueWithoutNotify(defaultIndex);
        //         OnPresetSelected(defaultIndex);
        //     }
        // }
        characterAdjuster?.SetTargetCharacter(currentCharacter);
    }
}
