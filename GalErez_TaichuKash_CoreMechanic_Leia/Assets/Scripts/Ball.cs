using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] BallType _ballType;
    public BallType @BallType => _ballType;

    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _upwardPushForce = 1.0f;

    /*private void FixedUpdate()
    {
        _rb2D.AddForce(_upwardPushForce * Vector2.up, ForceMode2D.Force);
    }*/

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Rigidbody2D otherRigidbody = collision.rigidbody;

            if (otherRigidbody != null)
            {
                // Calculate the downward force to apply based on weightTransferForce
                Vector2 upwardForce = Vector2.up * _upwardPushForce;
                otherRigidbody.AddForce(upwardForce, ForceMode2D.Force);
            }
        }
    }
}
