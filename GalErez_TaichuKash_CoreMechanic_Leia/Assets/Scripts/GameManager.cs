using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float _levelTime = 300.0f;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverCanvas;

    private float _timeRemaining;
    private void Awake()
    {
        _timeRemaining = _levelTime;
    }
    private void Update()
    {
        if (_timeRemaining > 0)
        {
            DecreaseTime();
        }
    }

    private void UpdateTimerText(float timeSinceStartup)
    {
        int minutes = Mathf.FloorToInt(timeSinceStartup / 60) % 60;
        int seconds = Mathf.FloorToInt(timeSinceStartup) % 60;

        _timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
    private void DecreaseTime()
    {
        _timeRemaining -= Time.deltaTime;
        UpdateTimerText(_timeRemaining);

        if (_timeRemaining <= 0)
        {
            GameOver();
        }
    }
    private void GameOver()
    {
        _gameOverCanvas.SetActive(true);
    }
}
