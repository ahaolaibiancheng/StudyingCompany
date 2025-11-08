using UnityEngine;

public enum AppMode { Pomodoro, Stopwatch, Countdown }
public enum PomodoroPhase { Work, ShortBreak, LongBreak }

public class PomodoroController : MonoBehaviour
{
    public AppMode CurrentMode = AppMode.Pomodoro;
    public PomodoroPhase Phase = PomodoroPhase.Work;

    public float WorkDuration = 25 * 60f;
    public float ShortBreakDuration = 5 * 60f;
    public float LongBreakDuration = 15 * 60f;
    public int Cycles = 4;

    private float time;
    private float elapsed;
    private bool isRunning;

    void Start() => ResetTimer();

    void Update()
    {
        if (!isRunning) return;

        switch (CurrentMode)
        {
            case AppMode.Pomodoro:
                time -= Time.deltaTime;
                if (time <= 0) HandlePomodoroCycle();
                break;
            case AppMode.Stopwatch:
                elapsed += Time.deltaTime;
                break;
            case AppMode.Countdown:
                time -= Time.deltaTime;
                if (time <= 0) isRunning = false;
                break;
        }
    }

    public void SetAppMode(AppMode mode)
    {
        CurrentMode = mode;
        ResetTimer();
    }


    public void ToggleRun() => isRunning = !isRunning;

    public void ResetTimer()
    {
        isRunning = false;
        elapsed = 0;
        Cycles = Mathf.Max(Cycles, 1);
        Phase = PomodoroPhase.Work;
        time = WorkDuration;
    }

    public void HandlePomodoroCycle()
    {
        isRunning = false;
        if (Phase == PomodoroPhase.Work)
        {
            if (Cycles > 1)
            {
                Cycles--;
                Phase = PomodoroPhase.ShortBreak;
                time = ShortBreakDuration;
            }
            // FIXME: 最后一个休息为长休息
            else
            {
                Phase = PomodoroPhase.LongBreak;
                time = LongBreakDuration;
            }
        }
        // 休息结束
        else
        {
            Phase = PomodoroPhase.Work;
            time = WorkDuration;
        }
    }

    // 参数设置接口
    public void SetWorkDuration(float v) => WorkDuration = v;
    public void SetShortBreakDuration(float v) => ShortBreakDuration = v;
    public void SetLongBreakDuration(float v) => LongBreakDuration = v;
    public void SetCycleCount(int v) => Cycles = v;

    // 数据获取接口
    public string GetFormattedTime()
    {
        float display = (CurrentMode == AppMode.Stopwatch) ? elapsed : Mathf.Max(time, 0);
        int m = Mathf.FloorToInt(display / 60f);
        int s = Mathf.FloorToInt(display % 60f);
        return $"{m:00}:{s:00}";
    }

    public float GetProgress()
    {
        float total = Phase == PomodoroPhase.Work ? WorkDuration : (Phase == PomodoroPhase.ShortBreak ? ShortBreakDuration : LongBreakDuration);
        if (CurrentMode == AppMode.Stopwatch) return 0;
        if (CurrentMode == AppMode.Countdown) return 1 - (time / total);
        return 1 - (time / total);
    }

    public string GetPhaseName()
    {
        return Phase switch
        {
            PomodoroPhase.Work => "专注",
            PomodoroPhase.ShortBreak => "短休息",
            PomodoroPhase.LongBreak => "长休息",
            _ => "",
        };
    }
}