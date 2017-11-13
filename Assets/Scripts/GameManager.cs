using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class GameManager : MonoBehaviour
{
    public Block blockPrefab;
    public Cursor cursorPrefab;

    public Sprite BackgroundSprite;
    public int FieldLength;
    public float OffsetXPosition;
    public float OffsetYPosition;

    // キーを押しっぱなしにしたあと反応しないフレーム数
    public int KeyDelayFrame = 20;
    // キー押しっぱなしにしている間、処理を行う間隔
    public int KeyRepeatInterval = 5;

    private List<bool> checkedField;
    // ブロックのゲームオブジェクト
    private List<Block> blocks = new List<Block>();
    private List<Sprite> background = new List<Sprite>();

    // ブロックの状態
    private List<Block.KIND> fieldData = new List<Block.KIND>();
    // カーソルのゲームオブジェクト
    private Cursor cursor;

    private const float BASEY = 3.0f;
    private const float BASEX = 6.0f;
    private const float BASELENGTH = 2.6f;

    public InputManager PlayerInputManager;

    private int frames = 0;

    // Use this for initialization
    void Start()
    {
        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate(blockPrefab);

            float x = i % FieldLength / (BASELENGTH * 2) * BASEX + OffsetXPosition;
            float y = i / FieldLength / BASELENGTH * BASEY + OffsetYPosition;
            obj.transform.position = new Vector3(x, y);
            
            if (i == 0 || i == FieldLength - 1 || i == (FieldLength * (FieldLength - 1)) || i == (FieldLength * FieldLength) - 1)
            {
                // フィールドの角の4つはNONEブロックにする
                fieldData.Add(Block.KIND.NONE);
            }
            else
            {
                // ブロックをランダム生成する
                fieldData.Add(obj.CreateBlock());
            }

            if (i % (FieldLength / 2 * (FieldLength * 2)) == 0)
            {
                Debug.Log($"[Center] {x}, {y}");
            }

            obj.name = $"Block[{blocks.Count}](Pos:{x}, {y})";

            blocks.Add(obj);
        }


        cursor = Instantiate(cursorPrefab);
        cursor.SetBlockData(blocks, FieldLength);
        cursor.SetFieldData(fieldData, FieldLength);

        // キー操作の定義

        // カーソルを左へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.LEFT, (key, frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapLeftBlock();
            }
            else
            {
                cursor.Left();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.LEFT, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.LEFT, KeyRepeatInterval, null);

        // カーソルを右へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.RIGHT, (key, frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapRightBlock();
            }
            else
            {
                cursor.Right();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.RIGHT, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.RIGHT, KeyRepeatInterval, null);

        // カーソルを上へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.UP, (key, frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapUpBlock();
            }
            else
            {
                cursor.Up();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.UP, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.UP, KeyRepeatInterval, null);

        // カーソルを下へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.DOWN, (key, frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapDownBlock();
            }
            else
            {
                cursor.Down();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.DOWN, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.DOWN, KeyRepeatInterval, null);

        // ブロックを選択
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.BLOCKCHANGE_A, (key, frames) => cursor.Hold = true);
        PlayerInputManager.RegisterOnUpHandler(KEYS.BLOCKCHANGE_A, (key, frames) => cursor.Hold = false);


        cursor.PositionX = FieldLength / 2;
        cursor.PositionY = FieldLength / 2;
    }

    // Update is called once per frame
    void Update()
    {
        FieldUpdate();

    }

    void FieldUpdate()
    {
        for(var i = 0; i < fieldData.Count; ++i)
        {
            blocks[i].Kind = fieldData[i];
        }
    }

    Vector3 GetBlockPosition(int x, int y)
    {
        return  blocks.PositionAt(x, y, FieldLength).transform.position;
    }
}


public class FieldManager
{

}
