using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // 背景画像のスケール
    private Vector3 defaultScale;
    
    private Material material;
    private SpriteRenderer gradationBackground;
    private SpriteRenderer[] backgroundRenderer = new SpriteRenderer[2];

    private int drawingFlipBackground = 0;

    // 描画していない背景のアルファ値
    // floatで管理すると誤差が発生するので整数値を1/100して扱う
    private int alpha = 0;
    private bool changing = false;

    private Block.KIND kind;

    private SpriteRenderer PrimarySprite
    {
        get
        {
            return backgroundRenderer[drawingFlipBackground];
        }
    }

    private SpriteRenderer SecondarySprite
    {
        get
        {
            return backgroundRenderer[1 - drawingFlipBackground];
        }
    }

    // Use this for initialization
    void Start () {
        gradationBackground = GetComponent<SpriteRenderer>();
        material = GetComponent<Material>();
        backgroundRenderer = GetComponentsInChildren<SpriteRenderer>().Where(v => v.tag == "DeepBackground").ToArray();
        defaultScale = PrimarySprite.transform.localScale;
        PrimarySprite.sortingOrder = 1;
        PrimarySprite.color = ConvertAlpha(PrimarySprite.color, 1.0f);
        // 描画していない背景のアルファ値を0にする
        SecondarySprite.color = ConvertAlpha(SecondarySprite.color, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (changing)
        {
            alpha -= 4;
            if (alpha >= 0)
            {
                PrimarySprite.color = ConvertAlpha(PrimarySprite.color, alpha / 100.0f);
            }
            if (alpha <= 0)
            {
                // 完全に透明になったらレイヤーのオーダーを入れ替える
                PrimarySprite.sortingOrder = 0;
                SecondarySprite.sortingOrder = 1;
                drawingFlipBackground = 1 - drawingFlipBackground;
                changing = false;
            }
        }
	}

    public void ChangeTheme(Block.KIND kind)
    {
        Sprite newSprite = null;
        alpha = 100;
        changing = true;
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

        // 描画していない方の背景を切り替える
        SecondarySprite.sortingOrder = 0;
        SecondarySprite.sprite = newSprite;
        SecondarySprite.transform.localScale = defaultScale;
        // アルファ値を1.0に
        SecondarySprite.color = ConvertAlpha(SecondarySprite.color, 1.0f);

    }

    private Color ConvertAlpha(Color before, float alpha)
    {
        before.a = alpha;
        return before;
    }

}
