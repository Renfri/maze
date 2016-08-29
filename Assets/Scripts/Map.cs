using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public class Map : MonoBehaviour
{
    Triangle[] Triangles;
    Dictionary<Vector3, int> TrianglesIndices = new Dictionary<Vector3, int>();
    int roundingHelper = 1000;

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
        CreateMap(wall, index);

        Debug.Log("asd");
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

    //private:
    private GameObject holder = new GameObject();
    private Vector3 middle;
    private float wallWidth;
    private uint mazeSize;

    private void Initialize(GameObject wall, Vector3 middle, uint size)
    {
        wallWidth = wall.transform.lossyScale.z;
        Holder.name = "Map";
        Middle = middle;
        mazeSize = size;
    }

    private void CreateMap(
        GameObject wall,
        int index)
    {
        Queue<Triangle> triangleQueue = new Queue<Triangle>();

        CreateFirstLevelOfTriangles(wall, triangleQueue, ref index);

        //common
        Triangle currentTriangle;
        int yRotation;

        //single triangle
        Vector3 triangleMiddle;
        Vector3 roundedMiddle;
        Vector3 triangleRotation;
        float x;
        float z;

        while (triangleQueue.Count > 0)
        {
            currentTriangle = triangleQueue.Dequeue();
            Debug.Assert(currentTriangle, "Triangle doesn't exist");
            yRotation = (int)Mathf.Round(currentTriangle.Rotation.y);

            if(yRotation == (int)Mathf.Round(currentTriangle.Sector * 60.0f))
            {
                if (currentTriangle.Level >= mazeSize)
                {
                    continue;
                }

                x = wallWidth * Mathf.Sqrt(3) * -Mathf.Sin(yRotation / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                z = wallWidth * Mathf.Sqrt(3) * -Mathf.Cos(yRotation / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                yRotation = (yRotation + 180) % 360;

                triangleMiddle = new Vector3(x, 0.0f, z) + currentTriangle.Middle;                // TODO: needed refactoring - MakeTriangle function
                triangleRotation = new Vector3(0.0f, yRotation, 0.0f);

                Triangles[index] = new Triangle(wall, triangleMiddle, triangleRotation, currentTriangle.Level + 1, currentTriangle.Sector, !currentTriangle.ShouldBeDrawn);
                triangleQueue.Enqueue(Triangles[index]);

                roundedMiddle = new Vector3(Mathf.Round(triangleMiddle.x * roundingHelper), 0.0f, Mathf.Round(triangleMiddle.z * roundingHelper));
                TrianglesIndices.Add(roundedMiddle, index++);
            }
            else
            {
                // 1
                x = wallWidth * Mathf.Sqrt(3) * -Mathf.Sin((yRotation + 120) / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                z = wallWidth * Mathf.Sqrt(3) * -Mathf.Cos((yRotation + 120) / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                yRotation = (yRotation + 180) % 360;
                triangleMiddle = new Vector3(x, 0.0f, z) + currentTriangle.Middle;                // TODO: needed refactoring - MakeTriangle function

                roundedMiddle = new Vector3(Mathf.Round(triangleMiddle.x * roundingHelper), 0.0f, Mathf.Round(triangleMiddle.z * roundingHelper));

                if (!TrianglesIndices.ContainsKey(roundedMiddle))
                {
                    triangleRotation = new Vector3(0.0f, yRotation, 0.0f);

                    Triangles[index] = new Triangle(wall, triangleMiddle, triangleRotation, currentTriangle.Level, currentTriangle.Sector, !currentTriangle.ShouldBeDrawn);
                    triangleQueue.Enqueue(Triangles[index]);

                    TrianglesIndices.Add(roundedMiddle, index++);
                }
                else
                {
                    Debug.Log("No chyba kpisz");
                }

                // 2
                x = wallWidth * Mathf.Sqrt(3) * -Mathf.Sin((yRotation + 60) / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                z = wallWidth * Mathf.Sqrt(3) * -Mathf.Cos((yRotation + 60) / 180.0f * Mathf.PI) / 3.0f; // TODO: needed refactoring - maybe lambda?
                triangleMiddle = new Vector3(x, 0.0f, z) + currentTriangle.Middle;                // TODO: needed refactoring - MakeTriangle function

                roundedMiddle = new Vector3(Mathf.Round(triangleMiddle.x * roundingHelper), 0.0f, Mathf.Round(triangleMiddle.z * roundingHelper));
                
                triangleRotation = new Vector3(0.0f, yRotation, 0.0f);

                Triangles[index] = new Triangle(wall, triangleMiddle, triangleRotation, currentTriangle.Level, currentTriangle.Sector, !currentTriangle.ShouldBeDrawn);
                triangleQueue.Enqueue(Triangles[index]);

                TrianglesIndices.Add(roundedMiddle, index++);
                
            }
        }
    }

    private void CreateFirstLevelOfTriangles(
        GameObject wall,
        Queue<Triangle> triangleQueue,
        ref int index)
    {
        index = 0;

        Vector3 triangleMiddle;
        Vector3 roundedMiddle;
        Vector3 triangleRotation;
        int level = 1;
        int sector;
        bool shouldBeDrawn;
        float x;
        float z;

        for (int i = 0; i < 6; i++)
        {
            x = wallWidth * Mathf.Sqrt(3) * -Mathf.Sin(i * 60.0f / 180.0f * Mathf.PI)  / 3.0f; // TODO: needed refactoring - maybe lambda?
            z = wallWidth * Mathf.Sqrt(3) * -Mathf.Cos(i * 60.0f / 180.0f * Mathf.PI)  / 3.0f; // TODO: needed refactoring - maybe lambda?

            triangleMiddle = new Vector3(x, 0.0f, z) + Middle;                                 // TODO: needed refactoring - MakeTriangle function
            triangleRotation = new Vector3(0.0f, i * 60.0f, 0.0f);
            sector = i;
            shouldBeDrawn = (i % 2) == 0;
            
            Triangles[index] = new Triangle(wall, triangleMiddle, triangleRotation, level, sector, shouldBeDrawn);

            triangleQueue.Enqueue(Triangles[index]);

            roundedMiddle = new Vector3(Mathf.Round(x * roundingHelper), 0.0f, Mathf.Round(z * roundingHelper));
            TrianglesIndices.Add(roundedMiddle, index++);
        }
    }

}


/*int numberOfHexagones = 1;
for (int i = 0; i < size; i++)
{
    numberOfHexagones += 6 * i;
}
Hexagones = new Hexagon[numberOfHexagones];

Hexagones[0] = new Hexagon(wall, middle, new Vector3(0.0f, 0.0f, 0.0f));
HexagonesIndices.Add(new Vector3(0.0f, 0.0f, 0.0f), 0);

Queue<Hexagon> queue = new Queue<Hexagon>();
queue.Enqueue(Hexagones[0]);

CreateMap(wall, queue, 1, numberOfHexagones, 2);
if (mazeSize > 2)
{
    CutOffUnnecessaryTriangles();
}*/

/*private void CreateMap(
    GameObject wall,
    Queue<Hexagon> currentLevelQueue,
    int index,
    int maxSize,
    int levelNumber)
{
    if (levelNumber > mazeSize)
    {
        return;
    }

    Queue<Hexagon> nextLevelQueue = new Queue<Hexagon>();

    foreach (Hexagon hex in currentLevelQueue)
    {
        for (int i = 0; i < maxNeighboursOfHexagon; i++)
        {
            float z = hex.Middle.z + wallWidth * Mathf.Sqrt(3) * Mathf.Cos(i * 60.0f / 180.0f * Mathf.PI);
            float x = hex.Middle.x + wallWidth * Mathf.Sqrt(3) * Mathf.Sin(i * 60.0f / 180.0f * Mathf.PI);
            Vector3 roundedMiddle = new Vector3(Mathf.Round(x * roundingHelper), 0.0f, Mathf.Round(z * roundingHelper));
            if (!HexagonesIndices.ContainsKey(roundedMiddle))
            {
                Debug.Assert(index < maxSize, "index is out of range");
                Hexagones[index] = new Hexagon(wall, new Vector3(x, 0.0f, z), new Vector3(0.0f, 0.0f, 0.0f));
                nextLevelQueue.Enqueue(Hexagones[index]);
                HexagonesIndices.Add(roundedMiddle, index++);
                // TODO: ustaw sasiedztwo dla triangleBases
            }
            else
            {
                // TODO: ustaw sasiedztwo dla triangleBases
            }
        }
    }

    currentLevelQueue.Clear();

    CreateMap(wall, nextLevelQueue, index, maxSize, ++levelNumber);
}

private void CutOffUnnecessaryTriangles()
{
    // TODO: Cut off jest zle zrobiony, poprawny dla mazeSize = 3, dla wyzszych usuwamy caly
    for (int i = 0; i < maxNeighboursOfHexagon; i++)
    {
        float z = (mazeSize - 1) * wallWidth * Mathf.Sqrt(3) * Mathf.Cos(i * 60.0f / 180.0f * Mathf.PI);
        float x = (mazeSize - 1) * wallWidth * Mathf.Sqrt(3) * Mathf.Sin(i * 60.0f / 180.0f * Mathf.PI);
        Vector3 roundedMiddle = new Vector3(Mathf.Round(x * roundingHelper), 0.0f, Mathf.Round(z * roundingHelper));
        if (HexagonesIndices.ContainsKey(roundedMiddle))
        {
            Hexagones[HexagonesIndices[roundedMiddle]].DeletePartOfHexagon(new int[3] { (i + 2) % 6, (i + 3) % 6, (i + 4) % 6 });
        }
    }



}*/
