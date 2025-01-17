using UnityEngine;
using System;

public static class EventSystem
{
    #region GameLoop
    public static Action<bool> OnPause;
    public static void InvokePause(bool isPaused)
    {
        Debugger.Log($"Invoke: Pause.");
        OnPause?.Invoke(isPaused);
    }
    #endregion

    #region MergeBalls
    public static Action<BallType, Ball, Ball> OnMergeBalls;
    public static void InvokeMergeBalls(BallType ballType, Ball ball, Ball otherBall)
    {
        Debugger.Log($"Invoke: MergeBalls {ballType}, created {(BallType)(int)ballType+1}");
        OnMergeBalls?.Invoke(ballType, ball, otherBall);
    }
    #endregion

    #region Score
    public static Action<BallType> OnScore;
    public static void InvokeScore(BallType ballType)
    {
        Debugger.Log($"Invoke: Score {ballType}.");
        OnScore?.Invoke(ballType);
    }
    #endregion

    #region Combos
    public static Action OnCombo;
    public static void InvokeCombo()
    {
        Debugger.Log($"Invoke: Combo.");
        OnCombo?.Invoke();
    }
    #endregion

    #region Death
    public static Action OnTriggeredEnterDeathTimerStart; 
    public static Action OnTriggeredExitDeathTimerStop;
    public static void InvokeTriggeredEnterDeathTimerStart()
    {
        Debugger.Log($"Invoke: TriggeredEnterDeathTimerStart.");
        OnTriggeredEnterDeathTimerStart?.Invoke();
    }
    public static void InvokeTriggeredExitDeathTimerStop()
    {
        Debugger.Log($"Invoke: TriggeredExitDeathTimerStop.");
        OnTriggeredExitDeathTimerStop?.Invoke();
    }
    #endregion
}
