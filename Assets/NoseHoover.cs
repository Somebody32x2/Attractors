using System.Collections.Generic;
using UnityEngine;

public class NoseHoover : Attractor
{
    public float scale = 10f;

    public override Dictionary<string, float> DefaultVariables => new()
    {
        { "a", 1.5f }
    };

    public override ParticleSystem.Particle[] MoveParticles(ParticleSystem.Particle[] particles, int numParticlesAlive)
    {
        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Calculate the next position of the particle
            Vector3 pos = particles[i].position;
            float dx = pos.y/scale;
            float dy = -pos.x/scale + pos.y/scale * pos.z/scale;
            float dz = variables["a"] - pos.y/scale * pos.y/scale;
            pos.x += dx * dt * scale;
            pos.y += dy * dt * scale;
            pos.z += dz * dt * scale;
            particles[i].position = pos;
        }

        return particles;
    }
}