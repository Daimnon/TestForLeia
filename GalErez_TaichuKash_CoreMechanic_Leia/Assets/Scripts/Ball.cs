using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] BallType _ballType;
    public BallType @BallType => _ballType;
}
