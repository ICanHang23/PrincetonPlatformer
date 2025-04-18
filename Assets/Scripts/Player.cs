using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UIElements;


public delegate void Callback();
public class Player : MonoBehaviour
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

    // pausing
    bool paused = false;
    GameObject pawseScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        box = GetComponent<BoxCollider2D>();
        GameObject arrowObj = transform.GetChild(0).gameObject;
        arrow = arrowObj.GetComponent<Arrow>();
    }

    private void Start()
    {
        pawseScreen = GameObject.Find("PauseScreen");
    }

    // Update is called once per frame
    void Update()
    {
        // pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
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
    }

    void move()
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
            if (groundedNow)
            {
                body.linearVelocityX = (7 * inputAxis);
            }
            else
            {
                body.AddForceX(8000 * inputAxis * Time.deltaTime);
            }
        }

        // For jumping
        bool canDoubleJump = !groundedNow && !doubleJumped && !dead;

        if (jumpInput() && !game.reachedGoal && groundedNow && !dead)
        {
            body.linearVelocityY = 10;
        }

        // For walljumps
        else if (jumpInput(true) && inputAxis != 0 && isWalled(inputVector)
            && wallJumpCount < 5)
        {
            wallJumpCount++;
            body.linearVelocityX = -16 * inputVector.x;
            body.linearVelocityY = 10;
        }

        // For double jumping
        else if (jumpInput(true) && !game.reachedGoal && canDoubleJump)
        {
            if (inputAxis != 0)
            {
                body.AddForceX(5000 * inputAxis * Time.deltaTime);
            }

            body.linearVelocityY = 8;
            doubleJumped = true;
        }
    }

    void handleHoagieDeployment()
    {
        // Check for hoagie deployment
        if (Input.GetKeyUp(KeyCode.F) || Input.GetMouseButtonDown(0))
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

        Vector2 scroll = Input.mouseScrollDelta;

        // Check for hoagie angle change
        if (Input.GetKey(KeyCode.E))
        {
            arrow.rotate(0.5);
            arrow.resetVisibily();
        }
        else if (Input.GetKey(KeyCode.R))
        {
            arrow.rotate(-0.5);
            arrow.resetVisibily();
        }
        // this is for scroll wheel
        else if (scroll.y > 0)
        {
            arrow.rotate(2);
            arrow.resetVisibily();
        }
        else if (scroll.y < 0)
        {
            arrow.rotate(-2);
            arrow.resetVisibily();
        }

        // Reset visibility
        if (Input.GetKey(KeyCode.Q) || Input.GetMouseButton(1))
        {
            arrow.resetVisibily();
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

        return notTooFast && notWalled && actuallyPressed && !completed && !dead;
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
            game.addDeath();
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

    public void pause()
    {
        if (paused)
        {
            Time.timeScale = 1;
            pawseScreen.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            pawseScreen.SetActive(true);
        }

        paused = !paused;
        
    }
}
