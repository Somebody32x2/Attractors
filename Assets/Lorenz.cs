using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lorenz : Attractor
{
    // A simple Lorenz attractor
    // https://en.wikipedia.org/wiki/Lorenz_system

    public override Dictionary<string, float> DefaultVariables => new()
    {
        { "sig", 10 },
        { "rho", 28 },
        { "beta", 8 / 3 }
    };

    public override ParticleSystem.Particle[] MoveParticles(ParticleSystem.Particle[] particles, int numParticlesAlive)
    {
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 pos = particles[i].position;
            float dx = variables["sig"] * (pos.y - pos.x);
            float dy = pos.x * (variables["rho"] - pos.z) - pos.y;
            float dz = pos.x * pos.y - variables["beta"] * pos.z;
            pos.x += dx * dt;
            pos.y += dy * dt;
            pos.z += dz * dt;
            particles[i].position = pos;
        }

        return particles;
    }
}
