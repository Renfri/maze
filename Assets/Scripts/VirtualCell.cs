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
        Neighbours = new List<VirtualCell>();
        IsVisited = false;
        IsDeadEnd = false;
        YCoordinate = -1; // yCoordinate is unassigned
        IsNeededTransition = false;
        Type = CellType.WALL;
        TypeOfField = FieldType.NONE;
        Position = pos;
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
        list.Add(new IntVector2(Position.x + 2, Position.y));
        list.Add(new IntVector2(Position.x,     Position.y + 2));
        list.Add(new IntVector2(Position.x - 2, Position.y + 2));
        list.Add(new IntVector2(Position.x - 2, Position.y));
        list.Add(new IntVector2(Position.x,     Position.y - 2));
        list.Add(new IntVector2(Position.x + 2, Position.y - 2));
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
        for (int i = 0; i < Neighbours.Count; i++)
        {
            list.Add(Neighbours[i]);
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

    public bool IsVisited { get; set; }

    public bool IsDeadEnd { get; set; }

    public float YCoordinate { get; set; }

    public bool IsNeededTransition { get; set; }

    public CellType Type { get; set; }

    public FieldType TypeOfField { get; set; }

    public IntVector2 Position { get; set; }

    public List<VirtualCell> Neighbours { get; set; }

    //private:

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
