using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    public Block blockPrefab;
    public Sprite BackgroundSprite;

    private List<Block> blocks = new List<Block>();
    private List<Sprite> background = new List<Sprite>();

    public float OffsetXPosition;
    public float OffsetYPosition;

    private const float BASEY = 4.0f;
    private const float BASEX = 7.0f;
    private const float BASELENGTH = 2.6f;

    public int FieldLength;

	// Use this for initialization
	void Start () {

        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate(blockPrefab);

            float x = i % FieldLength / (BASELENGTH * 2) * BASEX + OffsetXPosition;
            float y = i / FieldLength / BASELENGTH * BASEY + OffsetYPosition;
            Debug.Log($"x:{x} y:{y}");
            obj.transform.position = new Vector3(x, y);
            obj.Kind = Block.GetRandomBlockKind(true);
            
            blocks.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {
        var target = Random.Range(0, blocks.Count - 1);
        var kind = Block.GetRandomBlockKind();

        blocks[target].Kind = kind;
	}
}
