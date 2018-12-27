using System;
using System.Collections.Generic;
using System.Linq;

public interface INeuron
{
    double GetOutput();
    void UpdateWeight(double error);
    void CopyWeight(INeuron neuron);
    void CopyWeight(WeightCarrier carrier);
    double[] GetWeight();
    INeuron[] GetInputs();
    string ToString();
    string GetWeightInPrintableFormat();
}

public class InputNeuron : INeuron
{
    private static readonly double[] EMPTY_WEIGHT_LIST = { };
    private static readonly INeuron[] EMPTY_INPUTS_LIST = { };

    private Player player;
    private readonly int idx;

    public InputNeuron(Player player, int idx)
    {
        this.player = player;
        this.idx = idx;
    }

    public void UpdateInput(Player newPlayer)
    {
        player = newPlayer;
    }

    public double GetOutput()
    {
        return player.GetSensorInput(idx);
    }

    public double[] GetWeight()
    {
        return EMPTY_WEIGHT_LIST;
    }

    public INeuron[] GetInputs()
    {
        return EMPTY_INPUTS_LIST;
    }

    public void UpdateWeight(double error)
    {
        // Nothing
    }

    public void CopyWeight(INeuron neuron)
    {
        // Nothing
    }

    public void CopyWeight(WeightCarrier carrier)
    {
        // Nothing
    }

    public override string ToString()
    {
        return GetOutput().ToString();
    }

    public string GetWeightInPrintableFormat()
    {
        return "1";
    }
}

public abstract class Neuron: INeuron
{
    protected readonly Random rnd;
    protected readonly double threshold;

    protected INeuron[] inputs;
    protected double[] weights;

    public Neuron(params INeuron[] neurons)
    {
        rnd = new Random(this.GetHashCode());

        threshold = rnd.NextDouble() * 2 - 1;
        this.inputs = new INeuron[neurons.Length];
        this.weights = new double[neurons.Length];

        SetInputNeurons(neurons);
    }

    private void SetInputNeurons(params INeuron[] neurons)
    {
        for (int i=0; i<neurons.Length; i++)
        {
            inputs[i] = neurons[i];
            weights[i] = rnd.NextDouble() * 10 - 5;
        }
    }
    
    public double[] GetWeight()
    {
        return weights;
    }

    public INeuron[] GetInputs()
    {
        return inputs;
    }

    public abstract double GetOutput();

    public void UpdateWeight(double error)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            double change = rnd.NextDouble() * (error * 2) - error;
            weights[i] += change;
            inputs[i].UpdateWeight(change * 0.4);
        }
    }

    public void CopyWeight(INeuron neuron)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            weights[i] = neuron.GetWeight()[i];
            inputs[i].CopyWeight(neuron.GetInputs()[i]);
        }
    }

    public void CopyWeight(WeightCarrier carrier)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            weights[i] = carrier.get();
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].CopyWeight(carrier);

        }
    }

    public override string ToString()
    {
        string result = "";
        foreach (double weight in weights)
        {
            result = result + Math.Round(weight, 2).ToString() + ", ";
        }

        return result;
    }

    public string GetWeightInPrintableFormat()
    {
        string result = string.Join(",", Array.ConvertAll(weights, x => x.ToString()));
        foreach (INeuron inputNeuron in inputs)
        {
            result = result + "," + inputNeuron.GetWeightInPrintableFormat();
        }

        return result;
    }
}

public class HiddenNeuron: Neuron
{
    public HiddenNeuron(params INeuron[] neurons) : base(neurons)
    {

    }

    public override double GetOutput()
    {
        double sum = 0f;

        for (int i = 0; i < inputs.Length; i++)
        {
            sum += inputs[i].GetOutput() * weights[i];
        }

        return (sum >= threshold) ? 1 : 0;
    }
}

public class OutputNeuron: Neuron
{
    public OutputNeuron(params INeuron[] neurons): base(neurons)
    {
        
    }

    public override double GetOutput()
    {
        double sum = 0f;

        for (int i = 0; i < inputs.Length; i++)
        {
            sum += inputs[i].GetOutput() * weights[i];
        }

        return sum;
    }
}