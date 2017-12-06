using System.Collections.Generic;
using UnityEngine;

using KIND = Block.KIND;

public class FieldManager
{
    // ブロックのゲームオブジェクト
    private List<Block> blocks;
    // ブロックの状態
    //private List<BlockStatus> fieldData;

    private bool[] checkedList;

    private int fieldWidth;

    // フィールドの一辺大きさ
    public int FieldWidth { get { return fieldWidth; } }

    public delegate void OnEarnedPoint(Events.EarnedPointEventArgs e);
    public event OnEarnedPoint onEarnedPointEvent;

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
        int deletedBlocks = 0;

        for (int y = 0; y < FieldWidth; ++y)
        {
            for (int x = 0; x < FieldWidth; ++x)
            {
                var block = blocks.PositionAt(x, y, FieldWidth);
                // その地点のブロックがNONEでなかったら調べる
                if (block.Kind != KIND.NONE && !block.Deleting)
                {
                    // 上の壁
                    if (y == 0)
                    {
                        // 下に向かって調べる

                        // 始点とその一つ隣のブロックが同じだったらチェック開始
                        if (block.Kind == blocks.PositionAt(x, y + 1, FieldWidth).Kind)
                        {
                            var kind = blocks.PositionAt(x, y + 1, FieldWidth).Kind;
                            var r = checkField(kind, x, y + 1, -1, y);
                            if (r > 0)
                            {
                                block.Deleting = true;
                                Debug.Log($"Delete count:{r + 1}");
                                deletedBlocks += r + 1;
                                break;
                            }
                            // チェック済みリストをリセット
                            for (int i = 0; i < checkedList.Length; ++i)
                            {
                                checkedList[i] = false;
                            }
                        }
                    }

                    // 左の壁
                    if (x == 0)
                    {
                        // 右に向かって調べる

                        // 始点とその一つ隣のブロックが同じだったらチェック開始
                        if (block.Kind == blocks.PositionAt(x + 1, y, FieldWidth).Kind)
                        {
                            var kind = blocks.PositionAt(x + 1, y, FieldWidth).Kind;
                            var r = checkField(kind, x + 1, y, x, -1);
                            if (r > 0)
                            {
                                block.Deleting = true;
                                Debug.Log($"Delete count:{r + 1}");
                                deletedBlocks += r + 1;
                                break;
                            }
                            // チェック済みリストをリセット
                            for (int i = 0; i < checkedList.Length; ++i)
                            {
                                checkedList[i] = false;
                            }
                        }
                    }

                    // 右の壁
                    if (x == FieldWidth - 1)
                    {
                        // 左に向かって調べる

                        // 始点とその一つ隣のブロックが同じだったらチェック開始
                        if (block.Kind == blocks.PositionAt(x - 1, y, FieldWidth).Kind)
                        {
                            var kind = blocks.PositionAt(x - 1, y, FieldWidth).Kind;
                            var r = checkField(kind, x - 1, y, x, -1);
                            if (r > 0)
                            {
                                block.Deleting = true;
                                Debug.Log($"Delete count:{r + 1}");
                                deletedBlocks += r + 1;
                                break;
                            }
                            // チェック済みリストをリセット
                            for (int i = 0; i < checkedList.Length; ++i)
                            {
                                checkedList[i] = false;
                            }
                        }
                    }

                    // 下の壁
                    if (y == FieldWidth - 1)
                    {
                        // 上に向かって調べる

                        // 始点とその一つ隣のブロックが同じだったらチェック開始
                        if (block.Kind == blocks.PositionAt(x, y - 1, FieldWidth).Kind)
                        {
                            var kind = blocks.PositionAt(x, y - 1, FieldWidth).Kind;
                            var r = checkField(kind, x, y - 1, -1, y);
                            if (r > 0)
                            {
                                block.Deleting = true;
                                Debug.Log($"Delete count:{r + 1}");
                                deletedBlocks += r + 1;
                                break;
                            }
                            // チェック済みリストをリセット
                            for (int i = 0; i < checkedList.Length; ++i)
                            {
                                checkedList[i] = false;
                            }
                        }
                    }
                }
            }
        }

        if (deletedBlocks > 0)
        {
            var point = (int)Mathf.Pow(deletedBlocks, 1.5f);
            onEarnedPointEvent(new Events.EarnedPointEventArgs(point, "block_delete"));
        }
    }

    
    private int checkField(KIND target, int posX, int posY, int startX, int startY)
    {
        var block = blocks.PositionAt(posX, posY, FieldWidth);
        // 消去中ブロックなら0
        if (block.Deleting)
        {
            return 0;
        }

        // 現在位置が壁かどうかを調べる
        if (startX == posX || startY == posY)
        {
            // 始点の壁だった場合は0
            return 0;
        }

        // チェック済みなら0
        if (checkedList.PositionAt(posX, posY, FieldWidth))
        {
            return 0;
        }

        // チェック済みとする
        checkedList.SetValuePosition(posX, posY, FieldWidth, true);

        // 元のブロックと種類が違うブロックなら0
        if (block.Kind != target)
        {
            return 0;
        }

        // 現在位置が壁かどうか
        if(posX == 0 || posY == 0 || posX == FieldWidth - 1 || posY == FieldWidth - 1)
        {

            // その位置のブロックが、始点と同じ種類かどうかを判定
            if (block.Kind == target )
            {
                block.Deleting = true;
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // 各方向のブロックを調べるために再帰する
        var up = checkField(target, posX, posY + 1, startX, startY);
        var down = checkField(target, posX, posY - 1, startX, startY);
        var left = checkField(target, posX - 1, posY, startX, startY);
        var right = checkField(target, posX + 1, posY, startX, startY);

        var r = up + down + left + right;
        // 判定結果を代入
        block.Deleting = r > 0 ? true : false;

        return r > 0 ? r + 1 : 0;
    }
}
