using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class NNet
{
    private float fitness;
    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

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

    public NNet(NNet copy_network)
    {
        this.layers = new int[copy_network.layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = copy_network.layers[i];
        }
        //this.fitness = copy_network.fitness;
        InitNeurons();
        InitWeights();
        CopyWeights(copy_network.weights);
    }

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

    private void InitNeurons()
    {
        List<float[]> neuron_list = new List<float[]>();
        for(int i = 0; i < layers.Length; i++)
        {
            neuron_list.Add(new float[layers[i]]);
        }
        neurons = neuron_list.ToArray();
    }

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

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public int CompareNNets(NNet other)
    {
        if(other == null)
        {
            return 1;
        }
        if(fitness > other.GetFitness())
        {
            return 1;
        }
        else if(fitness <= other.GetFitness())
        {
            return 0;
        }
        else
        {
            //should not trigger
            return -1;
        }
    }

    public float[][][] GetWeights()
    {
        return weights;
    }

    public int[] GetLayers()
    {
        return layers;
    }

}
