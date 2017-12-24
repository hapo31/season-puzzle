using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public delegate void OnKeyDown(int frames = 1);
    public delegate void OnKeyDelay(int frames);
    public delegate void OnKeyHold(int frames);
    public delegate void OnKeyUp(int frames);

    public bool EnableHoldHandler { get; set; } = true;

    private delegate bool PushButtonExpr();

    private OnKeyDown[] OnKeyDownDelegates = new OnKeyDown[KEYS.KEY_MAX_COUNT.ToInt()];
    private KeyActionWithFrameInfo<OnKeyDelay>[] OnKeyDelayDelegates = new KeyActionWithFrameInfo<OnKeyDelay>[KEYS.KEY_MAX_COUNT.ToInt()];
    private KeyActionWithFrameInfo<OnKeyHold>[] OnKeyHoldDelegates = new KeyActionWithFrameInfo<OnKeyHold>[KEYS.KEY_MAX_COUNT.ToInt()];
    private OnKeyUp[] OnKeyUpDelegates = new OnKeyUp[KEYS.KEY_MAX_COUNT.ToInt()];

    private int[] keyFrames = new int[KEYS.KEY_MAX_COUNT.ToInt()];

    /// <summary>
    ///  指定したボタンが押された瞬間に呼び出されるハンドラを登録する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="action">実行するアクション</param>
    public void RegisterOnKeyDownHandler(KEYS key, OnKeyDown action)
    {
        OnKeyDownDelegates[key.ToInt()] = action;
    }

    /// <summary>
    ///  指定したボタンが押されたあとHoldハンドラを呼び出す代わりに呼び出すハンドラを登録する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="delayFrames">Holdハンドラの代わりにDelayハンドラを呼び出すフレーム数</param>
    /// <param name="action">実行するアクション</param>
    public void RegisterOnKeyDelayHandler(KEYS key, int delayFrames, OnKeyDelay action)
    {
        OnKeyDelayDelegates[key.ToInt()] = new KeyActionWithFrameInfo<OnKeyDelay> { Action = action, frames = delayFrames };
    }

    /// <summary>
    ///  指定したボタンが押された瞬間に呼び出されるハンドラを登録する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="repeatIntervalFrames">Holdハンドラを呼び出す間隔のフレーム数</param>
    /// <param name="action">実行するアクション</param>
    public void RegisterOnKeyHoldHandler(KEYS key, int repeatIntervalFrames, OnKeyHold action)
    {
        OnKeyHoldDelegates[key.ToInt()] = new KeyActionWithFrameInfo<OnKeyHold> { Action = action, frames = repeatIntervalFrames };
    }

    /// <summary>
    ///  指定したボタンが離された瞬間に呼び出されるハンドラを登録する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="action">実行するアクション</param>
    public void RegisterOnUpHandler(KEYS key, OnKeyUp action)
    {
        OnKeyUpDelegates[key.ToInt()] = action;
    }


    // Use this for initialization
    void Start () {
		
	}

    void FrameUpdateAndInvokeHandler(KEYS key, PushButtonExpr expr)
    {
        var keyInt = (int)key;
        if (expr())
        {
            Debug.Log($"{key.ToString()} {keyFrames[keyInt]}");
            keyFrames[keyInt] += 1;
            if (keyFrames[keyInt] == 1)
            {
                //Debug.Log($"[OnDown] key:{key} {keyFrames[keyInt]}");
                // ボタンが押された瞬間にハンドラを呼び出す
                OnKeyDownDelegates[keyInt]?.Invoke();
            }
            else if (keyFrames[keyInt] < OnKeyDelayDelegates[keyInt].frames)
            {
                // 入力判定を発生させないディレイフレーム数が設定されている場合は、代わりにOnKeyDelayハンドラを呼び出す
                //Debug.Log($"[OnDelay] key:{key} {keyFrames[keyInt]}");
                OnKeyDelayDelegates[keyInt].Action?.Invoke(keyFrames[keyInt]);
            }
            else if (IsHoldInvoking(key)) 
            {
                // キーが押されている間呼び出す
                //Debug.Log($"[OnHold] key:{key} {keyFrames[keyInt]}");
                if (EnableHoldHandler && OnKeyHoldDelegates[keyInt].Action == null)
                {
                    // Holdハンドラがnullの場合はOnKeyDownハンドラを呼び出す
                    OnKeyDownDelegates[keyInt]?.Invoke(keyFrames[keyInt]);
                }
                else
                {
                    // Holdハンドラが設定されている場合はそのまま呼び出す
                    OnKeyHoldDelegates[keyInt].Action?.Invoke(keyFrames[keyInt]);
                }
            }
        }
        else if(keyFrames[keyInt] != 0)
        {
            // ボタンが離された瞬間のハンドラを呼び出す
            OnKeyUpDelegates[keyInt]?.Invoke(keyFrames[keyInt]);
            // フレーム数をリセット
            //Debug.Log($"[OnUp] key:{key} {keyFrames[keyInt]}");
            keyFrames[keyInt] = 0;
        }
    }
	
    // OnHoldハンドラを呼び出すべきかどうかを判定する
    private bool IsHoldInvoking(KEYS key) 
    {
        var keyInt = key.ToInt();
        if (OnKeyHoldDelegates[keyInt].frames == 0) { return true; }
        var delay = OnKeyDelayDelegates[keyInt].frames == -1 ? 0 : OnKeyDelayDelegates[keyInt].frames;

        return (keyFrames[keyInt] - 1 - delay) % OnKeyHoldDelegates[keyInt].frames == 0;
    }

	// Update is called once per frame
	void Update () {

        FrameUpdateAndInvokeHandler(KEYS.LEFT, () => Input.GetAxis("Horizontal") == -1.0f);
        FrameUpdateAndInvokeHandler(KEYS.RIGHT, () => Input.GetAxis("Horizontal") == 1.0f);
        FrameUpdateAndInvokeHandler(KEYS.DOWN, () => Input.GetAxis("Vertical") == -1.0f);
        FrameUpdateAndInvokeHandler(KEYS.UP, () => Input.GetAxis("Vertical") == 1.0f);
        FrameUpdateAndInvokeHandler(KEYS.BLOCKCHANGE_A, () => Input.GetButton("BlockChangeA"));
        FrameUpdateAndInvokeHandler(KEYS.BLOCKCHANGE_B, () => Input.GetButton("BlockChangeB"));
        
    }


    public struct KeyActionWithFrameInfo<DelegateType>
    {
        public DelegateType Action;
        public int frames;
        KeyActionWithFrameInfo(DelegateType action, int frames = 0)
        {
            Action = action;
            this.frames = frames;
        }
    }
}

static class KeysExtend
{
    public static int ToInt(this KEYS key) { return (int)key; }
}

public enum KEYS
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    BLOCKCHANGE_A,
    BLOCKCHANGE_B,
    KEY_MAX_COUNT
}