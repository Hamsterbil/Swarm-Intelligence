using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision {

    const int numPoints = 300;
    public static readonly Vector3[] directions = new Vector3[Collision.numPoints];
    static Collision () {
        float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        float angle = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numPoints; i++) {
            float t = (float) i / numPoints;
            float inclination = Mathf.Acos (1 - 2 * t);
            float Azimuth = angle * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (Azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (Azimuth);
            float z = Mathf.Cos (inclination);
            directions[i] = new Vector3 (x, y, z);
        }
    }
}