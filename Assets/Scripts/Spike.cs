using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Player") && other.collider is BoxCollider2D)
        {
            Player playerscript = obj.GetComponent<Player>();
            playerscript.die();
        }
        else if (obj.tag.Equals("Ghost") && other.collider is BoxCollider2D)
        {
            Ghost ghostscript = obj.GetComponent<Ghost>();
            ghostscript.die();
        }
    }
}
