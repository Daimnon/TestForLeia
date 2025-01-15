using UnityEngine;
using System;

public static class EventSystem
{
    public static Action<BallType> OnGetBallFromPool;
    public static Action<Ball> OnReturnBallToPool;

    public static void InvokeGetBallFromPool(BallType ballType)
    {
        Debugger.Log($"Invoke: GetBallFromPool {ballType}");
        OnGetBallFromPool?.Invoke(ballType);
    }
    public static void InvokeReturnBallToPool(Ball ball)
    {
        if (!ball) return;
        Debugger.Log($"Invoke: ReturnBallToPool {ball.BallType}");
        OnReturnBallToPool?.Invoke(ball);
    }
}
