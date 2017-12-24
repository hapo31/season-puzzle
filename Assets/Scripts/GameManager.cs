using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Util;

public class GameManager : MonoBehaviour
{
    public Block blockPrefab;
    public Cursor cursorPrefab;

    public InputManager PlayerInputManager;
    public int FieldLength;
    public float OffsetXPosition;
    public float OffsetYPosition;
    public int GameTimeInMilliSeconds = 1000 * 60 * 2;
    public AudioClip BlockRegenerateSE;
    public Text ScoreText;
    public Text HiScoreText;
    public Text TimeText;

    private AudioSource bgm;
    private AudioSource audioSource;

    // キーを押しっぱなしにしたあと反応しないフレーム数
    public int KeyDelayFrame = 20;
    // キー押しっぱなしにしている間、処理を行う間隔
    public int KeyRepeatInterval = 5;

    // ブロックを消したあと再生成されるまでのフレーム数
    public int BlockRegenerateTime = 300;

    // スコア表示の桁数を表示するときのフォーマット
    private string ScoreTextFormat;

    private FieldManager fieldManager;

    // ブロックのゲームオブジェクト
    private List<Block> blocks = new List<Block>();
    private List<Sprite> background = new List<Sprite>();

    // ブロックの状態
    //private List<Block.KIND> fieldData = new List<Block.KIND>();
    // カーソルのゲームオブジェクト
    private Cursor cursor;

    // 背景の管理コンポーネント
    private BackgroundController backgroundContoroller;

    private const float BASEY = 3.0f;
    private const float BASEX = 6.0f;
    private const float BASELENGTH = 2.6f;

    private int showScore = 0;
    private int score = 0;
    private int hiScore = 0;

    private int Score
    {
        get { return score; }
        set
        {
            score = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        var audio = GetComponents<AudioSource>();
        audioSource = audio[0];


        // この辺でハイスコア読み込み処理
        // hiScore = hogehoge();

        // 時間表示の更新
        TimeText.text = MilliSecondsToString.format(GameTimeInMilliSeconds);
        // 背景コントローラーの取得
        backgroundContoroller = GameObject.Find("InfomationBackground").GetComponent<BackgroundController>();

        ScoreTextFormat = new string('0', ScoreText.text.Length);
        Score = 0;
        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate(blockPrefab);

            float x = i % FieldLength / (BASELENGTH * 2) * BASEX + OffsetXPosition;
            float y =  -(i / FieldLength / BASELENGTH * BASEY + OffsetYPosition);
            obj.transform.position = new Vector3(x, y);
            
            if (i == 0 || i == FieldLength - 1 || i == (FieldLength * (FieldLength - 1)) || i == (FieldLength * FieldLength) - 1)
            {
                // フィールドの角の4つはNONEブロックにする
                obj.Kind = Block.KIND.NONE;
            }
            else
            {
                // ブロックをランダム生成する
                obj.SetRandomBlock();
            }

            if (i % (FieldLength / 2 * (FieldLength * 2)) == 0)
            {
                Debug.Log($"[Center] {x}, {y}");
            }

            obj.name = $"Block[{blocks.Count}](Pos:{x}, {y})";

            blocks.Add(obj);
        }

        cursor = Instantiate(cursorPrefab);
        cursor.SetBlockData(blocks, FieldLength);
        // カーソルの初期位置を設定
        cursor.PositionX = FieldLength / 2;
        cursor.PositionY = FieldLength / 2;

        var settings = new FieldSettings { BlockRegenerateFrame = BlockRegenerateTime, FieldWidth = FieldLength };
        fieldManager = new FieldManager(blocks, settings);

        // キー操作の定義

        // カーソルを左へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.LEFT, (frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapLeftBlock();
            }
            else
            {
                cursor.Left();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.LEFT, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.LEFT, KeyRepeatInterval, null);

        // カーソルを右へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.RIGHT, (frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapRightBlock();
            }
            else
            {
                cursor.Right();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.RIGHT, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.RIGHT, KeyRepeatInterval, null);

        // カーソルを上へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.UP, (frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapUpBlock();
            }
            else
            {
                cursor.Up();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.UP, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.UP, KeyRepeatInterval, null);

        // カーソルを下へ移動
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.DOWN, (frames) =>
        {
            if (cursor.Hold)
            {
                cursor.SwapDownBlock();
            }
            else
            {
                cursor.Down();
            }
        });
        PlayerInputManager.RegisterOnKeyDelayHandler(KEYS.DOWN, KeyDelayFrame, null);
        PlayerInputManager.RegisterOnKeyHoldHandler(KEYS.DOWN, KeyRepeatInterval, null);

        // ブロックを選択
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.BLOCKCHANGE_A, (frames) => cursor.Hold = true);
        PlayerInputManager.RegisterOnUpHandler(KEYS.BLOCKCHANGE_A, (frames) => cursor.Hold = false);


        // スコアを獲得したときの処理
        fieldManager.onEarnedPointEvent += (r) =>
        {
            Debug.Log($"{r.Reason} {r.Point} {r.Kind}");
            Score += r.Point;
            backgroundContoroller.ChangeTheme(r.Kind);
        };

        fieldManager.onBlockGenerate += (r) =>
        {
            audioSource.PlayOneShot(BlockRegenerateSE);
        };
    }

    // Update is called once per frame
    void Update()
    {
        FieldUpdate();
        ScoreUpdate();
        TimeUpdate();

        if (GameTimeInMilliSeconds <= 0)
        {
            SceneManager.LoadScene("title");
        }
    }

    void FieldUpdate()
    {
        fieldManager.Update();
    }

    private void ScoreUpdate()
    {
        if (score > showScore)
        {
            showScore += 5;
            ScoreText.text = showScore.ToString(ScoreTextFormat);
        }

        // ハイスコアを上回ったら更新
        if (score > hiScore)
        {
            hiScore = showScore;
            HiScoreText.text = showScore.ToString(ScoreTextFormat);
        }
    }

    // 残り時間の更新
    private void TimeUpdate()
    {
        if (GameTimeInMilliSeconds > 0)
        {
            GameTimeInMilliSeconds -= 16;
            TimeText.text = MilliSecondsToString.format(GameTimeInMilliSeconds);
        }
    }

    Vector3 GetBlockPosition(int x, int y)
    {
        return  blocks.PositionAt(x, y, FieldLength).transform.position;
    }
}