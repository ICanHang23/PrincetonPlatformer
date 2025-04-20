using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;


// public delegate void Callback();
public class Ghost : MonoBehaviour
{

    // Components and other things
    private Rigidbody2D body;
    private BoxCollider2D box;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameData game;

    private bool jumped = false;
    private bool doubleJumped = false;
    private int wallJumpCount = 0;
    private bool dead = false;

    [SerializeField] float gyatt = 0.9f;
    int debug = 0;

    // for hoagie
    [SerializeField] GameObject hoagie;
    int directionFacing = 1;
    bool deployed = false;
    Arrow arrow;

    // pausing
    bool paused = false;

    // controls
    public PlayerInput currentInput;
    InputLogger.InputDiary diary;

    // fixed update stuff
    public int phyFrame = 0;
    public int nextPFrame = 0;
    public int entryIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        box = GetComponent<BoxCollider2D>();
        GameObject arrowObj = transform.GetChild(0).gameObject;
        arrow = arrowObj.GetComponent<Arrow>();

        currentInput = new PlayerInput();
        diary = JsonUtility.FromJson<InputLogger.InputDiary>(game.ghostDiary);
        Debug.Log("Dictionary size: " + diary.list.Count);
    }

    void FixedUpdate()
    {
        if (diary.checkifTime(entryIndex, phyFrame))
        {
            diary.getEntry(entryIndex, currentInput);
            currentInput.phyProcessed = false;
            entryIndex++;
            // Debug.Log("Now printing diary index " + entryIndex);
        }

        move();

        // To check for death
        if (transform.position.y < -15 && !dead)
        {
            die();
        }

        if (!paused)
        {
            handleHoagieDeployment();
        }

        currentInput.phyProcessed = true;
        phyFrame++;
    }

    void move()
    {
        // Set presumed statuses
        jumped = false;

        // Updating some variables that will be reused throughout
        float inputAxis = currentInput.moveDirection;

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
            if (groundedNow)
            {
                body.linearVelocityX = (7 * inputAxis);
            }
            else
            {
                body.AddForceX(80 * inputAxis);
            }
        }

        // For jumping
        bool gameStateCriteria = !game.reachedGoal && !dead;
        bool jump = currentInput.jump || currentInput.justJumped;
        bool justJumped = currentInput.justJumped;
        bool processed = currentInput.phyProcessed;

        if (currentInput.justJumped)
        {
            Debug.Log(justJumped + " " + !doubleJumped + " " + !processed);
            Debug.Log("Jump conditions: " + gameStateCriteria + " " + groundedNow + " " + jump);
        }


        if (gameStateCriteria && groundedNow && jump)
        {
            body.linearVelocityY = 10;
            jumped = true;
        }

        // For walljumps
        else if (justJumped && inputAxis != 0 && isWalled(inputVector)
            && wallJumpCount < 5 && !processed)
        {
            wallJumpCount++;
            body.linearVelocityX = -16 * inputVector.x;
            body.linearVelocityY = 10;
        }

        
        // For double jumping
        else if (justJumped && !doubleJumped && !processed)
        {
            Debug.Log("Double jumped");

            if (inputAxis != 0)
            {
                body.AddForceX(50 * inputAxis);
            }

            body.linearVelocityY = 8;
            doubleJumped = true;
        }
    }

    void handleHoagieDeployment()
    {
        // Check for hoagie deployment
        if ((currentInput.justF || currentInput.justClicked) && !currentInput.phyProcessed)
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
                buns.teleport(gameObject.transform);
            }
        }

        // Check for hoagie angle change
        if (currentInput.E)
        {
            arrow.rotate(0.5);
            arrow.resetVisibily();
        }
        else if (currentInput.R)
        {
            arrow.rotate(-0.5);
            arrow.resetVisibily();
        }
        // this is for scroll wheel
        else if (currentInput.scrollDelta > 0)
        {
            arrow.rotate(2);
            arrow.resetVisibily();
        }
        else if (currentInput.scrollDelta < 0)
        {
            arrow.rotate(-2);
            arrow.resetVisibily();
        }

        // Reset visibility
        if (currentInput.Q || currentInput.holdingRightClick)
        {
            arrow.resetVisibily();
        }
    }

    bool checkIfCanMove()
    {
        float inputAxis = currentInput.moveDirection;
        float xVelocity = body.linearVelocityX;

        bool notTooFast = Mathf.Abs(xVelocity) < 5 || Mathf.Sign(inputAxis) != Mathf.Sign(xVelocity);
        bool notWalled = !isWalled(new Vector2(inputAxis, 0));
        bool actuallyPressed = inputAxis != 0;
        bool completed = game.reachedGoal;

        return notTooFast && notWalled && actuallyPressed && !completed && !dead;
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
            resetVelocity();
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
        buns.ghost = true;
        buns.launch(trajectory);

        // Set the undeploy function
        buns.updateDeploy = updateDeploy;
    }

    void updateDeploy()
    {
        deployed = false;
    }
    private IEnumerator UnfreezePlayer()
    {
        yield return new WaitForSeconds(1);
        Vector2 newPosition = new Vector2(-5, 0);
        body.linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale=2;
        transform.position = newPosition;
        dead = false;
        game.deathCount++;
    }
}
