using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

///Neural Network class
public class NNet
{
    ///Defines how efficient particular NNet is
    private float fitness;
    ///NNet matrices
    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    ///New NNet constructor
    ///@param layers array of ints that define how many neurons are in every layer
     public NNet(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitWeights();
    }

    ///Copying constructor
    public NNet(NNet copy_network)
    {
        this.layers = new int[copy_network.layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = copy_network.layers[i];
        }
        InitNeurons();
        InitWeights();
        this.fitness = copy_network.fitness;
        CopyWeights(copy_network.weights);
    }


    ///Copying constructor (copies directly from weights matrices and layers array)
    public NNet(int[] layers, float[][][] weights)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitWeights();
        CopyWeights(weights);
    }


    ///Copies weights
    private void CopyWeights(float[][][] copy_weights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copy_weights[i][j][k];
                }
            }
        }
    }

    ///Initializes neuron matrix
    private void InitNeurons()
    {
        List<float[]> neuron_list = new List<float[]>();
        for(int i = 0; i < layers.Length; i++)
        {
            neuron_list.Add(new float[layers[i]]);
        }
        neurons = neuron_list.ToArray();
    }

    ///Initializes weights matrices
    private void InitWeights()
    {
        List<float[][]> weight_list = new List<float[][]>();
        for(int i = 1; i < layers.Length; i++) 
        {
            List<float[]> layer_weight_list = new List<float[]>();
            int neurons_in_previous_layer = layers[i - 1];
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuron_weights = new float[neurons_in_previous_layer];
                for(int k = 0; k < neurons_in_previous_layer; k++)
                {
                    neuron_weights[k] = Random.Range(-0.5f, 0.5f);
                }
                layer_weight_list.Add(neuron_weights);
            }
            weight_list.Add(layer_weight_list.ToArray());
        }
        weights = weight_list.ToArray();
    }

    ///Feeds forward data until all the layers of neurons are passed
    ///@param inputs array of sensory data passed by AgentController script
    public float[] FeedForward(float[] inputs)
    {
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for(int i = 1; i < layers.Length; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                //const bias
                float value = 0f;
                for(int  k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length-1];
    }

    ///Changes some weights in NNet
    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];
                    float rand_gen = Random.Range(0f, 1f);
                    if (rand_gen <= 0.05f)
                    {
                        weight *= -1f;
                    }
                    else if(rand_gen <= 0.15f)
                    {
                        weight = Random.Range(-0.5f, 0.5f);
                    }
                    else if(rand_gen <= 0.6f)
                    {
                        float factor = Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if(rand_gen <= 0.8f)
                    {
                        float factor = Random.Range(0f, 1f);
                        weight *= factor;
                    }


                    weights[i][j][k] = weight;
                }
            }
        }
    }

    ///Adds fitness
    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    ///Sets new fitness
    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    ///Returns fitness
    public float GetFitness()
    {
        return fitness;
    }

    ///Compares to NNets according to their fitnesses
    ///@returns 1 if other NNet is NULL or has lower fitness and 0 otherwise (-1 should not be returned)
    public static int CompareNNets(NNet net1, NNet net2)
    {
        if(net2 == null || net1 == null)
        {
            return 1;
        }
        if(net1.fitness > net2.fitness)
        {
            return -1;
        }
        else if(net1.fitness < net2.fitness)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    ///Returns weights matrices
    public float[][][] GetWeights()
    {
        return weights;
    }

    ///Returns layers array
    public int[] GetLayers()
    {
        return layers;
    }

}
