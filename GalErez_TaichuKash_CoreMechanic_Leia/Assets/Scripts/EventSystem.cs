using UnityEngine;
using System;

public static class EventSystem
{
    public static Action<BallType, Ball, Ball> OnMergeBalls;

    public static void InvokeMergeBalls(BallType ballType, Ball ball, Ball otherBall)
    {
        Debugger.Log($"Invoke: MergeBalls {ballType}, created {(BallType)(int)ballType+1}");
        OnMergeBalls?.Invoke(ballType, ball, otherBall);
    }
}
