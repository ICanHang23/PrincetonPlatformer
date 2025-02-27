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
        // Change x velocity
        float direction = Input.GetAxis("Horizontal");
        Vector2 directionVector = new Vector2(direction, 0);
        if (direction != 0 && !isWalled(directionVector))
        {
            body.linearVelocity = new Vector2(direction * 5, body.linearVelocity.y);
        }

        // Change y velocity for jumping
        if (jumpInput() && isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, 5);
        }
    }

    bool jumpInput()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
    }

    bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    bool isWalled(Vector2 direction)
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, direction, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
