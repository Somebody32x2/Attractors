using System.Collections.Generic;
using UnityEngine;

public abstract class ConventionalCirclesAttractor : CircleAttractor
{
    public override Dictionary<string, float> DefaultVariables =>
        new();

    public abstract Vector3 CalculateParticle(float i, double t, int a, bool useZ);

    public override ParticleSystem.Particle[] SetParticlePos(ParticleSystem.Particle[] particles, int numParticlesAlive,
        double t, float magnification, float[] iValues, bool useZ)
    {
        for (int particleIndex = 0; particleIndex < particles.Length; particleIndex++)
        {
            float i = iValues[particleIndex];

            particles[particleIndex].position = CalculateParticle(i, t, numParticlesAlive , useZ) * magnification;
        }

        return particles;
    }
}