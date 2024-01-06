using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum Axises
// {
//     X,
//     Y,
//     Z,
// }
public class Wavelengths : MonoBehaviour
{
    /// <summary>
    ///  Controlls the Particle System to make a cube of RGB particles
    ///  Use resoultion to control the number of particles in each dimension
    ///  Cover the range of 0 to 1 in each dimension
    ///  Arrange the particles in a cube
    ///  Set the color of each particle to match its position
    ///  Use the particle system to render the particles
    /// </summary>
    public float sideLength = 5f; // The total length of each side of the cube (so 5 particles per unit with resolution = 25, sideLength = 5)

    [Range(0.01f, 1.0f)]
    public float size = 0.1f;
    
    // public bool refresh = false;
    
    

    public int numParticles = 300;

    
    // Hold the last state of the variables
    private float lastSideLength;
    private float lastSize;
    private int lastNumParticles;


    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // Configure the particle system (# of particles, etc.)
        var main = ps.main;
        // main.maxParticles = (int) Mathf.Pow(resolution, 3);
        // Debug.Log("nump: " + numParticles);
        main.maxParticles = numParticles;
        
        main.startSpeed = 0f;
        main.startLifetime = Mathf.Infinity;
        main.startSize = size;
        main.startColor = Color.white;
        // Set the burst to spawn all the particles at once
        var burst = ps.emission.GetBurst(0);
        burst.count = numParticles;
        ps.emission.SetBurst(0, burst);
        
        ps.Clear();
        ps.Play();

        // Create the particles
        StartCoroutine(initParticles());
        // ParticleSystem.Particle[] particles = new ParticleSystem.Particle[main.maxParticles];
        // int numParticlesAlive = ps.GetParticles(particles);

    }

    IEnumerator initParticles()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(0.01f);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticlesAlive = ps.GetParticles(particles);

        int particleIndex = 0;

        for (float i = 0; i < 300; i += 300/numParticles)
        {
            if (particleIndex >= numParticlesAlive) break;
            particles[particleIndex].startColor = new Color(
                LongRedWavelengthFromLinearPos(400 + i),
                MedGreenWavelengthFromLinearPos(400 + i), 
                ShortBlueWavelengthFromLinearPos(400 + i) 
                );
            particles[particleIndex].position = new Vector3(
                LongRedWavelengthFromLinearPos(400 + i) * sideLength,
                MedGreenWavelengthFromLinearPos(400 + i) * sideLength,
                ShortBlueWavelengthFromLinearPos(400 + i) * sideLength
            );
            particleIndex++;
        }
        
        
        ps.SetParticles(particles, numParticlesAlive);
        // Log the number of particles
        // Debug.Log("Number of particles: " + particles.Length);
        // Log the pos and color of the first three particles
        // Debug.Log("First three particles: " + particles[0].position + " " + particles[0].startColor + " " + particles[1].position + " " + particles[1].startColor + " " + particles[2].position + " " + particles[2].startColor);
    }
    
    // Functions L(t), M(t), S(t) define wavelength
    // Convert RGB to wave lengths
    // Color RGBToWaveLength()
    private const float e = 2.7182818284590452353602874713527f;
    float LongRedWavelengthFromLinearPos(float t) { return Mathf.Pow(e, -0.5f * Mathf.Pow((t - 570f) / 50f, 2)); }
    float MedGreenWavelengthFromLinearPos(float t) { return Mathf.Pow(e, -0.5f * Mathf.Pow((t - 540f) / 40f, 2)); }
    float ShortBlueWavelengthFromLinearPos(float t) { return Mathf.Pow(e, -0.5f * Mathf.Pow((t - 450f) / 20f, 2)); }

    // Update is called once per frame
    void Update()
    {
        // Check if any of the variables have changed
        if (sideLength != lastSideLength || size != lastSize || numParticles != lastNumParticles)
        {
            // If so, reinitialize the particles
            Start();
            // Set the variables to the last state
            lastSideLength = sideLength;
            lastSize = size;
            lastNumParticles = numParticles;

        }
    }
}
