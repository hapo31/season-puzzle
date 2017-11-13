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
        var r = CheckDown();
        if (r)
        {
            --PositionY;
            SpriteUpdate();
        } 
        return r;
    }

    /// <summary>
    /// カーソルが下に移動できるかチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckDown()
    {
        return y - 1 >= 0 && fieldData.GetValue(x, y - 1) != Block.KIND.NONE;
    }

    /// <summary>
    /// カーソルを上へ
    /// </summary>
    /// <returns></returns>
    public bool Up()
    {
        var r = CheckUp();
        if (r)
        {
            ++PositionY;
            SpriteUpdate();
        }
        return r;
    }

    /// <summary>
    /// カーソルが上に移動できるかチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckUp()
    {
        return y + 1 < MaxLength && fieldData.GetValue(x, y + 1) != Block.KIND.NONE;
    }

    /// <summary>
    /// カーソルを左へ
    /// </summary>
    /// <returns></returns>
    public bool Left()
    {
        var r = CheckLeft();
        if (r)
        {
            --PositionX;
            SpriteUpdate();
        }
        return r;
    }

    /// <summary>
    /// カーソルが左に移動できるかチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckLeft()
    {
        return x - 1 >= 0 && fieldData.GetValue(x - 1, y) != Block.KIND.NONE;
    }

    /// <summary>
    /// カーソルを右へ
    /// </summary>
    /// <returns></returns>
    public bool Right()
    {
        var r = CheckRight();
        if (r)
        {
            ++PositionX;
            SpriteUpdate();
        }
        return r;
    }

    /// <summary>
    /// カーソルが右に移動できるかチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckRight()
    {
        return x + 1 < MaxLength && fieldData.GetValue(x + 1, y) != Block.KIND.NONE;
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
        if (CheckDown())
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
        if (CheckUp())
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
        if (CheckLeft())
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
        if (CheckRight())
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
