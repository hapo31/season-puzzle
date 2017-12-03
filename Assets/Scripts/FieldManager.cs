using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KIND = Block.KIND;
using System.Linq;

public class FieldManager
{
    // ブロックのゲームオブジェクト
    private List<Block> blocks;
    // ブロックの状態
    //private List<BlockStatus> fieldData;

    private bool[] checkedList;

    private int fieldWidth;

    public int FieldWidth { get { return fieldWidth; } }

    // フィールドの季節の状態
    public KIND FieldSeason { get; set; } = KIND.NONE;

    /// <summary>
    /// フィールドをアップデートする
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="fieldWidth"></param>
    public FieldManager(List<Block> blocks, int fieldWidth)
    {
        this.blocks = blocks;
        this.fieldWidth = fieldWidth;
        checkedList = new bool[blocks.Count];
    }

    /// <summary>
    /// 各壁から他の壁へ同じブロックが繋がっているかをチェックする
    /// </summary>
    public void Update()
    {        
        for (int y = 0; y < FieldWidth; ++y)
        {
            for (int x = 0; x < FieldWidth; ++x)
            {
                // その地点のブロックがNONEでなかったら調べる
                if (blocks.PositionAt(x, y, FieldWidth).Kind != KIND.NONE)
                {
                    // 上の壁
                    if (y == 0)
                    {
                        // 下に向かって調べる
                        
                    }

                    // 左の壁
                    if (x == 0)
                    {
                        // 右に向かって調べる
                    }

                    // 右の壁
                    if (x == FieldWidth - 1)
                    {
                        // 左に向かって調べる
                    }

                    // 下の壁
                    if (y == FieldWidth - 1)
                    {
                        // 上に向かって調べる
                    }
                }
            }
        }
    }

    
    private bool checkField(KIND target, int posX, int posY, int startX, int startY)
    {
        // スタート地点と同じマスならfalse
        if (startX == posX && startY == posY)
        {
            return false;
        }

        // チェック済みならfalse
        if (checkedList.PositionAt(posX, posY, FieldWidth))
        {
            return false;
        }

        // チェック済みとする
        checkedList.SetValuePosition(posX, posY, FieldWidth, true);

        // 元のブロックと種類が違うブロックならfalse
        if (blocks.PositionAt(posX, posY, FieldWidth).Kind != target)
        {
            return false;
        }

        // 調べようとしている方向が壁だったりフィールド外だったらtrueを返す
        if (KIND.NONE == blocks.PositionAt(posX, posY, fieldWidth).Kind || !blocks.InField(posX, posY, FieldWidth))
        {
            return true;
        }
        else
        {
            // 各方向のブロックを調べるために再帰する
            var up = checkField(target, posX, posY + 1, startX, startY);
            var down = checkField(target, posX, posY - 1, startX, startY);
            var left = checkField(target, posX - 1, posY, startX, startY);
            var right = checkField(target, posX + 1, posY, startX, startY);

            var r = up || down || left || right;
            // 判定結果を代入
            blocks.PositionAt(posX, posY, FieldWidth).Deleting = r;

            return r;
        }
    }
}
