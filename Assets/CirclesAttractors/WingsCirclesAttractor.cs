using System;
using System.Collections;
using UnityEngine;

public class WingsCirclesAttractor : ConventionalCirclesAttractor
{
    public override Vector3 CalculateParticle(float i, double t , int a, bool useZ)
    {
        return new Vector3(
            (float) (
                Math.Abs(Math.Sin(i)) * Math.Cos(t * i / a)
                ),
            (float) (
                Math.Cos(i) * Math.Cos(t * i / a)
                ),
            useZ ? (float) (Math.Abs(Math.Sin(i)) * Math.Sin(t * i / a)) : 0
            );
    }
}