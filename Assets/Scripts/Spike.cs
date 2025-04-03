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
        Debug.Log("lol");

        if (obj.tag.Equals("Player") && other.collider is BoxCollider2D)
        {
            Debug.Log("lmao");
            Move playerscript = obj.GetComponent<Move>();
            playerscript.die();
        }
    }
}
