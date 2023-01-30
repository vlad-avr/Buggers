using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NNetTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NNetCopyTest()
    {
        // Use the Assert class to test conditions
        int[] layers = { 2,2,2};
        NNet orig = new NNet(layers);
        NNet copy = new NNet(orig);
        for (int i = 0; i < layers.Length; i++)
        {
            Assert.AreEqual(orig.GetLayers()[i], copy.GetLayers()[i]);
        }
        float[][][] orig_weights = orig.GetWeights();
        float[][][] copy_weights = copy.GetWeights();
        Assert.AreEqual(orig_weights.Length, copy_weights.Length);
        for(int i = 0; i < orig_weights.Length; i++)
        {
            Assert.AreEqual(orig_weights[i].Length, copy_weights[i].Length);
            for(int j = 0; j < layers[i]; j++)
            {
                Assert.AreEqual(orig_weights[i][j].Length, copy_weights[i][j].Length);
                for(int k = 0; k < orig_weights[i][j].Length; k++)
                {
                    Assert.AreEqual(orig_weights[i][j][k], copy_weights[i][j][k]);
                }
            }
        }
    }
    [Test]
    public void NNetFeedForwardTest()
    {
        int[] layers = { 2, 4, 4, 2 };
        NNet net = new NNet(layers);
        float[] inp = { Random.Range(0f, 1f), Random.Range(0f, 1f) };
        float[] res = net.FeedForward(inp);
        for(int i = 0; i < layers[layers.Length-1]; i++)
        {
            Assert.IsTrue(res[i] >= -1f && res[i] <= 1f);
        }
       
    }
    [Test]
    public void NNetFitnessInfTest()
    {
        int[] layers = { 2, 2, 2 };
        NNet net = new NNet(layers);
        net.SetFitness(10);
        Assert.AreEqual(10, net.GetFitness());
        net.AddFitness(5);
        Assert.AreEqual(15, net.GetFitness());
        NNet net_other = new NNet(layers);
        net_other.SetFitness(20);
        int t = net.CompareNNets(net_other);
        Assert.IsTrue(t == 0);
    }
}
