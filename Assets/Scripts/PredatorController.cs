using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///Class for Agents of type Predator
public class PredatorController : AgentController
{
    ///Awake is called when the script instnce is loaded
    void Awake()
    {
        if (!is_test)
        {
            GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            EC = FindObjectOfType<EnvironmentController>();
            spawn_point = EC.predator_pos;
        }
    }

    ///Create a clone of this object with identical NNet but slighty different parameters (speed, size , etc.)
    public override void Reproduce()
    {
        GameObject obj = Instantiate(succesor, transform.position, transform.rotation);
        obj.GetComponent<PredatorController>().layers = (int[])layers.Clone();
        obj.GetComponent<PredatorController>().network = new NNet(this.network);
        obj.GetComponent<PredatorController>().network.SetFitness(0);
        obj.GetComponent<PredatorController>().delta_decision_time = delta_decision_time;
        obj.GetComponent<PredatorController>().sight_radius = sight_radius;
        EC.MutateAgent(obj.GetComponent<PredatorController>(), EC.config.predator_traits, this);
        EC.predator_count++;
        obj.GetComponent<PredatorController>().spawn_point = EC.predator_pos;
    }

    public override void drawLine()
    {
        if(target == null)
        {
            line.enabled = false;
            return;
        }
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
        line.enabled = true;
    }

    ///Gets angle value and distance to the nearest object of type Prey (returns as array)
    public override float[] InputSensors()
    {
        Collider2D[] hit_predator = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Prey"));
        List<float> input_arr = new List<float>();
        Collider2D min = null;
        for (int i = 0; i < hit_predator.Length; i++)
        {
            if (min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_predator[i].transform.position))
            {
                min = hit_predator[i];
            }
        }
        if (min == null)
        {
            min = this.GetComponent<Collider2D>();
        }
        target = min.transform;
        Vector2 vec = min.transform.position - transform.position;
        float angle = Vector2.Angle(transform.up, vec);
        if (angle >= 180f)
        {
            angle = -1f * (360f - angle);
        }
        input_arr.Add(angle / 180f);
        input_arr.Add(Vector3.Distance(transform.position, min.transform.position) / sight_radius);

        return input_arr.ToArray();

    }

    ///Destroys GameObject sending according signal to EnvironmentController
    public override void Die(int fitness)
    {
        this.network.AddFitness(fitness);
        EC.AddPredator(network);
        if (EC.UImgr.camera_pos_ref == this.gameObject)
        {
            EC.UImgr.ReleaseCamera();
        }
        Destroy(gameObject);
    }
}
