using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class NNet : MonoBehaviour
{
    public float fitness;
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
        else if(fitness < other.GetFitness())
        {
            return -1;
        }
        else
        {
            return 0;
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

   /* public struct Matrix
    {
        public double[][] matrix;
        public int rows, columns;

        public Matrix(int rows_count, int columns_count)
        {
            rows = rows_count;
            columns = columns_count;
            matrix = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new double[columns];
            }
        }

        public void Clear()
        {
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    matrix[i][j] = 0;
                }
            }
        }

        public void Tanh_norm()
        {
            for (int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    matrix[i][j] = Math.Tanh(matrix[i][j]);
                }
            }
        }

        public void Add_bias(double bias)
        {
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    matrix[i][j] += bias;
                }
            }
        }

        public void Randomise(int num_of_points)
        {
            for(int i = 0; i < num_of_points; i++)
            {
                int rand_row = Random.Range(0, rows);
                int rand_column = Random.Range(0, columns);
                matrix[rand_row][rand_column] = Mathf.Clamp((float)(matrix[rand_row][rand_column] + Random.Range(-1f, 1f)), -1f, 1f);
            }
        }

    }

    public Matrix inputLayer = new Matrix(1, 3);

    public List<Matrix> hiddenLayers = new List<Matrix>();

    public Matrix outputLayer = new Matrix(1, 2);

    public List<Matrix> weights = new List<Matrix>();

    public List<double> biases = new List<double>();

    public float fitness;

    public void Initialise(int hiddenLayerCount, int hiddenNeuronCount)
    {

        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {

            Matrix f = new Matrix(1, hiddenNeuronCount);

            hiddenLayers.Add(f);

            biases.Add(Random.Range(-1f, 1f));

            //WEIGHTS
            if (i == 0)
            {
                Matrix inputToH1 = new Matrix(3, hiddenNeuronCount);
                weights.Add(inputToH1);
            }

            Matrix HiddenToHidden = new Matrix(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(HiddenToHidden);

        }

        Matrix OutputWeight = new Matrix(hiddenNeuronCount, 2);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();

    }

    public NNet InitialiseCopy(int hiddenLayerCount, int hiddenNeuronCount)
    {
        NNet n = new NNet();

        List<Matrix> newWeights = new List<Matrix>();

        for (int i = 0; i < this.weights.Count; i++)
        {
            Matrix currentWeight = new Matrix(weights[i].rows, weights[i].columns);

            for (int x = 0; x < currentWeight.rows; x++)
            {
                for (int y = 0; y < currentWeight.columns; y++)
                {
                    currentWeight.matrix[x][y] = weights[i].matrix[x][y];
                }
            }

            newWeights.Add(currentWeight);
        }

        List<double> newBiases = new List<double>();

        newBiases.AddRange(biases);

        n.weights = newWeights;
        n.biases = newBiases;

        n.InitialiseHidden(hiddenLayerCount, hiddenNeuronCount);

        return n;
    }

    public void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {
            Matrix newHiddenLayer = new Matrix(1, hiddenNeuronCount);
            hiddenLayers.Add(newHiddenLayer);
        }

    }

    public void RandomiseWeights()
    {

        for (int i = 0; i < weights.Count; i++)
        {

            for (int x = 0; x < weights[i].rows; x++)
            {

                for (int y = 0; y < weights[i].columns; y++)
                {

                    weights[i].matrix[x][y] = Random.Range(-1f, 1f);

                }

            }

        }

    }

    public (float, float) RunNetwork(float a, float b, float c)
    {
        inputLayer.matrix[0][0] = a;
        inputLayer.matrix[0][1] = b;
        inputLayer.matrix[0][2] = c;

        inputLayer.Tanh_norm();

        //hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();
        hiddenLayers[0] = Matr_mult(inputLayer, weights[0]);
        hiddenLayers[0].Add_bias(biases[0]);
        hiddenLayers[0].Tanh_norm();
        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            //hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
            hiddenLayers[i] = Matr_mult(hiddenLayers[i - 1], weights[i]);
            hiddenLayers[i].Add_bias(biases[i]);
            hiddenLayers[i].Tanh_norm();
        }

        //outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();
        outputLayer = Matr_mult(hiddenLayers[hiddenLayers.Count - 1], weights[weights.Count - 1]);
        outputLayer.Add_bias(biases[biases.Count - 1]);
        outputLayer.Tanh_norm();

        //First output is acceleration and second output is steering
        return ((float)Sigmoid(outputLayer.matrix[0][0]), (float)Math.Tanh(outputLayer.matrix[0][1]));
    }

    private Matrix Matr_mult(Matrix matr1, Matrix matr2)
    {
        Matrix res = new Matrix(matr1.rows, matr2.columns);
        for (int i = 0; i < res.rows; i++)
        {
            for (int j = 0; j < res.columns; j++)
            {
                for (int k = 0; k < matr1.columns; k++)
                {
                    res.matrix[i][j] += matr1.matrix[i][k] * matr2.matrix[k][j];
                }
            }
        }
        return res;
    }

    private double Sigmoid(double s)
    {
        return (1 / (1 + Math.Exp(-s)));
    }


    /* public Matrix input_matr = new Matrix(1,3);
     public List<Matrix> hidden_layers = new List<Matrix>();
     public Matrix output_matr = new Matrix(1,2);
     public List<Matrix> weights = new List<Matrix>();
     public List<double> biases = new List<double>();

     public float fitness;

     public void Initialise(int hidde_layer_count, int hidden_neuron_count)
     {
         hidden_layers.Clear();
         weights.Clear();
         biases.Clear();
         input_matr.Clear();
         output_matr.Clear();

         for (int i = 0; i < hidde_layer_count + 1; i++)
         {
             Matrix temp = new Matrix(1, hidden_neuron_count);
             hidden_layers.Add(temp);
             biases.Add(Random.Range(-1f, 1f));
             if (i == 0)
             {
                 Matrix w1 = new Matrix(3, hidden_neuron_count);
                 weights.Add(w1);
             }

             Matrix hidden_to_hidden = new Matrix(hidden_neuron_count, hidden_neuron_count);
             weights.Add(hidden_to_hidden);
         }
         Matrix out_weight = new Matrix(hidden_neuron_count, 2);
         weights.Add(out_weight);
         biases.Add(Random.Range(-1f, 1f));
         Randomise_weights();
     }

     public NNet CopyNet(int hidden_layers_count, int hidden_neurons_count)
     {
         NNet n = new NNet();
         List<Matrix> new_weights = new List<Matrix>();
         for(int i = 0; i < weights.Count; i++)
         {
             Matrix weight_matr = new Matrix(weights[i].rows, weights[i].columns);
             for(int j = 0; j < weight_matr.rows; j++)
             {
                 for(int k = 0; k < weight_matr.columns; k++)
                 {
                     weight_matr.matrix[j][k] = weights[i].matrix[j][k];
                 }
             }
         }

         List<double> new_biases = new List<double>();
         new_biases.AddRange(biases);
         n.weights = new_weights;
         n.biases = new_biases;
         n.InitHidden(hidden_layers_count, hidden_neurons_count);
         return n;
     }

     public void InitHidden(int hidden_layers_count, int hidden_neurons_count)
     {
         input_matr.Clear();
         hidden_layers.Clear();
         output_matr.Clear();
         for(int i = 0; i < hidden_layers_count + 1; i++)
         {
             Matrix new_matr = new Matrix(1, hidden_neurons_count);
             hidden_layers.Add(new_matr);
         }
     }

     private void Randomise_weights()
     {
         for(int i = 0; i < weights.Count; i++)
         {
             for(int j = 0; j < weights[i].rows; j++)
             {
                 for(int k = 0; k < weights[i].columns; k++)
                 {
                     weights[i].matrix[j][k] = Random.Range(-1f, 1f);
                 }
             }
         }
     }

     public (float, float) RunNetwork(double input1, double input2, double input3)
     {
         input_matr.matrix[0][0] = input1;
         input_matr.matrix[0][1] = input2;
         input_matr.matrix[0][2] = input3;
         input_matr.Tanh_norm();
         hidden_layers[0] = Matr_mult(input_matr,weights[0]);
         hidden_layers[0].Add_bias(biases[0]);
         hidden_layers[0].Tanh_norm();
         for(int i = 1; i < hidden_layers.Count; i++)
         {
             hidden_layers[i] = Matr_mult(hidden_layers[i - 1], weights[i]);
             hidden_layers[i].Add_bias(biases[i]);
             hidden_layers[i].Tanh_norm();
         }
         output_matr = Matr_mult(hidden_layers[hidden_layers.Count - 1], weights[weights.Count - 1]);
         output_matr.Add_bias(biases[biases.Count - 1]);
         output_matr.Tanh_norm();
         return ((float)Sigmoid(output_matr.matrix[0][0]),(float)Math.Tanh(output_matr.matrix[0][1]));
     }



     private double Sigmoid(double val)
     {
         return (1 / (1 + Math.Exp(-val)));
     }*/
}
