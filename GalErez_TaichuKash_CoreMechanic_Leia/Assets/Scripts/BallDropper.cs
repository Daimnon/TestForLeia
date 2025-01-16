using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class BallDropper : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BallPooler _ballPooler;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Locations")]
    [SerializeField] private Transform _ceilingTr;
    [SerializeField] private Transform _spawnerSpriteTr;
    [SerializeField] private Transform _currentBallTr;
    [SerializeField] private Transform _nextBallTr;
    
    [SerializeField] private float _throwForce;
    [SerializeField] private float _delayBetweenThrows = 0.8f;
    [SerializeField] private float _throwPreparationTime = 0.4f;

    [Header("Drop Range Constraints")]
    [SerializeField] private float _xConstraintNegative = -5f;
    [SerializeField] private float _xConstraintPositive = 5f;

    private Ball _currentBall = null;
    private Ball _nextBall = null;

    private Coroutine _prepareToThrow;
    private bool _isThrowable = true;

    // apply Enhanced Touch
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
        InitDropper();
    }

    private void InitDropper() // setuping the balls and making sure they won't fall (upwards) or collide
    {
        _currentBall = _ballPooler.GetFromPool(BallType.Fire, _currentBallTr, true);
        _currentBall.RB2D.simulated = false;
        _nextBall = _ballPooler.GetFromPool(BallType.Fire, _nextBallTr, true);
        _nextBall.RB2D.simulated = false;
    }

    private void UpdateLineRenderer(Transform tr)
    {
        _lineRenderer.SetPosition(0, tr.position);
        Vector3 endLinePos = tr.position;
        endLinePos.y = _ceilingTr.localPosition.y - _ceilingTr.localScale.y /2; // so it will always reach the bottom edge of the gameObject
        _lineRenderer.SetPosition(1, endLinePos);
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
        _currentBall = _nextBall;
        _currentBall.transform.SetParent(_currentBallTr);
        _currentBall.transform.localPosition = Vector2.zero;

        _nextBall = _ballPooler.GetFromPool(BallType.Fire, _nextBallTr, true);
        _nextBall.RB2D.simulated = false;

        yield return new WaitForSeconds(_delayBetweenThrows);
        _isThrowable = true;
    }

    private void OnFingerDown(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        if (fingerWorldPos.x < _xConstraintNegative || fingerWorldPos.x > _xConstraintPositive) return;

        transform.position = fingerWorldPos;
        _prepareToThrow = StartCoroutine(PrepareToThrowRoutine(_throwPreparationTime));
        _lineRenderer.enabled = true;
        UpdateLineRenderer(transform);
    }
    private void OnFingerMove(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        if (fingerWorldPos.x < _xConstraintNegative || fingerWorldPos.x > _xConstraintPositive) return;

        transform.position = fingerWorldPos;
        UpdateLineRenderer(transform);
    }
    private void OnFingerUp(ETouch.Finger finger)
    {
        if (!_isThrowable) return;
        if (_prepareToThrow != null) StopCoroutine(_prepareToThrow);

        _lineRenderer.enabled = false;
        // throw current ball
        _currentBall.RB2D.simulated = true;
        _currentBall.transform.SetParent(null);
        _currentBall.RB2D.AddForce(_throwForce * Vector2.up, ForceMode2D.Impulse);
        _currentBall = null;
        _isThrowable = false;

        StartCoroutine(ChangeBallsRoutine()); // not tracked cause no need for now
    }
}
