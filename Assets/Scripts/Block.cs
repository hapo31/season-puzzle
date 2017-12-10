using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    private KIND kind;
    private int regenerateFrame = 300;
    private int deletedFrame = 0;

    private SpriteMask mask;

    public KIND Kind
    {
        get
        {
            return kind;
        }

        set
        {
            kind = value;
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                spriteRenderer.sprite = GetKindSprite(kind);
            }
        }
    }

    private bool deleting;
    /// <summary>
    /// 消去中フラグ
    /// </summary>
    public bool Deleting
    {
        get { return deleting; }
        set
        {
            deleting = value;
            if (value) {
                spriteRenderer.sprite = DeletedSprite;
            }
            else
            {
                spriteRenderer.sprite = GetKindSprite(kind);
                deletedFrame = 0;
            }
        }
    }

    /// <summary>
    /// 消去してから経ったフレーム数
    /// </summary>
    public int DeletedFrame
    {
        get { return deletedFrame; }
        set
        {
            if (deleting)
            {
                deletedFrame = value;
                //mask.transform.position = new Vector3()
            }
        }
    }

    public Sprite NoneSprite;
    public Sprite DeletedSprite;
    public Sprite SpringSprite;
    public Sprite SummerSprite;
    public Sprite FallSprite;
    public Sprite WinterSprite;

    SpriteRenderer spriteRenderer;
    
    Block(KIND kind, int regenerateFrame)
    {
        this.kind = kind;
        this.regenerateFrame = regenerateFrame;
    }

	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetKindSprite(this.kind);
        mask = GetComponent<SpriteMask>();

        // スプライトの見た目が初期化されていない可能性があるのでここで更新する
        Kind = kind;
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

    public void SetRandomBlock()
    {
        Kind = GetRandomBlockKind();
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
}
