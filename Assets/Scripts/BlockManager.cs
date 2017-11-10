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

    private List<Block.KIND> fieldData = new List<Block.KIND>();
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
            fieldData.Add(obj.CreateBlock());
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
        FieldUpdate();
        if (Input.GetAxis("Horizontal") < 0)
        {
            cursor.Left();
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            cursor.Right();
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            cursor.Up();
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            cursor.Down();
        }
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
