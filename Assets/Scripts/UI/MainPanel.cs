using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public Button UIMenuBtn;
    public Button UIEmailBtn;
    public Button UIPackageBtn;
    public Button UITaskBtn;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        UIMenuBtn.onClick.AddListener(OnMenuBtnClicked);
        UIEmailBtn.onClick.AddListener(OnEmailBtnClicked);
        UIPackageBtn.onClick.AddListener(OnPackageBtnClicked);
        UITaskBtn.onClick.AddListener(OnTaskBtnClicked);
    }

    public void OnMenuBtnClicked()
    {
        UIManager.Instance.OpenPanel(UIConst.MenuPanel);
    }

    public void OnEmailBtnClicked()
    {
        Debug.Log(">>>>> OnEmailBtnClicked");
        // UIManager.Instance.OpenPanel(UIConst.EmailPanel);
    }

    public void OnPackageBtnClicked()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }

    public void OnTaskBtnClicked()
    {
        UIManager.Instance.OpenPanel(UIConst.TaskPanel);
    }

    private void OnQuitGame()
    {
        print(">>>>> OnQuitGame");
// #if UNITY_EDITOR
//         EditorApplication.isPlaying = false;
// #else
        Application.Quit();
// #endif
    }
}