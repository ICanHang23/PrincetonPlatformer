using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Player") && other is BoxCollider2D)
        {
            Player player = obj.GetComponent<Player>();
            player.respawnLocation = transform.position;

        }
        else if (obj.tag.Equals("Ghost") && other is BoxCollider2D)
        {
            Ghost ghost = obj.GetComponent<Ghost>();
            ghost.respawnLocation = transform.position;
        }
    }
}
