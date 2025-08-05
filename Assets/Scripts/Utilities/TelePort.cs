using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePort : MonoBehaviour
{

    [SceneName] public string sceneFrom;
    [SceneName] public string sceneTo;

    public void TeleportToScene()
    {
        TransitionManager.Instance.Transtion(sceneFrom, sceneTo);
    }
}
