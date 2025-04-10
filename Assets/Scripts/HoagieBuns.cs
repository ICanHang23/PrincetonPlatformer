using UnityEngine;
using System.Collections.Generic;

public class HoagieBuns : MonoBehaviour
{
    Collider2D bunsCollider;
    public Callback updateDeploy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        string tag = collision.gameObject.tag;

        if (player != null && !tag.Equals("Player"))
        {
            teleport(player);
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

    public void teleport(GameObject player)
    {
        Move playerScript = player.GetComponent<Move>();
        player.transform.position = transform.position;
        // playerScript.resetVelocity();
        disable();
    }

    void disable()
    {
        if (updateDeploy != null)
        {
            updateDeploy();
        }
        Destroy(gameObject);
    }
}
