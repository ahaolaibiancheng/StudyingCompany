using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownCharacterLoader : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("DropdownCharacterSelectOptions");
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(lines));
        }
        else
        {
            Debug.LogError("DropdownCharacterSelectOptions.txt not found in Resources!");
        }
    }
}
