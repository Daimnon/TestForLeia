using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class BallSpawner : MonoBehaviour
{
    private ETouch.Finger _moveFinger;

    [Header("Components")]
    [SerializeField] private BallPooler _ballPooler;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Locations")]
    [SerializeField] private Transform _spawnerSpriteTr;
    [SerializeField] private Transform _currentBallTr;
    [SerializeField] private Transform _nextBallTr;
    
    [SerializeField] private float _throwForce;

    private Ball _currentBall = null;
    private Ball _nextBall = null;

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
    private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = _currentBall.transform.position;

        while (time < duration)
        {
            _currentBall.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _currentBall.transform.position = targetPosition;
    }

    private void OnFingerDown(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        transform.position = fingerWorldPos;
        StartCoroutine(LerpPosition(_spawnerSpriteTr.position, 0.4f));
    }
    private void OnFingerMove(ETouch.Finger finger)
    {
        Vector2 fingerWorldPos = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        transform.position = fingerWorldPos;
    }
    private void OnFingerUp(ETouch.Finger finger)
    {
        // throw current ball
        _currentBall.RB2D.simulated = true;
        _currentBall.RB2D.AddForce(_throwForce * Vector2.up, ForceMode2D.Impulse);

        // change balls
        _currentBall = _nextBall;
        _currentBall.RB2D.simulated = false;
        _currentBall.transform.SetParent(_currentBallTr);
        _currentBall.transform.position = Vector2.zero;

        // get new nextBall
        _nextBall = _ballPooler.GetFromPool(BallType.Fire, _nextBallTr, true);
        _nextBall.RB2D.simulated = false;
    }
}
