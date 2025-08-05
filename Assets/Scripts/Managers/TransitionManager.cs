using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [SceneName] public string sceneName;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;
    private bool isfade;
    private bool canTransition;

    void Start()
    {
        isfade = false;
        canTransition = true;
        StartCoroutine(TransitionToScene(string.Empty, sceneName));
    }
    public void Transtion(string from, string to)
    {
        if (!isfade && canTransition)
        {
            StartCoroutine(TransitionToScene(from, to));    // 协程
        }
    }

    private IEnumerator TransitionToScene(string from, string to)
    {
        yield return Fade(1);
        if (from != string.Empty)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return SceneManager.UnloadSceneAsync(from);
        }
        // 叠加在persistent场景之上
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        // 激活新场景，用场景序号的方式
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);  // 比匹配名字性能好
        SceneManager.SetActiveScene(newScene);

        // EventHandler.CallShowDialogueEvent(string.Empty);
        EventHandler.CallAfterSceneloadEvent();
        yield return Fade(0);
    }

    // 渐变, targetAlpha：0是透明，1是黑
    private IEnumerator Fade(float targetAlpha)
    {
        isfade = true;
        fadeCanvasGroup.blocksRaycasts = true;

        float speed = Math.Abs(targetAlpha - fadeCanvasGroup.alpha) / fadeDuration;

        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        // 渐变结束, 鼠标恢复
        fadeCanvasGroup.blocksRaycasts = false;
        isfade = false;
    }
}
