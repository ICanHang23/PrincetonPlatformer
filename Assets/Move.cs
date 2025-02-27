using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D box;
    [SerializeField] private LayerMask groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkIfCanMove())
        {
            body.AddForceX(15 * Input.GetAxis("Horizontal"));
        }
        
        if (jumpInput() && isGrounded())
        {
            body.linearVelocityY = 7;
        }

        if (transform.position.y < -15)
        {
            Vector2 newPosition = new Vector2(-5, 0);
            body.MovePosition(newPosition);
            body.linearVelocity = Vector2.zero;
        }
    }

    bool checkIfCanMove()
    {
        float inputAxis = Input.GetAxis("Horizontal");
        float xVelocity = body.linearVelocityX;

        bool notTooFast = Mathf.Abs(xVelocity) < 5 || Mathf.Sign(inputAxis) != Mathf.Sign(xVelocity);
        bool notWalled = !isWalled(new Vector2(inputAxis, 0));
        bool actuallyPressed = inputAxis != 0;

        return notTooFast && notWalled && actuallyPressed;
    }

    bool jumpInput()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
    }

    bool isGrounded()
    {
        Vector2 boxSize = box.bounds.size;
        Vector2 sizeVector = new Vector2(boxSize.x * 0.5f, boxSize.y);

        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, sizeVector, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    bool isWalled(Vector2 direction)
    {
        Vector2 boxSize = box.bounds.size;
        Vector2 sizeVector = new Vector2(boxSize.x, boxSize.y * 0.5f);

        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, sizeVector, 0, direction, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
