using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{
    private const string BALL_TAG = "Ball";
    private List<Ball> ballsAtRisk = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BALL_TAG))
        {
            ballsAtRisk.Add(collision.gameObject.GetComponent<Ball>());
            EventSystem.InvokeTriggeredEnterDeathTimerStart();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BALL_TAG))
        {
            ballsAtRisk.Remove(collision.gameObject.GetComponent<Ball>());
            EventSystem.InvokeTriggeredExitDeathTimerStop();
        }
    }
}
