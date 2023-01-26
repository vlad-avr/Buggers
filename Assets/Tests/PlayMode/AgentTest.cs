using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AgentTest
{

    [SetUp]
    public void SetUp()
    {
       /* prey_test = GameObject.Instantiate(Resources.Load<GameObject>("Prey"), new Vector3(0, 0, 0), Quaternion.identity);
        predator_test = GameObject.Instantiate(Resources.Load<GameObject>("Predator"), new Vector3(0, 5, 0), Quaternion.identity);
        prey_ctrl = prey_test.GetComponent<PreyController>();*/
    }
   
    [UnityTest]
    public IEnumerator AgentMoveTest()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        /* GameObject prey = new GameObject();
         prey.AddComponent<PreyController>();
         prey.AddComponent<Rigidbody2D>();
         Vector2 ctrl_pos = prey.transform.position;
         prey.GetComponent<PreyController>().Move(0f);
         yield return new WaitForSeconds(1);
         Assert.IsTrue(prey.transform.position.y > ctrl_pos.y);*/
        yield return null;
    }
}
