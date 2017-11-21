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
    
    //private List<Block.KIND> fieldData;

    private List<Block> blocks;
    private SpriteRenderer cursor;

    /// <summary>
    /// カーソルを上へ
    /// </summary>
    /// <returns></returns>
    public bool Up()
    {
        var r = CheckUp();
        if (r)
        {
            --PositionY;
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
        return y - 1 >= 0 && blocks.PositionAt(x, y - 1, MaxLength).Kind != Block.KIND.NONE;
    }

    /// <summary>
    /// カーソルを下へ
    /// </summary>
    /// <returns></returns>
    public bool Down()
    {
        var r = CheckDown();
        if (r)
        {
            ++PositionY;
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
        return y + 1 < MaxLength && blocks.PositionAt(x, y + 1, MaxLength).Kind != Block.KIND.NONE;
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
        return x - 1 >= 0 && blocks.PositionAt(x - 1, y, MaxLength).Kind != Block.KIND.NONE;
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
        return x + 1 < MaxLength && blocks.PositionAt(x + 1, y, MaxLength).Kind != Block.KIND.NONE;
    }

    /// <summary>
    /// ブロックのゲームオブジェクトデータを設定する
    /// </summary>
    /// <param name="source"></param>
    /// <param name="maxWidth"></param>
    public void SetBlockData(List<Block> source, int maxWidth)
    {
        MaxLength = maxWidth;
        blocks = source;
    }
    
    /// <summary>
    /// 選択位置のブロックとその上のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapUpBlock()
    {
        if (CheckUp())
        {
            SwapFieldData(x, y, x, y - 1);
            Up();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 選択位置のブロックとその下のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapDownBlock()
    {
        if (CheckDown())
        {
            SwapFieldData(x, y, x, y + 1);
            Down();
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

    // 指定した2つの座標のブロックを入れ替える
    private void SwapFieldData(int fromX, int fromY, int toX, int toY)
    {
        var fromBlock = blocks.PositionAt(fromX, fromY, MaxLength);
        var toBlock = blocks.PositionAt(toX, toY, MaxLength);
        var tmp = toBlock.Kind;

        toBlock.Kind = fromBlock.Kind;
        fromBlock.Kind = tmp;
    }

    // カーソルの座標をブロックに合わせる
    private void SpriteUpdate()
    {
        if (cursor != null)
        {
            var pos = blocks.PositionAt(PositionX, PositionY, MaxLength).transform.position;
            cursor.transform.position = pos;
        }
    }

	// Use this for initialization
	void Start ()
    {
        cursor = GetComponent<SpriteRenderer>();
        SpriteUpdate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
