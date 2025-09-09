using UnityEngine;

public class WindowResizer : MonoBehaviour
{
    void Start()
    {
        int targetWidth = 1920;
        int targetHeight = 1080;

        // Get current display resolution
        int screenW = Display.main.systemWidth;
        int screenH = Display.main.systemHeight;

        // Debug log
        Debug.Log($"[WindowResizer] Display resolution: {screenW}x{screenH}");

        // Margin to avoid taskbar/titlebar cutoff
        int margin = 100;

        if (targetWidth + margin > screenW || targetHeight + margin > screenH)
        {
            float scaleFactor = Mathf.Min((screenW - margin) / (float)targetWidth, (screenH - margin) / (float)targetHeight);
            int newW = Mathf.RoundToInt(targetWidth * scaleFactor);
            int newH = Mathf.RoundToInt(targetHeight * scaleFactor);

            Screen.SetResolution(newW, newH, false);
            Debug.Log($"[WindowResizer] Window resized to fit screen: {newW}x{newH}");
        }
        else
        {
            Screen.SetResolution(targetWidth, targetHeight, false);
            Debug.Log($"[WindowResizer] Full resolution applied: {targetWidth}x{targetHeight}");
        }
    }
}
