using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Util;
using Assets.Scripts.Util;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Block blockPrefab;
    public Cursor cursorPrefab;

    public InputManager PlayerInputManager;
    public int FieldLength;
    public float OffsetXPosition;
    public float OffsetYPosition;
    public int GameReadyFrames = 200;
    public int GameTimeInMilliSeconds = 1000 * 60 * 2; // 2分
    public AudioClip BlockDelete;
    public AudioClip Ready;
    public AudioClip Go;
    public AudioClip HiScoreUpdateSE;
    public AudioClip FinishSE;
    public AudioClip BlockRegenerateSE;

    public Text ScoreText;
    public Text HiScoreText;
    public Text TimeText;

    public SpriteRenderer Curtain;

    private AudioSource bgm;
    private AudioSource audioSource;

    private List<ParticleSystem> hiscoreEffects;

    private bool startFlag = false;
    private bool readyFlag = false;

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

    private const string HISCORE = "hiscore";

    private SaveDataManager saveDataManager = new SaveDataManager();
    private SaveData savedata;

    // 表示用スコア
    private int showScore = 0;
    // 実際の内部スコア
    private int score = 0;
    // ハイスコア
    private int hiScore = 0;

    // ハイスコア更新フラグ
    private bool hiScoreUpdated = false;
    // ゲーム終了フラグ
    private bool finished = false;

    private int Score
    {
        get { return score; }
        set
        {
            score = value;
        }
    }

    private int RestGameFrames { get { return GameTimeInMilliSeconds / 16; } }

    // Use this for initialization
    void Start()
    {

        startFlag = false;
        readyFlag = false;
        showScore = 0;
        score = 0;

        hiScoreUpdated = false;

        finished = false;


        ScoreTextFormat = new string('0', ScoreText.text.Length);
        var audio = GetComponents<AudioSource>();
        audioSource = audio[0];
        bgm = audio[1];

        hiscoreEffects = GameObject.FindGameObjectWithTag("HiScoreEffect").GetComponentsInChildren<ParticleSystem>().ToList();

        // ハイスコア読み込み処理
        savedata = saveDataManager.Load() ?? new SaveData();
        hiScore = savedata.HiScore;

        HiScoreText.text = hiScore.ToString(ScoreTextFormat);

        // 時間表示の更新
        TimeText.text = MilliSecondsToString.format(GameTimeInMilliSeconds);
        // 背景コントローラーの取得
        backgroundContoroller = GameObject.Find("InfomationBackground").GetComponent<BackgroundController>();

        Score = 0;
        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate(blockPrefab);

            float x = i % FieldLength / (BASELENGTH * 2) * BASEX + OffsetXPosition;
            float y = -(i / FieldLength / BASELENGTH * BASEY + OffsetYPosition);
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
        PlayerInputManager.RegisterOnKeyDownHandler(KEYS.BLOCKCHANGE_A, (frames) =>
        {
            if (!startFlag)
            {
                return;
            }
            cursor.Hold = true;
        });
        PlayerInputManager.RegisterOnUpHandler(KEYS.BLOCKCHANGE_A, (frames) =>
        {
            if (!startFlag)
            {
                return;
            }
            cursor.Hold = false;
        });


        // スコアを獲得したときの処理
        fieldManager.onEarnedPointEvent += (r) =>
        {
            Debug.Log($"{r.Reason} {r.Point} {r.Kind}");
            StartCoroutine("BlockDeletePlay", r.Count);
            Score += r.Point;
            backgroundContoroller.ChangeTheme(r.Kind);
        };

        fieldManager.onBlockGenerate += (r) =>
        {
            audioSource.PlayOneShot(BlockRegenerateSE);
        };

        var c = FadeIn(0.05f, () =>
        {
            Debug.Log("Fade finish");
            readyFlag = true;
        });

        StartCoroutine(c);
    }

    // Update is called once per frame
    void Update()
    {
        // 開始前
        if (!startFlag)
        {
            if (!readyFlag)
            {
                Debug.Log("Ready");
                readyFlag = true;
            }
            else
            {
                // ﾎﾟｯﾎﾟｯﾎﾟｯというレース前みたいなSEを鳴らす
                if (GameReadyFrames % 60 == 0 && GameReadyFrames > 0)
                {
                    audioSource.PlayOneShot(Ready);
                    Debug.Log($"{GameReadyFrames}");
                }

                // ゲーム開始フラグを立てる
                if (GameReadyFrames <= 0)
                {
                    audioSource.PlayOneShot(Go);
                    bgm.Play();
                    Debug.Log("Start");
                    startFlag = true;
                }
                else
                {
                    --GameReadyFrames;
                }
            }
        }
        else
        {
            if (!finished)
            {
                // 10秒前になったらカウントダウンする
                if (RestGameFrames > 0 && RestGameFrames <= 600 && RestGameFrames % 60 == 0)
                {
                    audioSource.PlayOneShot(Ready);
                }

                FieldUpdate();
                ScoreUpdate();
                TimeUpdate();

                // 残り時間がなくなったらゲーム終了
                if (GameTimeInMilliSeconds <= 0)
                {
                    finished = true;
                    // ゲーム終了時にもう一度スコア更新を行い、表示とのズレを無くす
                    ScoreUpdate();
                    audioSource.PlayOneShot(FinishSE);
                    // ハイスコア更新フラグが立っていたら保存
                    if (hiScoreUpdated)
                    {
                        savedata.HiScore = score;
                        saveDataManager.Save(savedata);
                    }

                    // フェードアウト
                    var c = FadeOut(0.01f, () =>
                    {
                        SceneManager.LoadScene("title");
                    });

                    StartCoroutine(c);
                }
            }
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

            // ゲームが終了していたら即代入する
            if (finished)
            {
                showScore = score;
                ScoreText.text = showScore.ToString(ScoreTextFormat);
            }
        }

        // ハイスコアを上回ったら更新
        if (score > hiScore)
        {
            hiScore = showScore;
            HiScoreText.text = showScore.ToString(ScoreTextFormat);
            if (!hiScoreUpdated && hiScore > 0) // 初期スコア時は鳴らさない
            {
                Debug.Log("HiScore Updated");
                // ハイスコア更新のSEを鳴らす
                audioSource.PlayOneShot(HiScoreUpdateSE);
                hiscoreEffects[0].Play();
                hiscoreEffects[1].Play();
            }
            hiScoreUpdated = true;
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
        return blocks.PositionAt(x, y, FieldLength).transform.position;
    }

    delegate void ThenFunc();
    IEnumerator FadeOut(float delta = 0.01f, ThenFunc then = null)
    {
        for (var f = 0f; f <= 1f; f += delta)
        {
            var c = Curtain.color;
            c.a = f;
            Curtain.color = c;
            Debug.Log($"{f}");
            yield return null;
        }
        then?.Invoke();
    }

    IEnumerator FadeIn(float delta = 0.01f, ThenFunc then = null)
    {
        for (var f = 1f; f <= 1f; f -= delta)
        {
            var c = Curtain.color;
            c.a = f;
            Curtain.color = c;
            yield return null;
        }
        then?.Invoke();
    }

    IEnumerator BlockDeletePlay(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            audioSource.PlayOneShot(BlockDelete);
            yield return new WaitForSeconds(0.1f);
        }
    }
}