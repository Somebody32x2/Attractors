using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorSpaceType
{
    RGB,
    HSV,
    // WaveLengths,
}
public enum Axises
{
    X,
    Y,
    Z,
}
public class RGBCube : MonoBehaviour
{
    /// <summary>
    ///  Controlls the Particle System to make a cube of RGB particles
    ///  Use resoultion to control the number of particles in each dimension
    ///  Cover the range of 0 to 1 in each dimension
    ///  Arrange the particles in a cube
    ///  Set the color of each particle to match its position
    ///  Use the particle system to render the particles
    /// </summary>
    public float resolution = 25;
    public float sideLength = 5f; // The total length of each side of the cube (so 5 particles per unit with resolution = 25, sideLength = 5)
    public ColorSpaceType colorSpaceType = ColorSpaceType.RGB;
    
    [Range(0.01f, 1.0f)]
    public float size = 0.1f;
    
    // public bool refresh = false;
    
    public bool setAnAxis = false;
    public Axises axis = Axises.X;
    [Range(0.0f, 1.0f)]
    public float axisValue = 0.5f;
    
    public bool useControlPoints = false;
    public GameObject[] controlPoints = new GameObject[4];
    
    public bool useSphere = false;
    public float sphereRadius = 1.0f;
    public Transform sphereCenter;

    
    // Hold the last state of the variables
    private float lastResolution;
    private float lastSideLength;
    private ColorSpaceType lastColorSpaceType;
    private float lastSize;
    private bool lastSetAnAxis;
    private Axises lastAxis;
    private float lastAxisValue;
    private bool lastUseControlPoints;
    private Vector3[] lastControlPoints = new Vector3[4];
    private bool lastUseSphere;
    private float lastSphereRadius;
    private Vector3 lastSphereCenter;


    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // Configure the particle system (# of particles, etc.)
        var main = ps.main;
        // main.maxParticles = (int) Mathf.Pow(resolution, 3);
        int numParticles = (int) (setAnAxis ?  Mathf.Pow(resolution, 2) : Mathf.Pow(resolution, 3));
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
        
