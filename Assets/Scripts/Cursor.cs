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
        var r = isMovablePosition(x, y - 1);
        if (r)
        {
            --PositionY;
            SpriteUpdate();
        } 
        return r;
    }

    /// <summary>
    /// カーソルを下へ
    /// </summary>
    /// <returns></returns>
    public bool Down()
    {
        var r = isMovablePosition(x, y + 1);
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
        var r = isMovablePosition(x - 1, y);
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
        var r = isMovablePosition(x + 1, y);
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
        blocks = source;
    }
    
    /// <summary>
    /// 選択位置のブロックとその上のブロックを入れ替える
    /// </summary>
    /// <returns>移動に成功したかどうか</returns>
    public bool SwapUpBlock()
    {
        if (isSwapablePosition(x, y, x, y - 1))
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
        if (isSwapablePosition(x, y, x, y + 1))
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
        if (isSwapablePosition(x, y, x - 1, y))
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
        if (isSwapablePosition(x, y, x + 1, y))
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
    
    /// <summary>
    /// その位置にカーソルが移動できるかどうか
    /// 指定座標がフィールド内かつ、ブロックの種類がNONEでなければ移動可能
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool isMovablePosition(int x, int y)
    {
        var inField = blocks.InField(x, y, MaxLength);
        return inField && blocks.PositionAt(x, y, MaxLength).Kind != Block.KIND.NONE;
    }

    /// <summary>
    /// その位置のブロックと交換できるか
    /// isMovablePositionがtrueを返すような条件かつ、そのブロックが消去中でなければ交換可能
    /// </summary>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    /// <returns></returns>
    private bool isSwapablePosition(int fromX, int fromY, int toX, int toY)
    {
        // 移動先にカーソルが移動できるか
        return isMovablePosition(toX, toY) &&
            // 移動元ブロックが消去中でないか
            !blocks.PositionAt(fromX, fromY, MaxLength).Deleting &&
            // 移動先ブロックが消去中でないか
            !blocks.PositionAt(toX, toY, MaxLength).Deleting;
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
