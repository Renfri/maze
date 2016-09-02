using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {

    //public:
    public GameObject wall;
    public uint mazeSize = 1;

	// Use this for initialization
	void Start () {
        Map map = gameObject.AddComponent<Map>() as Map;
        map.InitializeMap(wall, new Vector3(0.0f, 0.0f, 0.0f), mazeSize);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
}
