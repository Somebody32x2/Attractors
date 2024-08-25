using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DoublePendulumsManager : MonoBehaviour
{

    private ParticleSystem ps;
    public GameObject UI;
    public GameObject dtReferenceVar;

    public float mass1 = 50;
    public float mass2 = 50;
    public float length1 = 1;
    public float length2 = 1;
    public float spread1 = 1f;
    public float spread2 = 0f;
    public float gravity = 1f;
    private List<DoublePendulum> pendulums = new List<DoublePendulum>();
    public int numPendulums = 200;
    public float speed = 1; // Speed of the simulation -- Directly multiplies velocity calculations
    public float updatesPerTick = 1; // Provides a way to modify speed without changing velocity calculations (except if multByDt)
    public float magnification = 100;
    public bool showPoint1 = true;
    public bool showPoint2 = true;
    public bool drawLines = true;
    public bool multByDt = true; // Multiply the speed by the delta time, or use 1/60
    public LineRenderer lineRenderer;
    
    public float userTrailLength = 0.0001f;
    // public bool useZ = false;
    
    private bool lastDrawLines = true;
    private int lastNumPendulums = 200;
    private float lastUserTrailLength = 0.0001f;
    private bool lastShowPoint1 = true;
    private bool lastShowPoint2 = true;
    // Start is called before the first frame update
    void Start()
    {
        lastShowPoint1 = showPoint1;
        lastShowPoint2 = showPoint2;
        gameObject.GetComponent<LineRenderer>().enabled = drawLines;
        if (drawLines) lineRenderer = gameObject.GetComponent<LineRenderer>();
        
        ps = GetComponent<ParticleSystem>();
        SetNumPendulums(numPendulums.ToString());
        StartCoroutine(SetColors());
        ResetPS();
        // ResetPendulums();   
        
    }

    public float updatesRemaining = 0;
    private void Update()
    {
        if (lastNumPendulums != numPendulums)
        {
            SetNumPendulums(numPendulums+"");
        }
        if (lastUserTrailLength != userTrailLength)
        {
            SetTrailLength(userTrailLength+"");
        }
        if (lastShowPoint1 != showPoint1 || lastShowPoint2 != showPoint2)
        {
            ResetPS(false);
            lastShowPoint1 = showPoint1;
            lastShowPoint2 = showPoint2;
        }

        updatesRemaining += updatesPerTick;
        float dt = multByDt ? Time.deltaTime : 1 / 60f;
        while (updatesRemaining >= 1)
        {
            updatesRemaining--;

            // Update the position of each particle in the system
            ParticleSystem.Particle[] particles =
                new ParticleSystem.Particle[(numPendulums) * ((showPoint1 ? 1 : 0) + (showPoint2 ? 1 : 0))];
            int numParticlesAlive = ps.GetParticles(particles);

            for (int i = 0; i < pendulums.Count; i++)
            {
                // if (i * (showPoint2 ? 2 : 1) >= particles.Length - 1) 
                // Debug.Log($"len {pendulums.Count} i {i} showPoint1 {showPoint1} showPoint2 {showPoint2} particles.Length {particles.Length} i * (showPoint2 ? 2 : 1) {i * (showPoint2 ? 2 : 1)}");
                Vector3[] positions = pendulums[i].Update(gravity, speed, dt);
                if (showPoint1)
                {
                    if (updatesRemaining <= 1) particles[i * (showPoint2 ? 2 : 1)].position = positions[0] * magnification;
                }

                if (showPoint2)
                {
                    if (updatesRemaining <= 1) particles[i * (showPoint1 ? 2 : 1) + (showPoint1 ? 1 : 0)].position = positions[1] * magnification;
                }
            }
            if (updatesRemaining <= 1) ps.SetParticles(particles, numParticlesAlive);
        }

        // Apply the particle changes to the Particle System
        
        
        // On Press of i/k increase/decrease speed by one
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (speed < 1) speed += 0.1f;
            else speed++;
            SetSpeed(speed.ToString(), true);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (speed <= 1) speed -= 0.1f;
            else speed--;
            SetSpeed(speed.ToString(), true);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SetSpeed("0", true);
        }
        if (drawLines)
        {
            RedrawLines();
        }
        if (lastDrawLines != drawLines)
        {
            gameObject.GetComponent<LineRenderer>().enabled = drawLines;
            lastDrawLines = drawLines;
        }
    }

    private void ResetPendulums()
    {
        pendulums = new List<DoublePendulum>();
        for (int i = 0; i < numPendulums; i++)
        {
            pendulums.Add(new DoublePendulum(length1, length2, mass1, mass2, -2f + spread1 / numPendulums * i, -2f + spread2 / numPendulums * i));
        }
    }

    public void ResetPS(bool resetPends=true)
    {
        if (resetPends) ResetPendulums();
        // Reset the particle system
        ps.Clear();
        ps.Play();
        StartCoroutine(SetColors());
    }

    private void RedrawLines()
    {
        var main = ps.main;
        lineRenderer.positionCount = main.maxParticles;
        var particles = new ParticleSystem.Particle[main.maxParticles];
        ps.GetParticles(particles);
        var positions = particles.Select(particle => particle.position).ToArray();
        // Insert (0, 0, 0) so that it always goes after a point2 if point1 is enabled
        
        if (showPoint1 && showPoint2)
        {
            List<Vector3> npos = new List<Vector3>();
            npos.Capacity = positions.Length * 4;
            for (int i = 0; i < positions.Length; i += 2)
            {
                npos.Add(Vector3.zero);
                npos.Add(positions[i]);
                npos.Add(positions[i + 1]);
                npos.Add(positions[i]);
                
            }
            positions = npos.ToArray();
        }
        else
        {
            List<Vector3> npos = new List<Vector3>();
            npos.Capacity = positions.Length * 2;
            for (int i = 0; i < positions.Length; i++)
            {
                npos.Add(Vector3.zero);
                npos.Add(positions[i]);
            }
            positions = npos.ToArray();
        }
        // Debug.Log(positions.Length + " " + lineRenderer.positionCount + " " + positions[0] + " " + positions[^1] + " " + particles.Length);
        lineRenderer.SetPositions(positions);
    }

    public void ResetTime()
    {
        ResetPS();
    }
    public void SetNumPendulums(string inumPendulums)
    {
        numPendulums = int.Parse(inumPendulums);
        lastNumPendulums = numPendulums;
        // Set the number of pendulums
        var main = ps.main;
        main.maxParticles = (Int32.Parse(inumPendulums) + 1) * ((showPoint1 ? 1 : 0) + (showPoint2 ? 1 : 0));
        Debug.Log($"max particles {(Int32.Parse(inumPendulums) + 1) * ((showPoint1 ? 1 : 0) + (showPoint2 ? 1 : 0))}");
        var burst = ps.emission.GetBurst(0);
        burst.count = (Int32.Parse(inumPendulums) + 1) * ((showPoint1 ? 1 : 0) + (showPoint2 ? 1 : 0));
        ps.emission.SetBurst(0, burst);
        ResetPS();
    }
    public void SetTrailLength(string trailLength)
    {
        lastUserTrailLength = userTrailLength;
        // Set the trail length of the particle system (THIS GETS CALLED BY THE UI COMPONENT)
        var trail = ps.trails;
        userTrailLength = float.Parse(trailLength);
        trail.lifetime = userTrailLength;
        ResetPS();
        ResetPS();
    }
    public void SetTrailLength(string trailLength, bool resetPS)
    {
        // Set the trail length of the particle system
        var trail = ps.trails;
        trail.lifetime = float.Parse(trailLength);
        if (resetPS) ResetPS();
        if (resetPS) ResetPS();
    }
    public void SetSpeed(string speedStr)
    {
        // Set the speed
        speed = float.Parse(speedStr);
        // if (speed == 0) SetTrailLength("0", false);
        // else SetTrailLength(userTrailLength + "", false);
        // SetTrailLength(Mathf.Abs(userTrailLength/speed).ToString(), false);
        // ResetPS();
    }
    public void SetSpeed(string speedStr, bool setUI=false)
    {
        // Set the speed
        speed = float.Parse(speedStr);
        if (speed == 0) SetTrailLength("0", false);
        else SetTrailLength(userTrailLength + "", false);
        // SetTrailLength(Mathf.Abs(ps.trails.lifetime.constant/speed).ToString(), false);
        // ResetPS();
        if (setUI)
        {
            dtReferenceVar.GetComponentInChildren<TMP_InputField>().text = speedStr;
        }
    }

    // public void ResetToDefaults()
    // {
    //     // Set the current attractor to its default values
    //     CircleAttractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
    //     attractor.variables = attractor.DefaultVariables;
    //     // Set the speed to the default value
    //     speed = 1;
    //     // Reset the UI
    //     SwitchAttractor(attractorDropdown.value);
    // }

    IEnumerator SetColors()
    {
        yield return new WaitForSeconds(0.05f);
        // Give each particle its own rainbow color by looping through each particle and setting its color
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticlesAlive = ps.GetParticles(particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float t = (float)i / numParticlesAlive;
            particles[i].startColor = Color.HSVToRGB(t, 1, 1);
        }
        ps.SetParticles(particles, numParticlesAlive);
    }
}
