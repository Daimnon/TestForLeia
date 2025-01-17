using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    [SerializeField, Range(-300.0f, 300.0f)] private float _turnSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward, _turnSpeed * Time.deltaTime);
    }
}
