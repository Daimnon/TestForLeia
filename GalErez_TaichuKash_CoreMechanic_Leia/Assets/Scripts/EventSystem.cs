using UnityEngine;
using System;
using UnityEditorInternal;

public static class EventSystem
{
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
}
