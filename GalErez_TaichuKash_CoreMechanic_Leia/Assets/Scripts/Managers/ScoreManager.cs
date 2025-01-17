using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int[] _scoreByEnum;
    public int Score { get; set; }

    [SerializeField] private TextMeshProUGUI _scoreTextTMP;

    private void OnEnable()
    {
        EventSystem.OnScore += OnScore;
    }
    private void OnDisable()
    {
        EventSystem.OnScore -= OnScore;
    }
    private void Start()
    {
        _scoreByEnum = new int[System.Enum.GetValues(typeof(BallType)).Length];
        for (int i = 0; i < _scoreByEnum.Length; i++)
        {
            _scoreByEnum[i] = i*2; // quickly make an arbirtary score calculation based on the enums
        }
    }

    private void OnScore(BallType type)
    {
        Debugger.Log("Scored by pool");
        Score += _scoreByEnum[(int)type];
        _scoreTextTMP.text = Score.ToString();
    }
}
