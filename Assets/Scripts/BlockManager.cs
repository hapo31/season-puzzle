using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    public Block blockPrefab;

    private List<Block> blocks = new List<Block>();


    private const float BASEY = 4.0f;
    private const float BASEX = 7.0f;
    private const float BASELENGTH = 2.6f;

    public float FieldLength;

	// Use this for initialization
	void Start () {

        for (var i = 0; i < FieldLength * FieldLength; ++i)
        {
            var obj = Instantiate<Block>(blockPrefab);
            float x = i % FieldLength * BASELENGTH * BASEX;
            float y = i / FieldLength * BASELENGTH * BASEY;
            Debug.Log("x:" + x.ToString() + " y:" + y.ToString());
            obj.transform.position = new Vector3(x, y);
            obj.Kind = Block.GetRandomBlockKind();

            blocks.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
