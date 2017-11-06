using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    private KIND kind;

    public KIND Kind
    {
        get
        {
            return kind;
        }

        set
        {
            kind = value;
        }
    }

    public Sprite NoneSprite;
    public Sprite SpringSprite;
    public Sprite SummerSprite;
    public Sprite FallSprite;
    public Sprite WinterSprite;

    SpriteRenderer spriteRenderer;
    
    Block(KIND kind = KIND.NONE)
    {
        this.kind = kind;
    }

	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetKindSprite(this.kind);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    private Sprite GetKindSprite(KIND kind)
    {
        switch (kind)
        {
            case KIND.NONE:
                return NoneSprite;
            case KIND.SPRING:
                return SpringSprite;
            case KIND.SUMMER:
                return SummerSprite;
            case KIND.FALL:
                return FallSprite;
            case KIND.WINTER:
                return WinterSprite;
            default:
                return NoneSprite;
        }
    }

    public enum KIND
    {
        NONE,
        SPRING,
        SUMMER,
        FALL,
        WINTER
    }
}
