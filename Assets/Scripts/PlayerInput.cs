using System;

public class PlayerInput
{
    // basic movement
    public bool jump = false;
    public bool justJumped = false;
    public float moveDirection = 0;

    // scroll wheel
    public float scrollDelta = 0;

    // mouse buttons
    public bool justClicked = false;
    public bool holdingRightClick = false;

    // updated by fixedinput
    public bool phyProcessed = false;

    // keyboard hoagie controls
    public bool justF = false;
    public bool Q = false;
    public bool E = false;
    public bool R = false;
}
