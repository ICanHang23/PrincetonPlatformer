using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D box;
    private CapsuleCollider2D hurtbox;
    private bool doubleJumped = false;
    private int wallJumpCount = 0;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameData game;
    [SerializeField] float gyatt = 0.9f;
    int debug = 0;

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
        float inputAxis = Input.GetAxis("Horizontal");
        Vector2 inputVector = new Vector2(inputAxis, 0);
        bool groundedNow = isGrounded();

        // To check if double jump can be restored
        if (groundedNow)
        {
            doubleJumped = false;
            wallJumpCount = 0;
        }

        // For left or right movement
        if (checkIfCanMove())
        {
            if(groundedNow)
            {
                body.linearVelocityX = (7 * inputAxis);
            } else {
                body.AddForceX(10 * inputAxis);
            }           
        }

        // For jumping
        bool canDoubleJump = !groundedNow && !doubleJumped;

        if (jumpInput() && !game.reachedGoal && groundedNow)
        {
            body.linearVelocityY = 10;
        }

        // For walljumps
        else if (jumpInput(true) && inputAxis != 0 && isWalled(inputVector)
            && wallJumpCount < 5)
        {
            wallJumpCount++;
            body.linearVelocityX = -18 * inputVector.x;
            body.linearVelocityY = 10;
        }

        // For double jumping
        else if (jumpInput(true) && !game.reachedGoal && canDoubleJump)
        {
            if (inputAxis != 0)
            {
                body.AddForceX(200 * inputAxis);
            }

            body.linearVelocityY = 8;
            doubleJumped = true;
        }

        // To check for death
        if (transform.position.y < -15)
        {
            die();
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

        return notTooFast && notWalled && actuallyPressed && !completed;
    }

    bool jumpInput(bool precise = false)
    {
        if (precise)
        {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        }

        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
    }

    bool isGrounded()
    {
        Vector2 boxSize = box.bounds.size;
        Vector2 sizeVector = new Vector2(boxSize.x * gyatt, boxSize.y);

        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, sizeVector, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    bool isWalled(Vector2 direction)
    {
        Vector2 boxSize = box.bounds.size;
        Vector2 sizeVector = new Vector2(boxSize.x, boxSize.y * 0.95f);

        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, sizeVector, 0, direction, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public void die()
    {
        Vector2 newPosition = new Vector2(-5, 0);
        transform.position = newPosition;
        body.linearVelocity = Vector2.zero;
        game.deathCount++;
    }
}
