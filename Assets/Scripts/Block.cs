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
            if (kind != value && spriteRenderer != null && spriteRenderer.sprite != null)
            {
                kind = value;
                spriteRenderer.sprite = GetKindSprite(kind);
            }
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

    public KIND CreateBlock()
    {
        return Kind = GetRandomBlockKind();
    }

    public static KIND GetRandomBlockKind(bool generateNone = false)
    {
        return !generateNone ? (KIND)Random.Range(1, 5) : (KIND)Random.Range(0, 5);
    }

    public enum KIND
    {
        NONE,
        SPRING,
        SUMMER,
        FALL,
        WINTER
    }

    public class BlockStatus
    {
        public KIND kind;
        public bool deleting;

        public BlockStatus(KIND kind, bool deleting = false)
        {
            this.kind = kind;
            this.deleting = deleting;
        }
    }
}
