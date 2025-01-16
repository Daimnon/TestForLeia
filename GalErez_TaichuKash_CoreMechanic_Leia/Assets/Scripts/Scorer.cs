using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scorer : MonoBehaviour
{
    private const string BALL_TAG = "Ball";
    private int[] _scoreByEnum;
    public int Score { get; set; }

    [SerializeField] private TextMeshProUGUI _scoreTextTMP;

    private void Start()
    {
        _scoreByEnum = new int[System.Enum.GetValues(typeof(BallType)).Length];
        for (int i = 0; i < _scoreByEnum.Length; i++)
        {
            _scoreByEnum[i] = i;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BALL_TAG))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball.WasScored) return;

            Score += _scoreByEnum[(int)ball.BallType];
            _scoreTextTMP.text = Score.ToString();

            ball.WasScored = true;
        }
    }
}
