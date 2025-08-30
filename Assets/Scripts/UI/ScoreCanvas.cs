using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCanvas : BasePanel
{
    public Button characterBtn;
    public Button petBtn;

    void Start()
    {
        characterBtn.onClick.AddListener(OnCharacterBtnClicked);
        petBtn.onClick.AddListener(OnPetBtnClicked);
    }

    private void OnCharacterBtnClicked()
    {
        UIManager.Instance.OpenPanel(UIConst.CharacterCanvas);
    }
    
    private void OnPetBtnClicked()
    {
        throw new NotImplementedException();
    }
}
