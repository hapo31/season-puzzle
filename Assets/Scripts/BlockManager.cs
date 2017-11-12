using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    public Block blockPrefab;
    public Cursor cursorPrefab;

    public Sprite BackgroundSprite;
    public int FieldLength;
    public float OffsetXPosition;
    public float OffsetYPosition;

    public int KeyDelayFrame = 20;
    public int KeyRepeatInterval = 5;

    private List<Block> blocks = new List<Block>();
    private List<Sprite> background = new List<Sprite>();

    private List<Block.KIND> fieldData = new List<Block.KIND>();
    private Cursor cursor;

    private const float BASEY = 4.0f;
    private const float BASEX = 7.0f;
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
            fieldData.Add(obj.CreateBlock());
            obj.name = $"Block[{blocks.Count}](Pos:{x}, {y})";

            blocks.Add(obj);
        }


        cursor = Instantiate(cursorPrefab);
        cursor.SetBlockData(blocks, FieldLength);
        cursor.SetFieldData(fieldData, FieldLength);
        cursor.PositionX = 0;
        cursor.PositionY = 0;

        // キー操作の定義

        // カーソルを左へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.LEFT, key =>
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
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.LEFT, KeyRepeatInterval, (key, _) => cursor.Left());

        // カーソルを右へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.RIGHT, key =>
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
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.RIGHT, KeyRepeatInterval, (key, _) => cursor.Right());

        // カーソルを上へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.UP, key =>
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
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.UP, KeyRepeatInterval, (key, _) => cursor.Up());

        // カーソルを下へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.DOWN, key =>
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
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.DOWN, KeyRepeatInterval, (key, _) => cursor.Down());

        // ブロックを選択
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.BLOCKCHANGE_A, key => cursor.Hold = true);
        PlayerInputManager.RegisterOnUpHandler(KEYS.BLOCKCHANGE_A, (key, _) => cursor.Hold = false);


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
        return blocks[y * FieldLength + x].transform.position;
    }
}
