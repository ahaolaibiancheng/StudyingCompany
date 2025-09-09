using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CopyCommandButton : MonoBehaviour
{
    [Header("Target label to copy")]
    public TMP_Text targetLabel;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(CopyToClipboard);
        }
        else
        {
            Debug.LogWarning("[CopyCommandButton] No Button component found.");
        }
    }

    private void CopyToClipboard()
    {
        if (targetLabel == null)
        {
            Debug.LogWarning("[CopyCommandButton] targetLabel is not assigned.");
            return;
        }

        string textToCopy = targetLabel.text;
        GUIUtility.systemCopyBuffer = textToCopy;
        Debug.Log($"[CopyCommandButton] Copied to clipboard: \"{textToCopy}\"");
    }
}
