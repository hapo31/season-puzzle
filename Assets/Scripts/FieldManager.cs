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
        for (int i = 1; i < FieldWidth - 1; ++i)
        {
            // その壁にくっついているブロックはすべてチェック済みとしておく
            checkedList[i] = true;
        }

        // 上側の壁からチェック
        for (int i = 1; i < FieldWidth - 1; ++i)
        {
            // 下に向かってチェックする
            checkField(blocks[i].Kind, i % fieldWidth, 1);

        }

        // すべてチェックし終わったらブロックの種類を変える
        for (var i = 0; i < blocks.Count; ++i)
        {
            if (blocks[i].Deleting)
            {
                blocks[i].Kind = KIND.NONE;
            }
        }

        for (var i = 0; i < checkedList.Length; ++i)
        {
            checkedList[i] = false;
        }
    }

    
    private bool checkField(KIND target, int posX, int posY)
    {
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
            var up = checkField(target, posX, posY + 1);
            var down = checkField(target, posX, posY - 1);
            var left = checkField(target, posX - 1, posY);
            var right = checkField(target, posX + 1, posY);

            var r = up || down || left || right;
            // 判定結果を代入
            blocks.PositionAt(posX, posY, FieldWidth).Deleting = r;

            return r;
        }
    }
}
