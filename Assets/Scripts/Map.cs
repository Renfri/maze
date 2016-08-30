using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public class Map : MonoBehaviour
{
    // public:
    public Map(
        GameObject wall,
        Vector3 middle,
        uint mazeSize)
    {
        Initialize(wall, middle, mazeSize);

        Debug.Assert(mazeSize > 0, "mazeSize is out of range");

        int numberOfTriangles = 6;
        for (int i = 0; i < mazeSize; i++)
        {
            numberOfTriangles += 6 * (2 * i + 1);
        }
        Triangles = new Triangle[numberOfTriangles];

        int index = 0;
        CreateMap(index);

        SetNeighbourhood();

        Debug.Log("End of creation");
    }

    public GameObject Holder
    {
        get
        {
            return holder;
        }

        set
        {
            holder = value;
        }
    }

    public Vector3 Middle
    {
        get
        {
            return middle;
        }

        set
        {
            middle = value;
        }
    }

    public GameObject Wall
    {
        get
        {
            return wall;
        }

        set
        {
            wall = value;
        }
    }

    public Triangle[] Triangles
    {
        get
        {
            return triangles;
        }

        set
        {
            triangles = value;
        }
    }

    //private:
    public Triangle[] triangles;
    private Dictionary<Vector3, int> trianglesIndices = new Dictionary<Vector3, int>();
    private int roundingHelper = 1000;
    private GameObject holder = new GameObject();
    private GameObject wall;
    private Vector3 middle;
    private float wallWidth;
    private uint mazeSize;

    private void Initialize(GameObject wall, Vector3 middle, uint size)
    {
        wallWidth = wall.transform.lossyScale.z;
        Holder.name = "Map";
        Middle = middle;
        mazeSize = size;
        Wall = wall;
    }

    private void CreateMap(
        int index)
    {
        Queue<Triangle> triangleQueue = new Queue<Triangle>();

        CreateFirstLevelOfTriangles(triangleQueue, ref index);

        //common
        Triangle currentTriangle;
        int yRotation;
        int neighbourYRotation;

        //single triangle
        Triangle.Drawing shouldBeDrawn;
        Vector3 neighbourPosition;

        while (triangleQueue.Count > 0)
        {
            currentTriangle = triangleQueue.Dequeue();
            Debug.Assert(currentTriangle, "Triangle doesn't exist");
            yRotation = (int)Mathf.Round(currentTriangle.Rotation.y);
            neighbourYRotation = (yRotation + 180) % 360;
            shouldBeDrawn = currentTriangle.ShouldBeDrawn == Triangle.Drawing.DrawAll ? Triangle.Drawing.NoDraw : Triangle.Drawing.DrawAll;

            if (yRotation == (int)Mathf.Round(currentTriangle.Sector * 60.0f))
            {
                if (currentTriangle.Level >= mazeSize)
                {
                    continue;
                }

                neighbourPosition = currentTriangle.GetNeighbourPosition(Triangle.Direction.TriangleBase);
                MakeTriangle(triangleQueue, neighbourPosition, neighbourYRotation, currentTriangle.Level + 1, currentTriangle.Sector, shouldBeDrawn, ref index);
            }
            else
            {
                if (currentTriangle.Level == mazeSize)
                {
                    shouldBeDrawn = yRotation % 120 == 0 ? Triangle.Drawing.DrawBase : Triangle.Drawing.DrawAll;
                }

                neighbourPosition = currentTriangle.GetNeighbourPosition(Triangle.Direction.LeftLeg);
                MakeTriangle(triangleQueue, neighbourPosition, neighbourYRotation, currentTriangle.Level, currentTriangle.Sector, shouldBeDrawn, ref index);

                neighbourPosition = currentTriangle.GetNeighbourPosition(Triangle.Direction.RightLeg);
                MakeTriangle(triangleQueue, neighbourPosition, neighbourYRotation, currentTriangle.Level, currentTriangle.Sector, shouldBeDrawn, ref index);
            }
        }
    }

    private void CreateFirstLevelOfTriangles(
        Queue<Triangle> triangleQueue,
        ref int index)
    {
        float x, z;
        Vector3 triangleMiddle;
        int level = 1;
        int sector;
        Triangle.Drawing shouldBeDrawn;

        for (int i = 0; i < 6; i++)
        {
            x = Common.GetAxisCoordinate(wallWidth, i * 60, Mathf.Sin);
            z = Common.GetAxisCoordinate(wallWidth, i * 60, Mathf.Cos);
            triangleMiddle = new Vector3(x + Middle.x, 0.0f, z + Middle.z);

            sector = i;
            shouldBeDrawn = (i % 2) == 0 ? Triangle.Drawing.DrawAll : Triangle.Drawing.NoDraw;

            MakeTriangle(triangleQueue, triangleMiddle, i * 60.0f, level, sector, shouldBeDrawn, ref index);
        }
    }

    private void MakeTriangle(
        Queue<Triangle> triangleQueue,
        Vector3 triangleMiddle,
        float yRotation,
        int level,
        int sector,
        Triangle.Drawing shouldBeDrawn,
        ref int index)
    {
        Vector3 roundedMiddle = new Vector3(Mathf.Round(triangleMiddle.x * roundingHelper), 0.0f, Mathf.Round(triangleMiddle.z * roundingHelper));

        if (!trianglesIndices.ContainsKey(roundedMiddle))
        {
            Vector3 triangleRotation = new Vector3(0.0f, yRotation, 0.0f);

            Triangles[index] = new Triangle(Wall, triangleMiddle, triangleRotation, level, sector, shouldBeDrawn);
            trianglesIndices.Add(roundedMiddle, index);
            triangleQueue.Enqueue(Triangles[index++]);
        }
        else
        {
            Debug.Log("Triangle already exists");
        }
    }

    private int GetTriangleIndex(
        Vector3 position)
    {
        Vector3 roundedMiddle = new Vector3(Mathf.Round(position.x * roundingHelper), 0.0f, Mathf.Round(position.z * roundingHelper));
        return trianglesIndices.ContainsKey(roundedMiddle) ? trianglesIndices[roundedMiddle] : - 1;
    }

    private void SetNeighbourhood()
    {
        Vector3 neighbourPosition;
        int neighbourIndex;
        Debug.Assert(Triangles[0], "Triangles don't exist.");
        int numberOfNeighbours = Triangles[0].Neighbours.Length;
        //GameObject neighbour; // TODO: delete
        //int j = 0; // TODO: delete

        foreach (Triangle currentTriangle in Triangles)
        {
            for (int i = 0; i < numberOfNeighbours; i++)
            {
                neighbourPosition = currentTriangle.GetNeighbourPosition((Triangle.Direction)i);
                neighbourIndex = GetTriangleIndex(neighbourPosition);
                if(neighbourIndex >= 0)
                {
                    //neighbour = new GameObject(); // TODO: delete
                    //neighbour.transform.position = neighbourPosition; // TODO: delete
                    //neighbour.name = "Neighbour" + j; // TODO: delete
                    currentTriangle.Neighbours[i] = Triangles[neighbourIndex];
                }

            }
            //j++; // TODO: delete
        }
    }
}