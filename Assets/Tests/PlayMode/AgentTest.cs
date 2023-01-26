using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AgentTest
{
    private GameObject prey, pred;
    private PreyController prey_ctrl;
    private PredatorController pred_ctrl;
    [SetUp]
    public void SetUp()
    {
        prey = GameObject.Instantiate(Resources.Load<GameObject>("Prey_test"), new Vector3(-5, 0, 0), Quaternion.identity);
        prey_ctrl = prey.GetComponent<PreyController>();
        pred = GameObject.Instantiate(Resources.Load<GameObject>("Predator_test"), new Vector3(5, 0, 0), Quaternion.identity);
        pred_ctrl = pred.GetComponent<PredatorController>();
    }
   
    [UnityTest]
    public IEnumerator AgentMoveTest()
    {
        Vector3 prey_orig = prey.transform.position;
        Vector3 pred_orig = pred.transform.position;
        prey_ctrl.Move(0);
        pred_ctrl.Move(0);
        yield return new WaitForSeconds(0.1f);
        Assert.AreNotEqual(prey_orig, prey.transform.position);
        Assert.AreNotEqual(pred_orig, pred.transform.position);
    }
}
