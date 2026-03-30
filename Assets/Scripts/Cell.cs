using UnityEngine;

public class Cell : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private SpriteRenderer SpriteRenderer;
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite hightlight;
    private SpriteRenderer spriteReneder;



    // Update is called once per frame
    public void Awake()
    {
        spriteReneder = gameObject.GetComponent<SpriteRenderer>();

    }
    public void Normal()
    {
        gameObject.SetActive(true);
        spriteReneder.color = Color.white;
        spriteReneder.sprite = hightlight;
    }    
    public void Hightlight()
    {
        gameObject.SetActive(true);
        spriteReneder.color = Color.white;
        spriteReneder.sprite = normal;
    }
    public void Hover()
    {
        gameObject.SetActive(true);
        spriteReneder.color = new(1.0f, 1.0f, 1.0f, 0.5f);
        spriteReneder.sprite = normal;
    }
    public void Hilde ()
    {
        gameObject.SetActive(false);
    }    
}
