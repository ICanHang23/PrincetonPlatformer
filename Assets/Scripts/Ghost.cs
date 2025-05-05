using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UIElements;


// public delegate void Callback();
public class Ghost : MonoBehaviour
{
    // Components and other things
    protected Rigidbody2D body;
    protected BoxCollider2D box;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected GameData game;

    protected bool doubleJumped = false;
    protected int wallJumpCount = 0;
    protected bool dead = false;

    [SerializeField] float gyatt = 0.9f;
    int debug = 0;

    // for hoagie
    [SerializeField] GameObject hoagie;
    HoagieBuns thisHoagie;
    int directionFacing = 1;
    bool deployed = false;
    protected Arrow arrow;

    // pausing
    protected bool paused = false;

    // controls
    public PlayerInput currentInput;
    InputLogger.InputDiary diary;

    // fixed update stuff
    public int phyFrame = 0;
    public int nextPFrame = 0;
    public int entryIndex = 0;

    // respawn
    public Vector2 respawnLocation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        box = GetComponent<BoxCollider2D>();
        GameObject arrowObj = transform.GetChild(0).gameObject;
        arrow = arrowObj.GetComponent<Arrow>();

        currentInput = new PlayerInput();
        diary = JsonConvert.DeserializeObject<InputLogger.InputDiary>(game.ghostDiary);
        // Debug.Log("Dictionary size: " + diary.list.Count);

        respawnLocation = new Vector2(-5, 0);
    }

    void FixedUpdate()
    {
        if (diary.checkifTime(entryIndex, phyFrame))
        {
            diary.getEntry(entryIndex, currentInput);
            currentInput.phyProcessed = false;
            entryIndex++;
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

    protected void move()
    {
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


        if (gameStateCriteria && groundedNow && jump)
        {
            body.linearVelocityY = 10;
        }

        // For walljumps
        else if (justJumped && inputAxis != 0 && isWalled(inputVector)
            && wallJumpCount < 5 && !processed && gameStateCriteria)
        {
            wallJumpCount++;
            body.linearVelocityX = -16 * inputVector.x;
            body.linearVelocityY = 11;
        }

        
        // For double jumping
        else if (justJumped && !doubleJumped && !processed && gameStateCriteria)
        {
            // Debug.Log("Double jumped");

            if (inputAxis != 0)
            {
                body.AddForceX(50 * inputAxis);
                body.linearVelocityX += 4 * inputVector.x;
            }

            body.linearVelocityY = 8;
            doubleJumped = true;
        }
    }

    protected void handleHoagieDeployment()
    {
        // Check for hoagie deployment
        if ((currentInput.justF || currentInput.justClicked) && !currentInput.phyProcessed)
        {
            if (!deployed && !dead)
            {
                deployHoagie();
                deployed = true;
            }
            else if (!dead)
            {
                thisHoagie.teleport(gameObject.transform);
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

            if (thisHoagie != null)
            {
                thisHoagie.disable();
            }

            StartCoroutine(RespawnPlayer());
        }
    }
    protected virtual IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1);
        resetVelocity();
        transform.position = respawnLocation;
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
        thisHoagie = newHoagie.GetComponent<HoagieBuns>();

        if (!(this is Player))
        {
            thisHoagie.ghostIt();
        }

        thisHoagie.launch(trajectory);

        // Set the undeploy function
        thisHoagie.updateDeploy = updateDeploy;
    }

    void updateDeploy()
    {
        deployed = false;
        thisHoagie = null;
    }
}
