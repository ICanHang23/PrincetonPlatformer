using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * 5, body.velocity.y);
    }
}