        // Loop over the particles in each dimension
        // for (int dm1 = 0; dm1 < resolution; dm1++) {
        //     for (int dm2 = 0; dm2 < resolution; dm2++)  {
        //         for (int dm3 = 0; dm3 < resolution; dm3++) {
        //             // Set the position of the particle
        //             particles[particleIndex].position = new Vector3(dm1 / resolution, dm2 / resolution, dm3 / resolution) * sideLength; 
        //             // Set the color of the particle
        //             switch (colorSpaceType)
        //             {
        //                 case ColorSpaceType.RGB:
        //                     particles[particleIndex].startColor = new Color(dm1 / resolution, dm2 / resolution, dm3 / resolution);
        //                     break;
        //                 case ColorSpaceType.HSV:
        //                     particles[particleIndex].startColor = Color.HSVToRGB(dm1 / resolution, dm2 / resolution, dm3 / resolution);
        //                     break;
        //             }
        //             particleIndex++;
        //         }
        //     }
        // }
        // Loop over the particles in each dimension, using the axis value for the set axis
        if (!useControlPoints)
        {
            for (int dm1 = (axis == Axises.X && setAnAxis ? (int)(axisValue * resolution) : 0); 
                 dm1 < (axis == Axises.X && setAnAxis ? (int)(axisValue * resolution) + 1 : resolution);
                 dm1++) {
                for (int dm2 = (axis == Axises.Y && setAnAxis ? (int)(axisValue * resolution) : 0);
                     dm2 < (axis == Axises.Y && setAnAxis ? (int)(axisValue * resolution) + 1 : resolution);
                     dm2++) {
                    for (int dm3 = (axis == Axises.Z && setAnAxis ? (int)(axisValue * resolution) : 0);
                         dm3 < (axis == Axises.Z && setAnAxis ? (int)(axisValue * resolution) + 1 : resolution);
                         dm3++) {
                        // If using a sphere, check if the particle is within the sphere
                        if (useSphere) {
                            if (Vector3.Distance(new Vector3(dm1 / resolution, dm3 / resolution, dm2 / resolution) * sideLength, new Vector3(sphereCenter.position.x, sphereCenter.position.y, -sphereCenter.position.z)) > sphereRadius) {
                                continue;
                            }
                        }
                        // Set the position of the particle
                        particles[particleIndex].position =
                            new Vector3(dm1 / resolution, dm2 / resolution, dm3 / resolution) * sideLength;
                        // Set the color of the particle
                        switch (colorSpaceType)
                        {
                            case ColorSpaceType.RGB:
                                particles[particleIndex].startColor = new Color(dm1 / resolution, dm2 / resolution,
                                    dm3 / resolution);
                                break;
                            case ColorSpaceType.HSV:
                                particles[particleIndex].startColor = Color.HSVToRGB(dm1 / resolution, dm2 / resolution,
                                    dm3 / resolution);
                                break;
                            // case ColorSpaceType.WaveLengths:
                                // particles[particleIndex].startColor = RGBToWaveLength(dm1 / resolution, dm2 / resolution,
                                    // dm3 / resolution);
                                // break;
                        }

                        particleIndex++;
                    }
                }
            }
        }
        // else
        // {
        //     // Loop over the particles between the control points, so that only the cube's particles inside the plane of control points are shown
        //     // Get the control points
        //     Vector3[] controlPoints = new Vector3[4];
        //     for (int i = 0; i < 4; i++)
        //     {
        //         controlPoints[i] = this.controlPoints[i].transform.position;
        //     }
        //
        //     for (int dm1 = 0; dm1 < resolution; dm1++)
        //     {
        //         for (int dm2 = 0; dm2 < resolution; dm2++)
        //         {
        //             for (int dm3 = 0; dm3 < resolution; dm3++)
        //             {
        //                 // Determine if the particle is inside the plane of control points
        //                 // Get the position of the particle
        //                 Vector3 particlePos = new Vector3(dm1 / resolution, dm2 / resolution, dm3 / resolution) * sideLength;
        //                 // Get the plane of the control points
        //                 Plane plane = new Plane(controlPoints[0], controlPoints[1], controlPoints[2]);
        //                 // Determine if the particle is inside the plane
        //                 if (plane.GetSide(particlePos))
        //                 {
        //                     // Set the position of the particle
        //                     particles[particleIndex].position = particlePos;
        //                     // Set the color of the particle
        //                     switch (colorSpaceType)
        //                     {
        //                         case ColorSpaceType.RGB:
        //                             particles[particleIndex].startColor = new Color(dm1 / resolution, dm2 / resolution,
        //                                 dm3 / resolution);
        //                             break;
        //                         case ColorSpaceType.HSV:
        //                             particles[particleIndex].startColor = Color.HSVToRGB(dm1 / resolution,
        //                                 dm2 / resolution,
        //                                 dm3 / resolution);
        //                             break;
        //                     }
        //
        //                     particleIndex++;
        //                 }
        //             }
        //         }
        //     }
        //
        //     
        // }
        // Apply the particle changes to the Particle System
        ps.SetParticles(particles, numParticlesAlive);
        // Log the number of particles
        // Debug.Log("Number of particles: " + particles.Length);
        // Log the pos and color of the first three particles
        // Debug.Log("First three particles: " + particles[0].position + " " + particles[0].startColor + " " + particles[1].position + " " + particles[1].startColor + " " + particles[2].position + " " + particles[2].startColor);
    }
    
    // Functions L(t), M(t), S(t) define wavelength
    // Convert RGB to wave lengths
    // Color RGBToWaveLength()

    // Update is called once per frame
    void Update()
    {
        bool controlPointsChanged = false;
        // Check if the control points have changed
        for (int i = 0; i < 4; i++)
        {
            if (!controlPoints[i].transform.position.Equals(lastControlPoints[i]))
            {
                // Print the control point that changed and the difference
                // Debug.Log("Control point " + i + " changed by " + (controlPoints[i].transform.position - lastControlPoints[i]));
                controlPointsChanged = true;
                lastControlPoints[i] = controlPoints[i].transform.position;
            }
        }
        // Check if any of the variables have changed
        if (resolution != lastResolution || sideLength != lastSideLength || colorSpaceType != lastColorSpaceType || size != lastSize ||
            setAnAxis != lastSetAnAxis || axis != lastAxis || axisValue != lastAxisValue || controlPointsChanged ||
            useControlPoints != lastUseControlPoints || useSphere != lastUseSphere || sphereRadius != lastSphereRadius || sphereCenter.position != lastSphereCenter)
        {
            // If so, reinitialize the particles
            Start();
            // Set the variables to the last state
            lastResolution = resolution;
            lastSideLength = sideLength;
            lastColorSpaceType = colorSpaceType;
            lastSize = size;
            lastSetAnAxis = setAnAxis;
            lastAxis = axis;
            lastAxisValue = axisValue;
            lastUseControlPoints = useControlPoints;
            lastUseSphere = useSphere;
            lastSphereRadius = sphereRadius;
            lastSphereCenter = sphereCenter.position;

        }
    }
}
