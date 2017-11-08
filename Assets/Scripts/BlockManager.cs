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

    private List<Block> blocks = new List<Block>();
    private List<Sprite> background = new List<Sprite>();
    private Cursor cursor;

    private const float BASEY = 4.0f;
    private const float BASEX = 7.0f;
    private const float BASELENGTH = 2.6f;

    private int frames = 0;

    // Use this for initialization
    void Start()
    {

        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate(blockPrefab);

            float x = i % FieldLength / (BASELENGTH * 2) * BASEX + OffsetXPosition;
            float y = i / FieldLength / BASELENGTH * BASEY + OffsetYPosition;
            Debug.Log($"x:{x} y:{y}");
            obj.transform.position = new Vector3(x, y);
            obj.Kind = Block.GetRandomBlockKind(true);
            obj.name = $"Block[{blocks.Count}](Pos:{x}, {y})";

            blocks.Add(obj);
        }


        cursor = Instantiate(cursorPrefab);
        cursor.SetBlockData(blocks, FieldLength);
        cursor.PositionX = 0;
        cursor.PositionY = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //var target = Random.Range(0, blocks.Count - 1);
        //var kind = Block.GetRandomBlockKind();

        // blocks[target].Kind = kind;

        var d = -1;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            d = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            d = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            d = 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            d = 3;
        }

        switch (d)
        {
            case 0:
                cursor.Up();
                break;
            case 1:
                cursor.Right();
                break;
            case 2:
                cursor.Down();
                break;
            case 3:
                cursor.Left();
                break;
        }
        ++frames;
    }

    Vector3 GetBlockPosition(int x, int y)
    {
        return blocks[y * FieldLength + x].transform.position;
    }
}
