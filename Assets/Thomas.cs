using System.Collections.Generic;
using UnityEngine;

public class Thomas : Attractor
{
    public override Dictionary<string, float> DefaultVariables => new()
    {
        { "b", 0.19f }
    };

    public override ParticleSystem.Particle[] MoveParticles(ParticleSystem.Particle[] particles, int numParticlesAlive)
    {
        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Calculate the next position of the particle
            Vector3 pos = particles[i].position;
            float dx = -variables["b"] * pos.x + Mathf.Sin(pos.y);
            float dy = -variables["b"] * pos.y + Mathf.Sin(pos.z);
            float dz = -variables["b"] * pos.z + Mathf.Sin(pos.x);
            pos.x += dx * dt;
            pos.y += dy * dt;
            pos.z += dz * dt;
            particles[i].position = pos;
        }

        return particles;
    }
}