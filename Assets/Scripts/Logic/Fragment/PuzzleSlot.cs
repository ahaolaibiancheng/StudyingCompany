using UnityEngine;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour
{
    public bool isFilled = false;
    public Image slotImage;
    public Image filledImage;
    public ParticleSystem highlightEffect;

    public void ReceiveFragment(FragmentController fragment)
    {
        isFilled = true;
        if (slotImage) slotImage.enabled = false;
        if (filledImage) filledImage.enabled = true;
        if (highlightEffect) highlightEffect.Play();

        FindObjectOfType<PuzzleManager>().AddFragmentToSlot(this);
    }
}
