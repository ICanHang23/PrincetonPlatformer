using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;


public delegate void Callback();
public class Player : Ghost
{
    [SerializeField] GameObject pawseScreen;

    // fixed update stuff
    public PlayerInput previousInput;
    InputLogger inputLogger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();
        previousInput = new PlayerInput();
        inputLogger = new InputLogger(this);
    }

    private void Update()
    {
        // pausing
        if (Input.GetKeyDown(KeyCode.P))
        {
            pause();
        }

        // current input
        if (currentInput == null || currentInput.phyProcessed && !paused)
        {
            currentInput = new PlayerInput();
            currentInput.jump = jumpInput();
            currentInput.moveDirection = Input.GetAxis("Horizontal");
            currentInput.justJumped = jumpInput(true);


            currentInput.justClicked = Input.GetMouseButtonDown(0);
            currentInput.holdingRightClick = Input.GetMouseButton(1);
            currentInput.scrollDelta = Input.mouseScrollDelta.y;

            currentInput.Q = Input.GetKey(KeyCode.Q);
            currentInput.E = Input.GetKey(KeyCode.E);
            currentInput.R = Input.GetKey(KeyCode.R);
            currentInput.justF = Input.GetKeyUp(KeyCode.F);
        }
        else if (!paused)
        {
            if (jumpInput(true))
            {
                currentInput.justJumped = jumpInput(true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                currentInput.justClicked = Input.GetMouseButtonDown(0);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                currentInput.justF = Input.GetKeyUp(KeyCode.F);
            }
        }
    }

    void FixedUpdate()
    {
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

        inputLogger.checkControllDiffs(phyFrame);

        currentInput.phyProcessed = true;
        previousInput = currentInput;

        phyFrame++;
    }

    bool jumpInput(bool precise = false)
    {
        if (precise)
        {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        }

        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
    }

    protected override IEnumerator RespawnPlayer()
    {
        yield return base.RespawnPlayer();
        game.addDeath();
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

    public void logInputs()
    {
        string json = inputLogger.json();
        game.playerDiary = json;
        Debug.Log(json);
    }
}
