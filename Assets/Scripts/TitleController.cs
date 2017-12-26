using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {

    public InputManager Input;
    public Canvas UIParent;
    public GameObject Cursor;
    public Color SelectedColor;
    public Color UnselectedColor;
    public Image Curtain;

    public AudioClip Move;
    public AudioClip Enter;

    private AudioSource audioSource;
    private List<Text> UIs = new List<Text>();

    private int selectedIndex = -1;

    public int SelectedIndex
    {
        get
        {
            return selectedIndex;
        }

        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                // 選択項目が変わったら色を変える
                UIs.ForEach(v => v.color = UnselectedColor);
                UIs[selectedIndex].color = SelectedColor;
                Cursor.transform.position = UIs[selectedIndex].transform.position;
            }
        }
    }


    // Use this for initialization
    void Start () {
        UIs = UIParent.GetComponentsInChildren<Text>().ToList();
        Input.EnableHoldHandler = false;

        Input.RegisterOnKeyDownHandler(KEYS.UP, (frames) =>
        {
            audioSource.PlayOneShot(Move);
            SelectedIndex = SelectedIndex - 1 <= -1 ? UIs.Count - 1 : SelectedIndex - 1;   
        });

        Input.RegisterOnKeyDownHandler(KEYS.DOWN, (frames) =>
        {
            audioSource.PlayOneShot(Move);
            SelectedIndex = (SelectedIndex + 1) % UIs.Count;
        });

        Input.RegisterOnKeyDownHandler(KEYS.BLOCKCHANGE_A, (frames) =>
        {
            audioSource.PlayOneShot(Enter);
            IEnumerator coroutine = null;

            switch(UIs[SelectedIndex].name)
            {
                case "GameStart":
                    coroutine = Fade(0.05f, ()=>
                    {
                        SceneManager.LoadScene("main");
                    });
                    break;
                case "Quit":
                    coroutine = Fade(0.01f, () =>
                    {
                        Application.Quit();
                    });
                    break;
            }

            if (coroutine != null)
            {
                StartCoroutine(coroutine);
            }
        });

        audioSource = GetComponent<AudioSource>();
        SelectedIndex = 0;
    }

    // Update is called once per frame
    void Update () {
		
	}

    delegate void ThenFunc();
    IEnumerator Fade(float delta = 0.01f, ThenFunc then = null)
    {
        for (var f = 0f; f <= 1f; f += delta)
        {
            var c = Curtain.color;
            c.a = f;
            Curtain.color = c;
            yield return null;
        }
        then?.Invoke();
    }
}
