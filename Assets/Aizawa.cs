using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aizawa : Attractor
{
    public float scale = 10f;
    
    public override Dictionary<string, float> DefaultVariables =>
        new()
        {
            { "a", 0.95f },
            { "b", 0.7f },
            { "c", 0.6f },
            { "d", 3.5f },
            { "e", 0.25f },
            { "f", 0.1f },
        };

    public override ParticleSystem.Particle[] MoveParticles(ParticleSystem.Particle[] particles, int numParticlesAlive)
    {
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 pos = particles[i].position;
            float x = pos.x / scale;
            float y = pos.y / scale;
            float z = pos.z / scale;
            float dx = (z - variables["b"])* x - variables["d"] * y;
            float dy = variables["d"] * x + (z - variables["b"]) * y; // c+az-(z^3/3)-(x^2+y^2)(1+ez)+fz(x^3)
            float dz = variables["c"] + variables["a"] * z - (Mathf.Pow(z, 3)/3f)-(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) * (1 + variables["e"] * z) + variables["f"] * z * Mathf.Pow(x, 3);
            pos.x += dx * dt * scale;
            pos.y += dy * dt * scale;
            pos.z += dz * dt * scale;
            particles[i].position = pos;
        }

        return particles;
    }
}
