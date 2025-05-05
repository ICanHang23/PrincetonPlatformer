using UnityEngine;
using System.Collections.Generic;

public class HoagieBuns : MonoBehaviour
{
    Collider2D bunsCollider;
    public Callback updateDeploy;
    public bool ghost = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bunsCollider = GetComponent<BoxCollider2D>();
        checkIntegrity();
    }

    // Update is called once per frame
    void Update()
    {
        // To check if fell of edge
        if (transform.position.y < -15)
        {
            disable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (ghost)
        {
            player = GameObject.FindGameObjectWithTag("Ghost");
        }

        string tag = collision.gameObject.tag;

        if (player != null && !tag.Equals("Player") && !tag.Equals("Ghost"))
        {
            teleport(player.transform);
        }
    }

    private void checkIntegrity()
    {
        int ground = 6;

        List<Collider2D> overlapList = new List<Collider2D>();
        bunsCollider.Overlap(overlapList);

        foreach (Collider2D collider in overlapList)
        {
            GameObject colliderObject = collider.gameObject;

            if (colliderObject.layer == ground)
            {
                disable();
            }
        }
        
    }

    public void launch(Vector2 direction)
    {
        Rigidbody2D bunBody = GetComponent<Rigidbody2D>();
        bunBody.linearVelocity = direction * 12;
    }

    public void teleport(Transform trans)
    {
        trans.position = transform.position;
        // playerScript.resetVelocity();
        disable();
    }

    public void disable()
    {
        if (updateDeploy != null)
        {
            updateDeploy();
        }
        Destroy(gameObject);
    }

    public void ghostIt()
    {
        ghost = true;
        GameObject meat = transform.GetChild(0).gameObject;
        SpriteRenderer spr = meat.GetComponent<SpriteRenderer>();
        spr.color = Color.cyan;
    }
}
