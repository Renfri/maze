using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

    //jakies protected?

    public enum Orientation
    {
        pointyTopped,
        flatTopped
    }

    //posprawdzac pub/priv/prot
    public GameObject wallPrefab;

    private float edgeLength = 10.0F; // zmienić na 1 ;) przniesc do jakiegos configa!! obowiazkowo bo tego nie powinno sie dziedziczyc!
    private GameObject _basement, _leftArm, _rightArm;
    private IntVector2 _position;
    private Orientation _orientation = Orientation.pointyTopped;
    private const int INTERNAL_ANGLE = 60; // jakas klasa/interfejs na konsty?
    private const int STRAIGHT_ANGLE = 180;

    void Awake ()
    {
        // zmienic pozycje! +1m lel zmienic liczby z dupy na jakies nazwy ladne jak INTERNAL ANGLE
        //przeniesc czesc do Walla raczej
        _basement = Instantiate(wallPrefab) as GameObject;
        _basement.transform.parent = transform;
        _basement.transform.localPosition = new Vector3(0, 0, -3.830127F);

        _leftArm = Instantiate(wallPrefab) as GameObject;
        _leftArm.transform.parent = transform;
        _leftArm.transform.localPosition = new Vector3(-2.15F, 0, 0);
        _leftArm.transform.eulerAngles = new Vector3(0, -INTERNAL_ANGLE, 0);

        _rightArm = Instantiate(wallPrefab) as GameObject;
        _rightArm.transform.parent = transform;
        _rightArm.transform.localPosition = new Vector3(2.15F, 0, 0);
        _rightArm.transform.eulerAngles = new Vector3(0, INTERNAL_ANGLE, 0);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //GET LEFT NEIGHBOUR w klasie map?

    public IntVector2 position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;

            //dodacHeightDoEdita? YEP! Nope? Config? Przeyskutowac lele :P
            float VERTICAL_SIZE = edgeLength / 2 * Mathf.Sqrt(3);
            float HORIZONTAL_SIZE = edgeLength / 2;    
            gameObject.transform.position = new Vector3(_position.x * HORIZONTAL_SIZE, 0, _position.y * VERTICAL_SIZE); //na to funkcje tysz!

            if ((int)(_position.x + _position.y) % 2 == 0) //dodac funkcje na to
            {
                orientation = Orientation.pointyTopped;
            }
            else
            {
                orientation = Orientation.flatTopped;
            }
        }
    }

    public Orientation orientation
    {
        get
        {
            return _orientation;
        }

        private set
        {
            if(IsChangingOrientation(value))
            {
                _orientation = value;
                transform.Rotate(new Vector3(0, STRAIGHT_ANGLE, 0));
                wallSwapActive(_leftArm, _rightArm);
            }
        }
    }

    public bool basementActive
    {
        get
        {
            return _basement.activeSelf;
        }
        set
        {
            _basement.SetActive(value);
        }
    }

    public bool leftWallActive
    {
        get
        {
            if (_orientation == Orientation.pointyTopped)
                return _leftArm;
            else
                return _rightArm;
        }
        set
        {
            if (_orientation == Orientation.pointyTopped)
                _leftArm.SetActive(value);
            else
                _rightArm.SetActive(value);
        }
    }

    public bool rightWallActive
    {
        get
        {
            if (_orientation == Orientation.pointyTopped)
                return _rightArm;
            else
                return _leftArm;
        }
        set
        {
            if (_orientation == Orientation.pointyTopped)
                _rightArm.SetActive(value);
            else
                _leftArm.SetActive(value);
        }
    }

    private bool IsChangingOrientation(Orientation newOrientation)
    {
        if (_orientation != newOrientation)
            return true;
        else
            return false;
    }

    private void wallSwapActive(GameObject firstWall, GameObject secondWall) // do walla?
    {
        bool firstWallActive = firstWall.activeSelf;
        firstWall.SetActive(secondWall.activeSelf);
        secondWall.SetActive(firstWallActive);
    }
}
