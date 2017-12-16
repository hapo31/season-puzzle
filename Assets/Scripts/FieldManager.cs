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

    private FieldSettings settings;

    // フィールドの一辺大きさ
    public int FieldWidth { get { return settings.FieldWidth; } }

    public delegate void OnEarnedPoint(Events.EarnedPointEventArgs e);
    public event OnEarnedPoint onEarnedPointEvent;

    public delegate void OnBlockGenerate(int count);
    public event OnBlockGenerate onBlockGenerate;

    /// <summary>
    /// フィールドをアップデートする
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="fieldWidth"></param>
    public FieldManager(List<Block> blocks, FieldSettings settings)
    {
        this.blocks = blocks;
        this.settings = settings;
        checkedList = new bool[blocks.Count];
    }

    /// <summary>
    /// 各壁から他の壁へ同じブロックが繋がっているかをチェックする
    /// </summary>
    public void Update()
    {
        var deleteBlockInfo = DeleteBlock();
        if (deleteBlockInfo.Deleted > 0)
        {
            // 得点計算
            var point = deleteBlockInfo.Deleted * deleteBlockInfo.WallConnected * 10;
            onEarnedPointEvent?.Invoke(new Events.EarnedPointEventArgs(point, "block_delete", deleteBlockInfo.Kind));
        }
        UpdateBlocks();
    }

    /// <summary>
    /// ブロック消去処理を行う
    /// </summary>
    /// <returns>消したブロック数</returns>
    private DeleteBlockInfo DeleteBlock()
    {
        var deletedInfo = new DeleteBlockInfo();

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
                            if (r?.Deleted > 0)
                            {
                                block.Deleting = true;
                                deletedInfo.Deleted += r.Value.Deleted + 1;
                                deletedInfo.WallConnected += r.Value.WallConnected;
                                deletedInfo.Kind = kind;
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
                            if (r?.Deleted > 0)
                            {
                                block.Deleting = true;
                                deletedInfo.Deleted += r.Value.Deleted + 1;
                                deletedInfo.WallConnected += r.Value.WallConnected;
                                deletedInfo.Kind = kind;
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
                            if (r?.Deleted > 0)
                            {
                                block.Deleting = true;
                                deletedInfo.Deleted += r.Value.Deleted + 1;
                                deletedInfo.WallConnected += r.Value.WallConnected;
                                deletedInfo.Kind = kind;
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
                            if (r?.Deleted > 0)
                            {
                                block.Deleting = true;
                                deletedInfo.Deleted += r.Value.Deleted + 1;
                                deletedInfo.WallConnected += r.Value.WallConnected;
                                deletedInfo.Kind = kind;
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

        return deletedInfo;
    }

    /// <summary>
    /// 消去フラグが付いているブロックを再生成したりする
    /// </summary>
    /// <returns></returns>
    public void UpdateBlocks()
    {
        var generateCount = 0;

        for (int i = 0; i < blocks.Count; ++i)
        {
            var block = blocks[i];
            if (block.Deleting)
            {
                block.DeletedFrame += 1;
                // 指定フレーム数が経ったらブロックを再生成
                if (block.DeletedFrame >= settings.BlockRegenerateFrame)
                {
                    block.Deleting = false;

                    generateCount ++;
                    
                    block.SetRandomBlock();
                }
            }
        }
        if (generateCount > 0)
        {
            onBlockGenerate?.Invoke(generateCount);
        }
    }
    
    private DeleteBlockInfo? checkField(KIND target, int posX, int posY, int startX, int startY)
    {
        var block = blocks.PositionAt(posX, posY, FieldWidth);
        // 消去中ブロックなら0
        if (block.Deleting)
        {
            return null;
        }

        // 現在位置が壁かどうかを調べる
        if (startX == posX || startY == posY)
        {
            // 始点の壁だった場合は0
            return null;
        }

        // チェック済みなら0
        if (checkedList.PositionAt(posX, posY, FieldWidth))
        {
            return null;
        }

        // チェック済みとする
        checkedList.SetValuePosition(posX, posY, FieldWidth, true);

        // 元のブロックと種類が違うブロックなら0
        if (block.Kind != target)
        {
            return null;
        }

        // 現在位置が壁かどうか
        if(posX == 0 || posY == 0 || posX == FieldWidth - 1 || posY == FieldWidth - 1)
        {
            // その位置のブロックが、始点と同じ種類かどうかを判定
            if (block.Kind == target )
            {
                block.Deleting = true;
                return new DeleteBlockInfo(1, 1);
            }
            else
            {
                return null;
            }
        }

        // 各方向のブロックを調べるために再帰する
        var up = checkField(target, posX, posY + 1, startX, startY);
        var down = checkField(target, posX, posY - 1, startX, startY);
        var left = checkField(target, posX - 1, posY, startX, startY);
        var right = checkField(target, posX + 1, posY, startX, startY);

        var r = up + down + left + right;
        // 判定結果を代入
        block.Deleting = r.Deleted > 0 ? true : false;

        if (r.Deleted > 0)
        {
            r.Deleted += 1;
            return r;
        } 
        else
        {
            return null;
        }
    }
}

/// <summary>
/// フィールドの設定データ
/// </summary>
public class FieldSettings
{
    public int BlockRegenerateFrame { get; set; }
    public int FieldWidth { get; set; }
}

struct DeleteBlockInfo
{
    // 何方向の壁とつながっていたか
    public int WallConnected;
    // 消したブロックの数
    public int Deleted;
    // 直前に消したブロックの種類
    public KIND Kind;

    public DeleteBlockInfo(int deleted = 0, int connected = 0, KIND kind = KIND.NONE)
    {
        Deleted = deleted;
        WallConnected = connected;
        Kind = kind;
    }

    public static DeleteBlockInfo operator+(DeleteBlockInfo? leftVal, DeleteBlockInfo? rightVal)
    {
        int deleted = 0, wall = 0;

        deleted += leftVal != null ? leftVal.Value.Deleted : 0;
        deleted += rightVal != null ? rightVal.Value.Deleted : 0;
        wall += leftVal != null ? leftVal.Value.WallConnected : 0;
        wall += rightVal != null ? rightVal.Value.WallConnected : 0;

        return new DeleteBlockInfo(deleted, wall);
    }
}