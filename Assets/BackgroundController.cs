using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

    public Sprite SpringSprite;
    public Sprite SummerSprite;
    public Sprite FallSprite;
    public Sprite WinterSprite;

    public Color SpringColor;
    public Color SummerColor;
    public Color FallColor;
    public Color WinterColor;

    private Material material;
    private SpriteRenderer backgroundRenderer;

    private Block.KIND kind;

    // Use this for initialization
    void Start () {
        material = GetComponent<Material>();
        backgroundRenderer = GetComponentInChildren<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeTheme(Block.KIND kind)
    {
        Sprite newSprite = null;
        switch(this.kind = kind)
        {
            case Block.KIND.SPRING:
                newSprite = SpringSprite;
                break;

            case Block.KIND.SUMMER:
                newSprite = SummerSprite;
                break;

            case Block.KIND.FALL:
                newSprite = FallSprite;
                break;

            case Block.KIND.WINTER:
                newSprite = WinterSprite;
                break;
        }
        backgroundRenderer.sprite = newSprite;
        backgroundRenderer.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
    }

}
