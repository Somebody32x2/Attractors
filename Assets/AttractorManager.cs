using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AttractorManager : MonoBehaviour
{
    // Hash table of attractors
    public TMP_Dropdown attractorDropdown;
    private Dictionary<string, Attractor> attractors = new Dictionary<string, Attractor>();
    private Dictionary<int, String> attractorDropdownNameIndexes = new Dictionary<int, String>();
    private ParticleSystem ps;
    public GameObject variableInputPrefab;
    public GameObject UI;
    public GameObject dtReferenceVar;
    
    // Start is called before the first frame update
    void Start()
    {
        // Add the attractors to the hash table (they are on this object, but all but one are disabled)
        foreach (Attractor attractor in GetComponentsInChildren<Attractor>())
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
    
    public void ResetPS()
    {
        // Reset the particle system
        ps.Clear();
        ps.Play();
        StartCoroutine(SetColors());
    }
    
    public void SwitchAttractor(int attractorIndex)
    {
        // Disable all attractors
        foreach (Attractor att in attractors.Values)
        {
            att.enabled = false;
        }
        
        // Store the attractor
        Attractor attractor = attractors[attractorDropdownNameIndexes[attractorIndex]];
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
        // Set the trail length of the particle system
        var trail = ps.trails;
        trail.lifetime = float.Parse(trailLength);
        ResetPS();
    }
    private void SetAttractorVariable(string variableName, string variableValue)
    {
        // Set the value of the given variable in the attractor
        Attractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
        attractor.variables[variableName] = float.Parse(variableValue);
        // ResetPS();
    }
    public void SetDt(string dt)
    {
        // Set the dt of the attractor
        Attractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
        attractor.dt = float.Parse(dt);
        // ResetPS();
    }

    public void ResetToDefaults()
    {
        // Set the current attractor to its default values
        Attractor attractor = attractors[attractorDropdownNameIndexes[attractorDropdown.value]];
        attractor.variables = attractor.DefaultVariables;
        // Set the dt to the default value
        attractor.dt = 0.01f;
        
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
