using UnityEngine;

public class ScannerScript : MonoBehaviour
{
    [SerializeField]
    int num = 0;
    [SerializeField]
    Texture2D unlockedTexture;

    public bool locked = true;
    public bool open = false;
    SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
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
            if (!locked && !open)
            {
                open = true;

                Rect spriteRect = new Rect(0.0f, 0.0f, unlockedTexture.width, unlockedTexture.height);
                Vector2 spriteVect = new Vector2(0.5f, 0.5f);
                sprite.sprite = Sprite.Create(unlockedTexture, spriteRect, spriteVect);

                GameObject door = GameObject.Find("Door" + num);
                DoorScript doorScript = door.GetComponent<DoorScript>();
                doorScript.Open();
            }
        }
    }
}
