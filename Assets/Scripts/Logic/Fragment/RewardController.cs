using UnityEngine;
using UnityEngine.Playables;

public class RewardController : MonoBehaviour
{
    public PlayableDirector storyTimeline;
    public AudioClip rewardSound;

    public void TriggerStory()
    {
        Debug.Log("🎉 拼图完成！触发奖励故事！");
        if (storyTimeline) storyTimeline.Play();
        if (rewardSound) AudioSource.PlayClipAtPoint(rewardSound, Camera.main.transform.position);
    }
}
