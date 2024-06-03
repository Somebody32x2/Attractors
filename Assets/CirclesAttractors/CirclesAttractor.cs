using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    

    public abstract ParticleSystem.Particle[] SetParticlePos(ParticleSystem.Particle[] particles, int numParticlesAlive,
        double t, float magnification, float[] iValues, bool useZ);

}