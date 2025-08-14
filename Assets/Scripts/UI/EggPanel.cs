using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggPanel : MonoBehaviour
{
    public Button shakeBtn;
    public Button interactBtn;
    public Button hatchBtn;

    public void OnShakeBtnClicked()
    {
        Debug.Log(">>>>> OnShakeBtnClicked");
    }

    public void OnInteractBtnClicked()
    {
         Debug.Log(">>>>> OnInteractBtnClicked");
    }

    public void OnHatchBtnClicked()
    {
        Debug.Log(">>>>> OnHatchBtnClicked");
    }
}
