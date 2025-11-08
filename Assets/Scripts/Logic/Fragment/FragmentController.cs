using UnityEngine;
using System.Collections;

public class FragmentController : MonoBehaviour
{
    private PuzzleSlot targetSlot;
    private float duration;
    private bool isFlying = false;

    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsed = 0f;

    public ParticleSystem mergeEffect;
    public AudioClip mergeSound;

    private AudioSource audioSource;

    public void Initialize(PuzzleSlot target, float flyDuration)
    {
        targetSlot = target;
        duration = flyDuration;
        startPos = transform.position;
        endPos = target.transform.position;

        audioSource = gameObject.AddComponent<AudioSource>();
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // 曲线插值（缓出效果）
        float easedT = Mathf.SmoothStep(0, 1, t);
        transform.position = Vector3.Lerp(startPos, endPos, easedT);
        transform.Rotate(Vector3.forward, 180f * Time.deltaTime);

        if (t >= 1f) Arrive();
    }

    void Arrive()
    {
        isFlying = false;

        // 播放合并特效
        if (mergeEffect) Instantiate(mergeEffect, targetSlot.transform.position, Quaternion.identity);
        if (mergeSound) audioSource.PlayOneShot(mergeSound);

        // 通知拼图格子
        targetSlot.ReceiveFragment(this);
        Destroy(gameObject);
    }
}
