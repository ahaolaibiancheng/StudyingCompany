using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvas : BasePanel
{
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(OnCloseBtnClicked);
    }

    private void OnCloseBtnClicked()
    {
        UIManager.Instance.ClosePanel(UIConst.CharacterCanvas);
    }
}
