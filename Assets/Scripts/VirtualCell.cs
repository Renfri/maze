using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class VirtualCell : MonoBehaviour { // TODO: delete MonoBehaviour in the future
    //public:
    public enum CellType
    {
        WALL,
        FIELD
    }

    public enum FieldType // TODO: change system to more flexible
    {
        BLUE,
        RED,
        GREEN,
        NEUTRAL,
        YELLOW,
        PINK,
        ORANGE,
        COUNT,
        NONE,
    }

    public void Initialize(IntVector2 pos)
    {
        neighbours = new List<VirtualCell>();
        isVisited = false;
        isDeadEnd = false;
        yCoordinate = -1; // yCoordinate is unassigned
        isNeededTransition = false;
        type = CellType.WALL;
        typeOfField = FieldType.NONE;
        position = pos;
    }

    public Vector2 GetXZCoordinates()
    {
        return new Vector2(
                       Position.x * Horizontal + Position.y * Shift,
                       -1 * Position.y * Vertical);
    }

    public Vector3 GetXYZCoordinates()
    {
        return new Vector3(
                       Position.x * Horizontal + Position.y * Shift,
                       YCoordinate,
                       - 1 * Position.y * Vertical);
    }

    public List<IntVector2> GetRandomlyPotentialIndirectNeighbours()
    {
        List<IntVector2> list = new List<IntVector2>();
        list.Add(new IntVector2(position.x + 2, position.y));
        list.Add(new IntVector2(position.x,     position.y + 2));
        list.Add(new IntVector2(position.x - 2, position.y + 2));
        list.Add(new IntVector2(position.x - 2, position.y));
        list.Add(new IntVector2(position.x,     position.y - 2));
        list.Add(new IntVector2(position.x + 2, position.y - 2));
        List<IntVector2> randomList = new List<IntVector2>();

        while(list.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            randomList.Add(list[randomIndex]);
            list.RemoveAt(randomIndex);
        }
        return randomList;
    }

    public List<VirtualCell> GetRandomlyNeighbours()
    {
        List<VirtualCell> list = new List<VirtualCell>();
        for (int i = 0; i < neighbours.Count; i++)
        {
            list.Add(neighbours[i]);
        }
        List<VirtualCell> randomList = new List<VirtualCell>();

        while (list.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            randomList.Add(list[randomIndex]);
            list.RemoveAt(randomIndex);
        }
        return randomList;
    }

    public bool IsVisited
    {
        get
        {
            return isVisited;
        }

        set
        {
            isVisited = value;
        }
    }

    public bool IsDeadEnd
    {
        get
        {
            return isDeadEnd;
        }

        set
        {
            isDeadEnd = value;
        }
    }

    public float YCoordinate
    {
        get
        {
            return yCoordinate;
        }

        set
        {
            yCoordinate = value;
        }
    }

    public bool IsNeededTransition
    {
        get
        {
            return isNeededTransition;
        }

        set
        {
            isNeededTransition = value;
        }
    }

    public CellType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public FieldType TypeOfField
    {
        get
        {
            return typeOfField;
        }

        set
        {
            typeOfField = value;
        }
    }

    public IntVector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    public List<VirtualCell> Neighbours
    {
        get
        {
            return neighbours;
        }

        set
        {
            neighbours = value;
        }
    }

    //private:
    bool isVisited;
    bool isDeadEnd;
    float yCoordinate;
    bool isNeededTransition;
    CellType type;
    FieldType typeOfField;
    IntVector2 position;
    List<VirtualCell> neighbours;

    private static readonly float Horizontal = Mathf.Sqrt(3) / 2;
    private static readonly float Vertical = 0.75F;
    private static readonly float Shift = Horizontal / 2;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
