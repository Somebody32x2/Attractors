using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CirclesMode
{
    Flower
}
public class MainCirclesAttractor : CircleAttractor
{
    public float scale = 10f;
    public CirclesMode circlesMode = CirclesMode.Flower;
    
    public override Dictionary<string, float> DefaultVariables =>
        new();

    public override ParticleSystem.Particle[] SetParticlePos(ParticleSystem.Particle[] particles, int numParticlesAlive, double t, float magnification, float[] iValues, bool useZ)
    {
        for (int particleIndex = 0; particleIndex < particles.Length; particleIndex++)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            float i = iValues[particleIndex];
            switch (circlesMode)
            {
                case CirclesMode.Flower:
                    double d = 1000 + numParticlesAlive;
                    x = Math.Sin(i) * Math.Cos((t / d) * i);
                    y = Math.Cos(i) * Math.Cos((t / d) * i);
                    if (useZ) z = Math.Sin(i) * Math.Sin((t / d) * i);
                    else z = 0;
                    break;
            }

            x *= magnification;
            y *= magnification;
            z *= magnification;

            particles[particleIndex].position = new Vector3((float) x, (float) y, (float)z );
        }

        return particles;
    }

    // public override ParticleSystem.Particle[] SetParticlePos(ParticleSystem.Particle[] particles, int numParticlesAlive, )
    // {
        // for (int i = 0; i < numParticlesAlive; i++)
        // {
            // Vector3 pos = particles[i].position;
            // float x = pos.x / scale;
            // float y = pos.y / scale;
            // float z = pos.z / scale;
            // float dx = (z - variables["b"])* x - variables["d"] * y;
            // float dy = variables["d"] * x + (z - variables["b"]) * y; // c+az-(z^3/3)-(x^2+y^2)(1+ez)+fz(x^3)
            // float dz = variables["c"] + variables["a"] * z - (Mathf.Pow(z, 3)/3f)-(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) * (1 + variables["e"] * z) + variables["f"] * z * Mathf.Pow(x, 3);
            // pos.x += dx * dt * scale;
            // pos.y += dy * dt * scale;
            // pos.z += dz * dt * scale;
            // particles[i].position = pos;
        // }

        // return particles;
    // }
}
