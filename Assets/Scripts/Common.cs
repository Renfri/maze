using UnityEngine;
using System.Collections;
using System;

public class Common : MonoBehaviour {

    public static float GetAxisCoordinate(
        float radius,
        int angle,
        Func<float, float> trigonometricFunc)
    {
        return radius * Mathf.Sqrt(3) * -trigonometricFunc(angle / 180.0f * Mathf.PI) / 3.0f;
    }
}
