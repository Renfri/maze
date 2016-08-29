using UnityEngine;
using System.Collections;

public class Hexagon : MonoBehaviour {
       /*             3
        *         ________
        *        /        \
        *     2 /          \ 4
        *      /            \
        *      \            /
        *     1 \          / 5
        *        \________/ 
        *             0
        */


    // public:
    public Hexagon(
        GameObject wall,
        Vector3 middle,
        Vector3 rotation)
    {
        Initialize(wall, middle);
        Middle = middle;
        /*Triangles[0] = new Triangle(wall, new Vector3(0.0f, 0.0f, -(wallWidth * Mathf.Sqrt(3) * 2) / 6.0f), new Vector3(0.0f, 0.0f, 0.0f));
        Triangles[1] = new Triangle(wall, new Vector3(-wallWidth / 2.0f, 0.0f, -(wallWidth * Mathf.Sqrt(3)) / 6.0f), new Vector3(0.0f, 60.0f, 0.0f));
        Triangles[2] = new Triangle(wall, new Vector3(-wallWidth / 2.0f, 0.0f, (wallWidth * Mathf.Sqrt(3)) / 6.0f), new Vector3(0.0f, 120.0f, 0.0f));
        Triangles[3] = new Triangle(wall, new Vector3(0.0f, 0.0f, (wallWidth * Mathf.Sqrt(3) * 2) / 6.0f), new Vector3(0.0f, 180.0f, 0.0f));
        Triangles[4] = new Triangle(wall, new Vector3(wallWidth / 2.0f, 0.0f, (wallWidth * Mathf.Sqrt(3)) / 6.0f), new Vector3(0.0f, 240.0f, 0.0f));
        Triangles[5] = new Triangle(wall, new Vector3(wallWidth / 2.0f, 0.0f, -(wallWidth * Mathf.Sqrt(3)) / 6.0f), new Vector3(0.0f, 300.0f, 0.0f));*/
        HoldTriangles();
        SetHexagonOnRightPosition(middle, rotation);
        SetNeighbourhood();
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

    public void DeletePartOfHexagon(int[] ids)
    {
        Debug.Assert(ids.Length < 6, "too many ids");

        foreach (int i in ids)
        {
            Triangles[i].Delete();
            Triangles[i] = null;
        }
    }

    // private:
    private Triangle[] triangles = new Triangle[6];
    private GameObject holder = new GameObject();
    private Vector3 middle;
    private float wallWidth;

    private void SetNeighbourhood()
    {
        for (int i = 0; i < Triangles.Length - 1; i++)
        {
            Triangles[i].Neighbours[Triangle.leftLeg] = Triangles[i + 1];
            Triangles[Triangles.Length - i - 1].Neighbours[Triangle.rightLeg] = Triangles[Triangles.Length - i - 2];
        }
        Triangles[5].Neighbours[Triangle.leftLeg] = Triangles[0];
        Triangles[0].Neighbours[Triangle.rightLeg] = Triangles[5];
    }

    private void SetHexagonOnRightPosition(
        Vector3 middle,
        Vector3 rotation)
    {
        Holder.transform.position += middle;
        Holder.transform.Rotate(rotation);
    }

    private void HoldTriangles()
    {
        for (int i = 0; i < Triangles.Length; i++)
        {
            Triangles[i].Holder.transform.parent = Holder.transform;
        }
    }

    private void Initialize(GameObject wall, Vector3 middle)
    {
        wallWidth = wall.transform.lossyScale.z;
        Holder.name = "Hexagon";
        Middle = middle;
    }
}
