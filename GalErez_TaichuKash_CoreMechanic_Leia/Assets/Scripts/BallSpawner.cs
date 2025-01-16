using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class BallSpawner : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BallPooler _ballPooler;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Locations")]
    [SerializeField] private Transform _spawnerSpriteTr;
    [SerializeField] private Transform _currentBallTr;
    [SerializeField] private Transform _nextBallTr;
    
    [SerializeField] private float _throwForce;
    [SerializeField] private float _delayBetweenThrows = 0.8f;
    [SerializeField] private float _throwPreparationTime = 0.4f;

    private Ball _currentBall = null;
    private Ball _nextBall = null;

    private Coroutine _prepareToThrow;
    private bool _isThrowable = true;

    private void OnEnable()
    {
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += OnFingerDown;
        ETouch.Touch.onFingerMove += OnFingerMove;
        ETouch.Touch.onFingerUp += OnFingerUp;
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= OnFingerDown;
        ETouch.Touch.onFingerMove -= OnFingerMove;
        ETouch.Touch.onFingerUp -= OnFingerUp;
        ETouch.EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        InitSpawner();
    }

    private void InitSpawner()
    {
        _currentBall = _ballPooler.GetFromPool(BallType.Fire, _currentBallTr, true);
        _currentBall.RB2D.simulated = false;
        _nextBall = _ballPooler.GetFromPool(BallType.Fire, _nextBallTr, true);
        _nextBall.RB2D.simulated = false;
    }
    private IEnumerator PrepareToThrowRoutine(float duration)
    {
        Ball ball = _currentBall; // so if the current ball is switched to the new ball it won't be lost
        float time = 0;

        while (time < duration)
        {
            ball.transform.position = Vector3.Lerp(ball.transform.position, _spawnerSpriteTr.position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ball.transform.position = _spawnerSpriteTr.position;

    }
    private IEnumerator ChangeBallsRoutine()
    {
        // change balls
        _currentBall = _nextBall;
        _currentBall.transform.SetParent(_currentBallTr);
        _currentBall.transform.localPosition = Vector2.zero;

        // get new nextBall
        _nextBall = _ballPooler.GetFromPool(BallType.Fire, _nextBallTr, true);
        _nextBall.RB2D.simulated = false;

        yield return new WaitForSeconds(_delayBetweenThrows);
        _isThrowable = true;
    }


    private void OnFingerDown(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        transform.position = fingerWorldPos;
        _prepareToThrow = StartCoroutine(PrepareToThrowRoutine(0.4f));
    }
    private void OnFingerMove(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        transform.position = fingerWorldPos;
    }
    private void OnFingerUp(ETouch.Finger finger)
    {
        if (!_isThrowable) return;
        if (_prepareToThrow != null) StopCoroutine(_prepareToThrow);

        // throw current ball
        _currentBall.RB2D.simulated = true;
        _currentBall.transform.SetParent(null);
        _currentBall.RB2D.AddForce(_throwForce * Vector2.up, ForceMode2D.Impulse);
        _currentBall = null;
        _isThrowable = false;

        StartCoroutine(ChangeBallsRoutine()); // not track cause no need for now
    }
}
