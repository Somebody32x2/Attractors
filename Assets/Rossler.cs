using System.Collections.Generic;
using UnityEngine;

public class Rossler : Attractor
{
    // A simple Rossler attractor
    // https://en.wikipedia.org/wiki/R%C3%B6ssler_attractor
    private int dimensionalMax = 10000;
    public override Dictionary<string, float> DefaultVariables => new()
    {
        { "a", 0.2f },
        { "b", 0.2f },
        { "c", 14f }
    };

    public override ParticleSystem.Particle[] MoveParticles(ParticleSystem.Particle[] particles, int numParticlesAlive)
    {
        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Calculate the next position of the particle
            Vector3 pos = particles[i].position;
            float dx = -pos.y - pos.z;
            float dy = pos.x + variables["a"] * pos.y;
            float dz = variables["b"] + pos.z * (pos.x - variables["c"]);
            pos.x += dx * dt;
            pos.y += dy * dt;
            pos.z += dz * dt;
            particles[i].position = pos;
            
            // If the particle is outside the bounds (>max in any dim), reset it back to near the origin
            if (Mathf.Abs(pos.x) > dimensionalMax || Mathf.Abs(pos.y) > dimensionalMax || Mathf.Abs(pos.z) > dimensionalMax)
            {
                pos.x = Random.Range(-1f, 1f);
                pos.y = Random.Range(-1f, 1f);
                pos.z = Random.Range(-1f, 1f);
                particles[i].position = pos;
            }
            
        }

        return particles;
    }
}