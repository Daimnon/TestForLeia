using System.Collections.Generic;
using UnityEngine;

public enum BallType
{
    Rock,
    Star, // the ameba looking one
    Fire,
    Food,
    Plant,
    Water,
    Ice,
    Baby,
    Cloud,
    Pikachu, // the electric purple one
    Sengoku // the sun looking one
}

public class BallPooler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Ball[] _prefabs;

    [Header("Configuration for each pool")]
    [SerializeField] private int _defaultCapacity = 20;

    private Dictionary<BallType, List<Ball>> _ballPools;

    private void Awake()
    {
        InitPools();
    }

    private void InitPools() // lot's of debugs cause can see dictionary in inspector by default
    {
        _ballPools = new Dictionary<BallType, List<Ball>>();

        foreach (BallType type in System.Enum.GetValues(typeof(BallType)))
        {
            Debugger.Log($"Creating pool for {type}.");

            Ball prefab = _prefabs[(int)type];
            if (prefab == null)
            {
                Debugger.Log($"Prefab for {type} is missing.");
                continue;
            }

            List<Ball> pool = new(_defaultCapacity);
            for (int i = 0; i < _defaultCapacity; i++)
            {
                Ball newBall = Instantiate(prefab);
                newBall.gameObject.SetActive(false); // Deactivate initially
                pool.Add(newBall);
                Debugger.Log($"Created {type}.");
            }

            _ballPools[type] = pool;
        }
    }

    /// <summary>
    /// Automatically sets the ball's position according to the parent.
    /// </summary>
    /// <returns>Ball from pool</returns>
    public Ball GetFromPool(BallType type, Transform newParent, bool isActive)
    {
        if (_ballPools.TryGetValue(type, out var pool) && pool.Count > 0)
        {
            Ball ball = pool[0];
            ball.transform.SetParent(newParent);
            ball.transform.localPosition = Vector2.zero;
            ball.gameObject.SetActive(isActive);
            pool.RemoveAt(0);
            return ball;
        }

        Debug.LogWarning($"No ball available in the pool for BallType {type}");
        return null;
    }

    /// <summary>
    /// Automatically sets the ball's parent as the pool, sets it's position to zero and set it's active state to false.
    /// </summary>
    public void ReturnToPool(Ball ball)
    {
        if (_ballPools.TryGetValue(ball.BallType, out List<Ball> pool))
        {
            ball.transform.SetParent(transform);
            ball.transform.localPosition = Vector2.zero;
            ball.gameObject.SetActive(false);
            pool.Add(ball);
        }
        else
        {
            Debug.LogWarning($"No pool found for BallType {ball.BallType}");
        }
    }

    
}
