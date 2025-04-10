using UnityEngine;
using System;
using System.Collections;


public delegate void Callback();
public class Move : MonoBehaviour
{
    private Rigidbody2D body;
    private BoxCollider2D box;
    private bool doubleJumped = false;
    private int wallJumpCount = 0;
    private bool dead = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameData game;
    [SerializeField] float gyatt = 0.9f;
    int debug = 0;

    // for hoagie
    [SerializeField] GameObject hoagie;
    int directionFacing = 1;
    bool deployed = false;
    Arrow arrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        box = GetComponent<BoxCollider2D>();
        // Debug.Log(transform.childCount);
        GameObject arrowObj = transform.GetChild(0).gameObject;
        arrow = arrowObj.GetComponent<Arrow>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updating some variables that will be reused throughout
        float inputAxis = Input.GetAxis("Horizontal");
        Vector2 inputVector = new Vector2(inputAxis, 0);
        bool groundedNow = isGrounded();
        if (inputAxis != 0)
        {
            directionFacing = Math.Sign(inputAxis);
            arrow.dir(directionFacing);
        }

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
        if (transform.position.y < -15 && !dead)
        {
            die();
        }

        // Check for hoagie deployment
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (!deployed)
            {
                deployHoagie();
                deployed = true;
            }
            else
            {
                GameObject hoagie = GameObject.FindGameObjectWithTag("HoagieBun");
                HoagieBuns buns = hoagie.GetComponent<HoagieBuns>();
                buns.teleport(gameObject);
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            arrow.toggleVisibily(true);
            arrow.rotate(1);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            arrow.toggleVisibily(true);
            arrow.rotate(-1);
        }
        else
        {
            arrow.toggleVisibily(false);
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
        if (!dead) 
        {
            dead = true;
            GetComponent<Rigidbody2D>().gravityScale=0;
            StartCoroutine(RespawnPlayer());
        }
    }
    private IEnumerator RespawnPlayer()
        {
            yield return new WaitForSeconds(1);
            Vector2 newPosition = new Vector2(-5, 0);
            resetVelocity();
            transform.position = newPosition;
            GetComponent<Rigidbody2D>().gravityScale=2;
            game.deathCount++;
            dead = false;
        }

    public void resetVelocity()
    {
        body.linearVelocity = Vector2.zero;
    }

    void deployHoagie()
    {
        // Spawn the hoagie
        Vector3 hoagiePosition = transform.position;
        hoagiePosition.y += 0.1f;
        GameObject newHoagie = Instantiate(hoagie, hoagiePosition, Quaternion.identity);

        // Calculate the launch angle
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        double angle = arrowScript.angle * (Math.PI / 180);
        Vector2 trajectory = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
        trajectory.x *= directionFacing;


        // Launch the thing
        HoagieBuns buns = newHoagie.GetComponent<HoagieBuns>();
        buns.launch(trajectory);

        // Set the undeploy function
        buns.updateDeploy = updateDeploy;
    }

    void updateDeploy()
    {
        deployed = false;
    }
}
