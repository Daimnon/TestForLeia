using UnityEngine;

public class Ball : MonoBehaviour
{
    private const string BALL_TAG = "Ball";

    public bool WasScored { get; set; }

    [SerializeField] BallType _ballType;
    public BallType @BallType => _ballType;

    [SerializeField] private Rigidbody2D _rb2D;
    public Rigidbody2D RB2D => _rb2D;

    //[SerializeField] private float _upwardPushForce = 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // because the gameObjects are identical I can just make sure one of them is not trigger if I compare the instance ID
        if (collision.gameObject.CompareTag(BALL_TAG) && GetInstanceID() < collision.gameObject.GetInstanceID()) 
        {
            Ball otherBall = collision.gameObject.GetComponent<Ball>();
            otherBall.enabled = false;

            if (otherBall.BallType == _ballType && (int)otherBall.BallType < System.Enum.GetValues(typeof(BallType)).Length)
            {
                EventSystem.InvokeMergeBalls(_ballType, this, otherBall);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BALL_TAG))
        {
            Rigidbody2D otherRigidbody = collision.rigidbody;

            if (otherRigidbody != null)
            {
                // Calculate the downward force to apply based on weightTransferForce
                Vector2 upwardForce = Vector2.up * /*_upwardPushForce*/ _rb2D.mass;
                otherRigidbody.AddForce(upwardForce, ForceMode2D.Force);
            }
        }
    }
}
