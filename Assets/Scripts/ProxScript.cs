using UnityEngine;

public class ProxScript : MonoBehaviour
{
    [SerializeField]
    int num = 0;
    private SpriteRenderer spr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Player") && other is BoxCollider2D)
        {
            GameObject door = GameObject.Find("ProxScanner" + num);
            door.GetComponent<ScannerScript>().locked = false;

            Destroy(gameObject);

        }
    }
}
