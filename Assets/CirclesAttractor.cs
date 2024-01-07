using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// LOTS of Attractors at http://www.3d-meier.de/tut19/Seite0.html
public abstract class CircleAttractor : MonoBehaviour
{
    private ParticleSystem ps;
    public Dictionary<string, float> variables;
    public abstract Dictionary<string, float> DefaultVariables { get; }
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        variables = DefaultVariables;
    }

    // Update is called once per frame
    // void Update() TODO: MOVE TO CirclesAttractorManager
    // {
    //     if (Time.timeScale == 0) return;
    //     // Update the position of each particle in the system
    //     ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
    //     int numParticlesAlive = ps.GetParticles(particles);
    //     particles = MoveParticles(particles, numParticlesAlive);
    //     // Apply the particle changes to the Particle System
    //     ps.SetParticles(particles, numParticlesAlive);
    // }

    public abstract ParticleSystem.Particle[] SetParticlePos(ParticleSystem.Particle[] particles, int numParticlesAlive,
        double t, float magnification, float[] iValues);

}