using UnityEngine;

public class Ball : MonoBehaviour
{
    private const string BALL_TAG = "Ball";

    public BallPooler @BallPooler { get; set; }

    [SerializeField] BallType _ballType;
    public BallType @BallType => _ballType;

    [SerializeField] private Rigidbody2D _rb2D;
    public Rigidbody2D RB2D => _rb2D;

    [SerializeField] private float _upwardPushForce = 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(BALL_TAG))
        {
            Ball otherBall = collision.gameObject.GetComponent<Ball>();
            if (otherBall.BallType == _ballType)
            {
                /*int newBallTypeValue = (int)_ballType + 1;
                this.BallPooler.ReturnToPool(otherBall);
                Ball newBall = this.BallPooler.GetFromPool((BallType)newBallTypeValue, null, true);
                newBall.transform.position = transform.position;
                this.BallPooler.ReturnToPool(this);*/
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
                Vector2 upwardForce = Vector2.up * _upwardPushForce;
                otherRigidbody.AddForce(upwardForce, ForceMode2D.Force);
            }
        }
    }
}
