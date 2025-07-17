using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeend;
    public GameObject panel;

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * animationSpeend;
            gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(time);
            yield return null;
        }
    }

    IEnumerator HidePanel(GameObject gameObject)
    {
        float time = 0;
        while (time < 1)
        {
            gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(time);
            time += Time.deltaTime * animationSpeend;
            yield return null;
        }
    }
}
