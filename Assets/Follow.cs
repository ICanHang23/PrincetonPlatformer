using UnityEngine;

public class Follow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject player;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        transform.position = new Vector3(playerPosition.x + 3, playerPosition.y, -10); 
    }
}
