using System;
using UnityEngine;

public class DoublePendulum
{
    private double r1;
    private double r2;
    private double m1;
    private double m2;
    private double a1;
    private double a2;
    private double a1_v;
    private double a2_v;
    public bool dead;

    public DoublePendulum(float length1, float length2, float mass1, float mass2, float angle1, float angle2)
    {
        this.r1 = length1;
        this.r2 = length2;
        this.m1 = mass1;
        this.m2 = mass2;
        this.a1 = Mathf.PI / angle1;
        this.a2 = Mathf.PI / angle2;
        this.a2_v = 0d;
        this.a1_v = 0d;
    }

    public Vector3[] Update(float g, float speed, float dt)
    {
        // Log each step and its ingredients to show where NaNs are coming from
        double num1 = -g * (2.0d * m1 + m2) * (a1);
        // Debug.Log($"num1: {num1} m1: {m1} m2: {m2} a1: {a1} gravity: {g}");
        double num2 = -m2 * g * Math.Sin(a1 - 2.0d * a2);
        // Debug.Log($"num2: {num2} a1: {a1} a2: {a2}");
        double num3 = -2.0d * Math.Sin(a1 - a2) * m2;
        // Debug.Log($"num3: {num3} m2: {m2} a1: {a1} a2: {a2}");
        double num4 = a2_v * a2_v * r2 + a1_v * a1_v * r1 * (a1 - a2);
        // Debug.Log($"num4: {num4} a2_v: {a2_v} r2: {r2} a1_v: {a1_v} r1: {r1} a1: {a1} a2: {a2}");
        double den = r1 * (2.0d * m1 + m2 - m2 * (2.0d * a1 - 2.0d * a2));
        // Debug.Log($"den: {den} r1: {r1} m1: {m1} m2: {m2} a1: {a1} a2: {a2}");
        double a1_a = (num1 + num2 + num3 * num4) / den;
        
        // Debug.Log($"a1_v: {a1_v} a2_v {a2_v} a1_a: {a1_a} \n num1: {num1} num2: {num2} num3: {num3} num4: {num4} den: {den}");

        num1 = 2.0d * Math.Sin(a1 - a2);
        num2 = a1_v * a1_v * r1 * (m1 + m2);
        num3 = g * (m1 + m2) * Math.Cos(a1);
        num4 = a2_v * a2_v * r2 * m2 * Math.Cos(a1 - a2);
        den = r2 * (2.0d * m1 + m2 - m2 * Math.Cos(2.0d * a1 - 2.0d * a2));
        double a2_a = (num1 * (num2 + num3 + num4)) / den;

        // Debug.Log($"a1_v: {a1_v} a2_v {a2_v} a1_a: {a1_a}, a2_a: {a2_a}");
        
        double x1 = r1 * Math.Sin(a1);
        double y1 = r1 * Math.Cos(a1);
        
        double x2 = x1 + r2 * Math.Sin(a2);
        double y2 = y1 + r2 * Math.Cos(a2);
        
        
        // Debug.Log($"a1_v {a1_v} a2_v {a2_v} a1 {a1} a2 {a2} a1_a {a1_a} a2_a {a2_a} speed {speed} dt {dt}");
        this.a1_v += a1_a * speed * dt;
        this.a2_v += a2_a * speed * dt;
        this.a1 += a1_v * speed * dt;
        this.a2 += a2_v * speed * dt;
        // Debug.Log($"a1_v {a1_v} a2_v {a2_v} a1 {a1} a2 {a2} a1_a {a1_a} a2_a {a2_a} speed {speed} dt {dt}");
        
        // Detect NaNs
        if (!dead && double.IsNaN(x1) || double.IsNaN(y1) || double.IsInfinity(x1) || double.IsInfinity(y1) || double.IsNaN(x2) || double.IsNaN(y2) || double.IsInfinity(x2) || double.IsInfinity(y2))
        {
            // Debug.LogWarning("Particle died");
            dead = true;
            m1 = 0;
            m2 = 0;
            r1 = 0;
            r2 = 0;
            x1 = 0;
            y1 = 0;
            a1 = 0;
            a2 = 0;
            a1_v = 0;
            a2_v = 0;
            a1_a = 0;
            a2_a = 0;
            x2 = 0;
            y2 = 0;
        }
        
        return new Vector3[]
        {
            new Vector3((float)x1, (float)y1, 0),
            new Vector3((float)x2, (float)y2, 0)
        };
    }
}