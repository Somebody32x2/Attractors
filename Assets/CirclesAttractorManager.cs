using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CirclesAttractorManager : MonoBehaviour
{
    // Hash table of attractors
    public TMP_Dropdown attractorDropdown;
    private Dictionary<string, CircleAttractor> attractors = new Dictionary<string, CircleAttractor>();
    private Dictionary<int, String> attractorDropdownNameIndexes = new Dictionary<int, String>();
    private ParticleSystem ps;
    private CircleAttractor activeAttractor;
    public GameObject variableInputPrefab;
    public GameObject UI;
    public GameObject dtReferenceVar;
    
    public float speed = 1;
    public double t = 0;
    public float magnification = 1;
    public bool drawLines = true;
    public LineRenderer lineRenderer;
    public bool isRendering = true;
    public float renderIncrement = 0.1f;
    public float renderOffset = 0;
    public float renderSpeed = 1;
    
    public float userTrailLength = 0.0001f;
    public bool useZ = true;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<LineRenderer>().enabled = drawLines;
        if (drawLines) lineRenderer = gameObject.GetComponent<LineRenderer>();
        
        // Add the attractors to the hash table (they are on this object, but all but one are disabled)
        foreach (CircleAttractor attractor in GetComponentsInChildren<CircleAttractor>())
        {
            Debug.Log("Adding attractor: " + attractor.GetType().ToString());
            attractors.Add(attractor.GetType().ToString(), attractor);
            // Add the attractor name (using the class nane) to the dropdown and store the index
            attractorDropdownNameIndexes.Add(attractorDropdown.options.Count, attractor.GetType().ToString());
            attractorDropdown.options.Add(new TMP_Dropdown.OptionData(attractor.GetType().ToString()));
        }
        
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(SetColors());
        
        // Switch to the first attractor to get the UI set up
        SwitchAttractor(0);


    }

    private void Update()
    {
        var oldT = t;
        t += Time.deltaTime * speed * Time.timeScale;
        // Debug.Log("New T: " + t + ", Old: " + oldT + " diff: " + (t - oldT) + " tried to add " + (Time.deltaTime * speed * Time.timeScale));
        // Update the position of each particle in the system
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticlesAlive = ps.GetParticles(particles);
        // Assemble iValues
        float[] iValues = new float[numParticlesAlive];
        float iValue = 0;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            iValue += 1 + (isRendering ? renderOffset : 0);
            iValues[i] = iValue;
        }
        particles = activeAttractor.SetParticlePos(particles, numParticlesAlive, t, magnification, iValues, useZ);
        // Apply the particle changes to the Particle System
        ps.SetParticles(particles, numParticlesAlive);
        if (isRendering)
        {
            renderOffset += renderIncrement * renderSpeed;
            if (renderOffset > 1)
            {
                renderOffset = 0;
                renderIncrement /= 5;
            }
        }
        // On Press of i/k increase/decrease speed by one
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Mathf.Abs(speed) < 1 && !Input.GetKey(KeyCode.LeftShift)) speed += 0.1f;
            else speed++;
            SetSpeed(speed.ToString(), true);
            if (isRendering && Mathf.Abs(renderSpeed) < 1 && !Input.GetKey(KeyCode.LeftShift)) renderSpeed += 0.1f;
            else if (isRendering) renderSpeed++;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Debug.Log((speed--).ToString() + " " + float.Parse((speed--).ToString()));
            if ((speed is <= 1 and >= -1) && !Input.GetKey(KeyCode.LeftShift)) speed -= 0.1f;
            else speed--;
            SetSpeed(speed.ToString(), true);
            if (isRendering && (renderSpeed is <= 1 and >= -1) && !Input.GetKey(KeyCode.LeftShift)) renderSpeed -= 0.1f;
            else if (isRendering) renderSpeed--;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SetSpeed("0", true);
            if (isRendering) renderSpeed = 0;
        }
        if (drawLines)
        {
            RedrawLines();
        }
    }

    public void ResetPS()
    {
        // Reset the particle system
        // t = 0;
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
        lineRenderer.SetPositions(particles.Select(particle => particle.position).ToArray());
    }

    public void ResetTime()
    {
        t = 0;
        ResetPS();
    }
    
    public void SwitchAttractor(int attractorIndex)
    {
        // Disable all attractors
        foreach (CircleAttractor att in attractors.Values)
        {
            att.enabled = false;
        }
        
        // Store the attractor
        CircleAttractor attractor = attractors[attractorDropdownNameIndexes[attractorIndex]];
        // Enable the attractor with the given name
        attractor.enabled = true;
        attractor.variables = attractor.DefaultVariables;
        
        // Add the variables and inputs to the UI and set the values and callbacks/listeners
        foreach (Transform child in UI.transform)
        {
            // Destroy all the children of the UI except the reference variable (dt)
            if (child.gameObject != dtReferenceVar)
            {
                Destroy(child.gameObject);
            }
        }
        
        Debug.Log(attractor.variables);
        int i = 0;
        foreach (KeyValuePair<string, float> variable in attractor.variables)
        {
            GameObject variableInput = Instantiate(variableInputPrefab, UI.transform);
            // Move the variable input to the correct position (based on the number of variables, +120x for each col and -30y for each row and start at just below the dtrefvar, go from top to bottom then add another column)
            variableInput.transform.position = dtReferenceVar.transform.position + new Vector3(180 * (i / 3), -40 * (i % 3) - 50, 0);
            variableInput.transform.Find("VariableName").GetComponent<TMP_Text>().text = variable.Key;
            TMP_InputField inputField = variableInput.transform.Find("VariableValue").GetComponent<TMP_InputField>();
            inputField.text = variable.Value.ToString();
            inputField.onEndEdit.AddListener(delegate { SetAttractorVariable(variable.Key, inputField.text); });
            i++;
        }


        activeAttractor = attractor;
        // Reset the particle system
        ResetPS();
    }
    public void SetNumParticles(string numParticles)
    {
        // Set the number of particles in the particle system by changing the maxParticles and burst amount
        var main = ps.main;
        main.maxParticles = Int32.Parse(numParticles);
        var burst = ps.emission.GetBurst(0);
        burst.count = Int32.Parse(numParticles);
        ps.emission.SetBurst(0, burst);
        ResetPS();
    }
    public void SetTrailLength(string trailLength)
    {
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
    private void SetAttractorVariable(string variableName, string variableValue)
    {
        // Set the value of the given variable in the attractor
        CircleAttractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
        attractor.variables[variableName] = float.Parse(variableValue);
        // ResetPS();
    }
    public void SetSpeed(string speedStr)
    {
        // Set the speed
        speed = float.Parse(speedStr);
        if (speed == 0) SetTrailLength("0", false);
        if (speed < 1) SetTrailLength("0", false);
        SetTrailLength(Mathf.Abs(userTrailLength/speed).ToString(), false);
        // ResetPS();
    }
    public void SetSpeed(string speedStr, bool setUI=false)
    {
        // Set the speed
        speed = float.Parse(speedStr);
        if (speed == 0) SetTrailLength("0", false);
        if (speed < 1) SetTrailLength("0", false);
        SetTrailLength(Mathf.Abs(ps.trails.lifetime.constant/speed).ToString(), false);
        // ResetPS();
        if (setUI)
        {
            dtReferenceVar.GetComponentInChildren<TMP_InputField>().text = speedStr;
        }
    }

    public void ResetToDefaults()
    {
        // Set the current attractor to its default values
        CircleAttractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
        attractor.variables = attractor.DefaultVariables;
        // Set the speed to the default value
        speed = 1;
        // Reset the UI
        SwitchAttractor(attractorDropdown.value);
    }

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
