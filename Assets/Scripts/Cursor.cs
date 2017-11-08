using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Cursor : MonoBehaviour {

    public int PositionX
    {
        get { return x; }
        set
        {
            x = value;
            SpriteUpdate();
        }
    }

    public int PositionY
    {
        get { return y; }
        set
        {
            y = value;
            SpriteUpdate();
        }
    }

    public int MaxLength;

    private int x;
    private int y;

    private PositionArray<Block> data;
    private SpriteRenderer cursor;

    public bool Down()
    {
        var r = PositionY - 1 >= 0;
        if (r)
        {
            --PositionY;
            SpriteUpdate();
        } 
        return r;
    }

    public bool Up()
    {
        var r = PositionY + 1 < MaxLength;
        if (r)
        {
            ++PositionY;
            SpriteUpdate();
        }
        return r;
    }

    public bool Left()
    {
        var r = PositionX - 1 >= 0;
        if (r)
        {
            --PositionX;
            SpriteUpdate();
        }
        return r;
    }

    public bool Right()
    {
        var r = PositionX + 1 < MaxLength;
        if (r)
        {
            ++PositionX;
            SpriteUpdate();
        }
        return r;
    }

    public void SetBlockData(List<Block> source, int maxWidth)
    {
        MaxLength = maxWidth;
        data = new PositionArray<Block>(source, MaxLength, MaxLength);
    }


    private void SpriteUpdate()
    {
        Debug.Log($"{x}, {y}");
        if (cursor != null)
        {
            var pos = data.GetValue(PositionX, PositionY).transform.position;
            cursor.transform.position = pos;
            Debug.Log($"GetBlock[{PositionX * PositionY}]=>{pos.x}, {pos.y}");
        }
    }

	// Use this for initialization
	void Start ()
    {
        cursor = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
