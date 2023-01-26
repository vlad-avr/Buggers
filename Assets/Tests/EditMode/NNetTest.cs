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
}
