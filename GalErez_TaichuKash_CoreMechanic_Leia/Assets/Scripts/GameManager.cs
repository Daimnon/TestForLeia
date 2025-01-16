using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BallPooler _ballPooler;

    [Header("Game Timer")]
    [SerializeField] private float _levelTime = 300.0f;
    [SerializeField] private TextMeshProUGUI _levelTimerText;
    private float _timeRemaining;

    [Header("Combo")]
    [SerializeField] private float _comboTime = 0.6f;
    [SerializeField] private float _comboTimer = 0.0f;
    private Coroutine _comboHandler = null;
    private bool _isComboActive = false;

    [Header("DeathTimer")]
    [SerializeField] private float _deathTime = 1.2f;
    [SerializeField] private float _deathTimer = 0.0f;
    private Coroutine _deathTimerHandler = null;
    private bool _isDeathTimerActive = false;

    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverCanvas;

    private bool _isPaused = false;

    private void Awake()
    {
        _timeRemaining = _levelTime;
        _comboTimer = _comboTime;
        _deathTimer = _deathTime;
    }
    private void OnEnable()
    {
        EventSystem.OnMergeBalls += OnMergeBalls;
        EventSystem.OnCombo += OnCombo;
        EventSystem.OnTriggeredEnterDeathTimerStart += OnTriggeredEnterDeathTimerStart;
        EventSystem.OnTriggeredExitDeathTimerStop += OnTriggeredExitDeathTimerStop;
        EventSystem.OnPause += OnPause;
    }
    private void OnDisable()
    {
        EventSystem.OnMergeBalls -= OnMergeBalls;
        EventSystem.OnCombo -= OnCombo;
        EventSystem.OnTriggeredEnterDeathTimerStart -= OnTriggeredEnterDeathTimerStart;
        EventSystem.OnTriggeredExitDeathTimerStop -= OnTriggeredExitDeathTimerStop;
        EventSystem.OnPause -= OnPause;
    }
    private void Update()
    {
        if (!_isPaused && _timeRemaining > 0)
        {
            DecreaseTime();
        }
    }

    private void UpdateTimerText(float timeSinceStartup)
    {
        int minutes = Mathf.FloorToInt(timeSinceStartup / 60) % 60;
        int seconds = Mathf.FloorToInt(timeSinceStartup) % 60;

        _levelTimerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
    private void DecreaseTime()
    {
        _timeRemaining -= Time.deltaTime;
        UpdateTimerText(_timeRemaining);

        if (_timeRemaining <= 0)
        {
            ResetDeathTimer();
            GameOver();
        }
    }
    private IEnumerator HandleCombo()
    {
        _isComboActive = true;
        while (_isComboActive)
        {
            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0)
            {
                _comboTimer = _comboTime;
                _isComboActive = false;
            }
            yield return null;
        }
    }
    private IEnumerator HandleDeathTimer()
    {
        _isDeathTimerActive = true;
        while (_isDeathTimerActive)
        {
            _deathTimer -= Time.deltaTime;
            if (_deathTimer <= 0) GameOver();

            yield return null;
        }
    }
    private void GameOver()
    {
        _gameOverCanvas.SetActive(true);
    }
    private void ResetDeathTimer()
    {
        _deathTimer = _deathTime;
        _isDeathTimerActive = false;
        _deathTimerHandler = null;
    }

    #region Events
    private void OnMergeBalls(BallType ballType, Ball ball, Ball otherBall) // wanted maybe to split it into two events but I think I won't
    {
        Debugger.Log("Merged by pool");
        int newBallTypeValue = (int)ballType + 1;
        _ballPooler.ReturnToPool(otherBall);
        Ball newBall = _ballPooler.GetFromPool((BallType)newBallTypeValue, null, true);
        newBall.transform.position = ball.transform.position;
        _ballPooler.ReturnToPool(ball);
        EventSystem.InvokeScore((BallType)newBallTypeValue); // apply score of new ball

        if (_isComboActive) EventSystem.InvokeScore((BallType)newBallTypeValue);
    }
    private void OnCombo()
    {
        if (_comboHandler == null)
            _comboHandler = StartCoroutine(HandleCombo());
    }
    private void OnTriggeredEnterDeathTimerStart()
    {
        _deathTimerHandler ??= StartCoroutine(HandleDeathTimer());
        
    }
    private void OnTriggeredExitDeathTimerStop()
    {
        if (_deathTimerHandler != null)
        {
            StopCoroutine(_deathTimerHandler);
            ResetDeathTimer();
        }
    }
    private void OnPause(bool isPaused)
    {
        _isPaused = isPaused;
    }
    #endregion
}
