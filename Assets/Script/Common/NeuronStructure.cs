using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public class NeuronStructure
{
    Player player;

    private InputNeuron[] inputNeurons;
    private HiddenNeuron[][] hiddenNeurons;
    private OutputNeuron[] outputNeurons;

    List<int> ancestors = new List<int>();

    public int rank;

    public NeuronStructure(Player inputPlayer, params int[] neuronNumers)
    {
        rank = -1;
        player = inputPlayer;
        CreateLayer(neuronNumers);
    }

    public void CreateLayer(params int[] neuronNumers)
    {
        if (neuronNumers.Length < 2)
        {
            throw new FormatException("Neuron Structure should include at leat input, output nueorn layer. CURRENT: " + neuronNumers.Length);
        }

        inputNeurons = new InputNeuron[neuronNumers[0]];
        for (int i = 0; i < inputNeurons.Length; i++)
        {
            inputNeurons[i] = new InputNeuron(player, i);
        }

        hiddenNeurons = new HiddenNeuron[neuronNumers.Length - 2][];
        for (int i = 0; i < hiddenNeurons.Length; i++)
        {
            hiddenNeurons[i] = new HiddenNeuron[neuronNumers[i + 1]];

            INeuron[] prevNeurons = (i == 0) ? (INeuron[])inputNeurons : (INeuron[])hiddenNeurons[i - 1];
            for (int j = 0; j < hiddenNeurons[i].Length; j++)
            {
                hiddenNeurons[i][j] = new HiddenNeuron(prevNeurons);
            }
        }

        outputNeurons = new OutputNeuron[neuronNumers[neuronNumers.Length - 1]];
        {
            INeuron[] prevNeurons = (neuronNumers.Length == 2) ? (INeuron[])inputNeurons : (INeuron[])hiddenNeurons[hiddenNeurons.Length - 1];
            for (int i = 0; i < outputNeurons.Length; i++)
            {
                outputNeurons[i] = new OutputNeuron(prevNeurons);
            }
        }
    }

    public float GetOuput(int idx)
    {
        return (float)outputNeurons[idx].GetOutput();
    }

    public void UpdateWeight()
    {
        for (int i=0; i<outputNeurons.Length; i++)
        {
            outputNeurons[i].UpdateWeight(player.GetError());
        }
    }

    public void CopyWeight(NeuronStructure neuronStructure)
    {
        for (int i = 0; i < outputNeurons.Length; i++)
        {
            outputNeurons[i].CopyWeight(neuronStructure.outputNeurons[i]);
        }

        ancestors.Add(neuronStructure.getId());
    }

    public void CopyWeight(double[][] weights)
    {
        for (int i = 0; i < outputNeurons.Length; i++)
        {
            outputNeurons[i].CopyWeight(new WeightCarrier(weights[i]));
        }
    }

    public void UpdateInput(Player newPlayer)
    {
        player = newPlayer;

        for (int i = 0; i < inputNeurons.Length; i++)
        {
            inputNeurons[i].UpdateInput(player);
        }
    }

    public double GetPriority()
    {
        return player.GetPriority();
    }

    public int getId()
    {
        return GetHashCode();
    }

    public string GetWeightInPrintableFormat()
    {
        string result = "";

        foreach(INeuron neuron in outputNeurons)
        {
            result = result + neuron.GetWeightInPrintableFormat() + "\n";
        }

        return result;
    }
}

public class NeuronStructureFactory
{
    private const string START_WEIGHT_FILE_NAME = "input_weight.txt";

    private static readonly int SIZE = 32;
    private static readonly int PART = 5;
    private static readonly int[] NEURON_NUMBERS = { 270, 2, 3, 4 };

    private static double[][][] weights = null;

    private static int step = 0;
    private static NeuronStructure[] structures = new NeuronStructure[SIZE];
    private static int index = 0;

    public static NeuronStructure CreateStructure(Player player)
    {
        lock (structures)
        {
            LoadStartWeights();

            index = (index + 1) % SIZE;

            if (structures[index] == null)
            {
                structures[index] = new NeuronStructure(player, NEURON_NUMBERS);

                if (weights != null)
                {
                    structures[index].CopyWeight(weights[index % PART]);
                }
            }
            else
            {
                structures[index].UpdateInput(player);
            }
        }

        return structures[index];
    }

    private static void LoadStartWeights()
    {
        if (File.Exists(START_WEIGHT_FILE_NAME) == false)
        {
            return;
        }

        if (weights != null)
        {
            return;
        }

        string[] lines = File.ReadAllLines(START_WEIGHT_FILE_NAME);
        int setCnt = lines.Length / NEURON_NUMBERS[NEURON_NUMBERS.Length - 1];
        int outputNeuronCnt = NEURON_NUMBERS[NEURON_NUMBERS.Length - 1];

        weights = new double[setCnt][][];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = new double[outputNeuronCnt][];

            for (int j = 0; j < outputNeuronCnt; j++)
            {
                weights[i][j] = Array.ConvertAll(lines[index / PART].Split(','), x => double.Parse(x));
            }
        }
    }

    public static void MoveToNextStep()
    {
        step++;

        Array.Sort(structures, (x, y) => x.GetPriority().CompareTo(y.GetPriority()));

        RecordWeights();

        Swap(structures.Length - 4, structures.Length * 2 / 3);

        int from = 0;
        int to = from + structures.Length / PART;

        for (int j = 1; j < PART; j++)
        {
            for (int i = from; i < to; i++)
            {
                structures[i].CopyWeight(structures[structures.Length - j]);
                structures[i].UpdateWeight();
                structures[i].rank = -1;
            }

            from = to;
            to = to + structures.Length / PART;
        }

        for (int i = from; i < structures.Length - PART; i++)
        {
            structures[i].CopyWeight(structures[structures.Length - PART]);
            structures[i].UpdateWeight();
            structures[i].rank = -1;
        }


        for (int i = 1; i <= PART; i++)
        {
            structures[structures.Length - i].rank = i;
        }
    }

    //public static void MoveToNextStep()
    //{
    //    step++;

    //    Array.Sort(structures, (x, y) => x.GetPriority().CompareTo(y.GetPriority()));
    //    structures[structures.Length - 1].UpdateWeight();

    //    for (int i=0; i<structures.Length-1; i++)
    //    {
    //        structures[i].CopyWeight(structures[structures.Length - 1]);
    //    }
    //}


    private static void RecordWeights()
    {
        string result = "";

        for (int i = structures.Length - PART; i < structures.Length; i++)
        {
            result = result + structures[i].GetWeightInPrintableFormat();
        }

        File.WriteAllText("result_weight_" + step + ".txt", result);
    }

    private static void Swap(int a, int b)
    {
        NeuronStructure temp = structures[a];
        structures[a] = structures[b];
        structures[b] = temp;
    }

    public static int GetStep()
    {
        return step;
    }
}