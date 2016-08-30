using UnityEngine;
using System.Collections;
using System;

public class Triangle : MonoBehaviour
{
    /*
        * Triangle:
        * Middle = { 0.0 }, Rotation = { 0.0 }
        * 
        * Left:
        *  Pos:
        *      x = -a/4
        *      y = 0.0
        *      z = a*sqrt(3)/4 - a*sqrt(3)/6
        *  Rot:
        *      x = 0.0
        *      y = 30.0
        *      z = 0.0    
        * Rigth:
        *  Pos:
        *      x = a/4
        *      y = 0.0
        *      z = a*sqrt(3)/4 - a*sqrt(3)/6
        *  Rot:
        *      x = 0.0
        *      y = 150.0
        *      z = 0.0
        * Base:
        *  Pos:
        *      x = 0.0
        *      y = 0.0
        *      z = -a*sqrt(3)/6
        *  Rot:
        *      x = 0.0
        *      y = 0.0
        *      z = 0.0
        */

    //public:
    public enum Drawing
    {
        NoDraw,
        DrawBase,
        DrawAll
    }

    public enum Direction
    {
        TriangleBase,
        LeftLeg,
        RightLeg
    }


    public Triangle(
        GameObject wall,
        Vector3 middle,
        Vector3 rotation,
        int level,
        int sector,
        Drawing shouldBeDrawn)
    {
        Initialize(wall, middle, rotation, level, sector, shouldBeDrawn);

        if (ShouldBeDrawn != Drawing.NoDraw)
        {
            if (ShouldBeDrawn == Drawing.DrawAll)
            {
                Walls[1] = GetLeftLeg(wall, middle, rotation);
                Walls[2] = GetRightLeg(wall, middle, rotation);
            }
            Walls[0] = GetTriangleBase(wall, middle, rotation);

            HoldWalls();
        }

        SetTriangleOnRightPosition();
    }

    public Vector3 GetNeighbourPosition(
        Direction direction)
    {
        int angle = ((int)Rotation.y + (int)direction * 120) % 360;
        float x = Common.GetAxisCoordinate(wallWidth, angle, Mathf.Sin);
        float z = Common.GetAxisCoordinate(wallWidth, angle, Mathf.Cos);

        return new Vector3(x + Middle.x, 0.0f, z + Middle.z);
    }

    public void Delete()
    {
        foreach (GameObject wall in Walls)
        {
            DestroyObject(wall, 0.0f);
        }

        DestroyObject(Holder, 0.0f);
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

    public GameObject[] Walls
    {
        get
        {
            return walls;
        }

        set
        {
            walls = value;
        }
    }

    public Triangle[] Neighbours
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

    public Vector3 Rotation
    {
        get
        {
            return rotation;
        }

        set
        {
            rotation = value;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
        }
    }

    public int Sector
    {
        get
        {
            return sector;
        }

        set
        {
            sector = value;
        }
    }

    public Drawing ShouldBeDrawn
    {
        get
        {
            return shouldBeDrawn;
        }

        set
        {
            shouldBeDrawn = value;
        }
    }

    //private:
    private GameObject holder = new GameObject();
    private Vector3 middle;
    private Vector3 rotation;
    private GameObject[] walls = new GameObject[3];
    private Triangle[] neighbours = new Triangle[3];
    private float wallWidth = 0;
    private int level;
    private int sector; // TODO: make enum from this
    private Drawing shouldBeDrawn;

    private void Initialize(GameObject wall, Vector3 middle, Vector3 rotation, int level, int sector, Drawing shouldBeDrawn)
    {
        Debug.Assert(level >= 0);
        Debug.Assert(sector >= 0);
        wallWidth = wall.transform.lossyScale.z;
        Holder.name = "Triangle";
        Middle = middle;
        Rotation = rotation;
        Level = level;
        Sector = sector;
        ShouldBeDrawn = shouldBeDrawn;
    }

    private GameObject GetLeftLeg(
        GameObject wall,
        Vector3 middle,
        Vector3 rotation)
    {
        Vector3 leftLegPos = new Vector3(-wallWidth / 4.0f, 0.0f, (wallWidth * Mathf.Sqrt(3.0f)) / 12.0f);
        Vector3 leftLegRotation = new Vector3(0.0f, 30.0f, 0.0f);

        GameObject leftLeg = Instantiate(wall, leftLegPos, Quaternion.Euler(leftLegRotation)) as GameObject;

        return leftLeg;
    }

    private GameObject GetRightLeg(
        GameObject wall,
        Vector3 middle,
        Vector3 rotation)
    {
        Vector3 rightLegPos = new Vector3(wallWidth / 4.0f, 0.0f, (wallWidth * Mathf.Sqrt(3.0f)) / 12.0f);
        Vector3 rightLegRotation = new Vector3(0.0f, 150.0f, 0.0f);

        GameObject rightLeg = Instantiate(wall, rightLegPos, Quaternion.Euler(rightLegRotation)) as GameObject;

        return rightLeg;
    }

    private GameObject GetTriangleBase(
        GameObject wall,
        Vector3 middle,
        Vector3 rotation)
    {
        Vector3 basePos = new Vector3(0.0f, 0.0f, -(wallWidth * Mathf.Sqrt(3.0f) / 6.0f));
        Vector3 baseRotation = new Vector3(0.0f, 90.0f, 0.0f);

        GameObject triangleBase = Instantiate(wall, basePos, Quaternion.Euler(baseRotation)) as GameObject;

        return triangleBase;
    }

    private void HoldWalls()
    {
        for (int i = 0; i < Walls.Length; i++)
        {
            if (Walls[i] != null)
            {
                Walls[i].transform.parent = Holder.transform;
            }
        }
    }

    private void SetTriangleOnRightPosition()
    {
        Holder.transform.position = Middle;
        Holder.transform.Rotate(Rotation);
    }
}
