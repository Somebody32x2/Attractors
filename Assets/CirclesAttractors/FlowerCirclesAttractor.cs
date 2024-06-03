using System;
using System.Collections;
using UnityEngine;

public class FlowerCirclesAttractor : ConventionalCirclesAttractor
{
    public override Vector3 CalculateParticle(float i, double t , int a, bool useZ)
    {
        double d = 1000 + a;
        return new Vector3(
            (float) (Math.Sin(i) * Math.Cos((t / d) * i)),
            (float) (Math.Cos(i) * Math.Cos((t / d) * i)),
            useZ ? (float) (Math.Sin(i) * Math.Sin((t / d) * i)) : 0
            );
    }
}