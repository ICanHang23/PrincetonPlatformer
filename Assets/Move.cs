using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D box;
    private CapsuleCollider2D hurtbox;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameData game;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        box = GetComponent<BoxCollider2D>();
        hurtbox = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (checkIfCanMove())
        {
            if(isGrounded())
            {
                body.linearVelocityX = (5 * Input.GetAxis("Horizontal"));
            } else {
                body.AddForceX(15 * Input.GetAxis("Horizontal"));
            }           
        }
        
        if (jumpInput() && isGrounded() && !game.reachedGoal)
        {
            body.linearVelocityY = 7;
        }

        if (transform.position.y < -15)
        {
            Vector2 newPosition = new Vector2(-5, 0);
            transform.position = newPosition;
            body.linearVelocity = Vector2.zero;
            game.deathCount++;
        }
    }

    bool checkIfCanMove()
    {
        float inputAxis = Input.GetAxis("Horizontal");
        float xVelocity = body.linearVelocityX;

        bool notTooFast = Mathf.Abs(xVelocity) < 5 || Mathf.Sign(inputAxis) != Mathf.Sign(xVelocity);
        bool notWalled = !isWalled(new Vector2(inputAxis, 0));
        bool actuallyPressed = inputAxis != 0;
        bool completed = game.reachedGoal;

        Debug.Log(completed);

        return notTooFast && notWalled && actuallyPressed && !completed;
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
