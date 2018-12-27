using UnityEngine;
using UnityEditor;
using System;

public class WeightCarrier
{
    private double[] weights;
    private int idx;

    public WeightCarrier(double[] weights)
    {
        this.weights = weights;
        idx = 0;
    }

    public double get()
    {
        if (idx >= weights.Length)
        {
            throw new Exception("No more weights in carrier!");
        }

        return weights[idx++];
    }
}