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

    public bool Hold { get; set; }

    private int x;
    private int y;
    
    private PositionArray<Block.KIND> fieldData;

    private PositionArray<Block> data;
    private SpriteRenderer cursor;

    /// <summary>
    /// カーソルを下へ
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// カーソルを上へ
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// カーソルを左へ
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// カーソルを右へ
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ブロックのゲームオブジェクトデータを設定する
    /// </summary>
    /// <param name="source"></param>
    /// <param name="maxWidth"></param>
    public void SetBlockData(List<Block> source, int maxWidth)
    {
        MaxLength = maxWidth;
        data = new PositionArray<Block>(source, MaxLength, MaxLength);
    }

    /// <summary>
    /// フィールドデータを設定する
    /// </summary>
    /// <param name="source"></param>
    /// <param name="maxWidth"></param>
    public void SetFieldData(List<Block.KIND> source, int maxWidth)
    {
        MaxLength = maxWidth;
        fieldData = new PositionArray<Block.KIND>(source, maxWidth, maxWidth);
    }
    
    /// <summary>
    /// 選択位置のブロックとその下のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapDownBlock()
    {
        if (y > 0)
        {
            SwapFieldData(x, y, x, y - 1);
            Down();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 選択位置のブロックとその上のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapUpBlock()
    {
        if (y < MaxLength)
        {
            SwapFieldData(x, y, x, y + 1);
            Up();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 選択位置のブロックとその左のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapLeftBlock()
    {
        if (x > 0)
        {
            SwapFieldData(x, y, x - 1, y);
            Left();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 選択位置のブロックとその右のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapRightBlock()
    {
        if (x < MaxLength)
        {
            SwapFieldData(x, y, x + 1, y);
            Right();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SwapFieldData(int fromX, int fromY, int toX, int toY)
    {
        var from = fieldData.GetValue(fromX, fromY);
        var to = fieldData.GetValue(toX, toY);
        fieldData.SetValue(toX, toY, from);
        fieldData.SetValue(fromX, fromY, to);
    }

    private void SpriteUpdate()
    {
        if (cursor != null)
        {
            var pos = data.GetValue(PositionX, PositionY).transform.position;
            cursor.transform.position = pos;
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
