using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Class for Agents of type Prey
public class PreyController : AgentController
{

    private Transform predator_pos;
    ///Awake is called when the script instnce is loaded
    void Awake()
    {
        if (!is_test)
        {
            GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            EC = FindObjectOfType<EnvironmentController>();
            spawn_point = EC.prey_pos;
        }
    }

    ///Detects collision with GameObjects that have PredatorController script attached
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Predator"))
        {
            GetEaten(collision.collider.GetComponent<PredatorController>());
        }
    }
    ///Ensures that Prey object is destroyed properly in case of collision with Predator
    private void GetEaten(PredatorController predator)
    {
        predator.cur_energy = predator.energy;
        predator.network.AddFitness(2f);
        Die(-5);
    }
    ///Create a clone of this object with identical NNet but slighty different parameters (speed, size , etc.)
    public override void Reproduce()
    {
        GameObject obj = Instantiate(succesor, transform.position, transform.rotation);
        obj.GetComponent<PreyController>().layers = (int[])layers.Clone();
        obj.GetComponent<PreyController>().network = new NNet(this.network);
        obj.GetComponent<PreyController>().network.SetFitness(0);
        obj.GetComponent<PreyController>().delta_decision_time = delta_decision_time;
        obj.GetComponent<PreyController>().sight_radius = sight_radius;
        EC.MutateAgent(obj.GetComponent<PreyController>(), EC.config.prey_traits, this);
        EC.prey_count++;
        obj.GetComponent<PreyController>().spawn_point = EC.prey_pos;
    }

    ///Gets angle value and distance to the nearest objects of type Predator and Food (returns as array)
    public override float[] InputSensors()
    {
        Collider2D[] hit_food = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Food"));
        Collider2D[] hit_predator = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Predator"));
        List<float> input_arr = new List<float>();
        Collider2D min = null;
        for (int i = 0; i < hit_food.Length; i++)
        {
            if (min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_food[i].transform.position))
            {
                min = hit_food[i];
            }
        }
        Vector2 vec = min.transform.position - transform.position;
        float angle = Vector2.SignedAngle(transform.up, vec);
        input_arr.Add(angle / 180f);
        input_arr.Add(Vector2.Distance(transform.position, min.transform.position) / sight_radius);
        if(min == null)
        {
            min = this.GetComponent<Collider2D>();
        }
        target = min.transform;
        min = null;
        for (int i = 0; i < hit_predator.Length; i++)
        {
            if (min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_predator[i].transform.position))
            {
                min = hit_predator[i];
            }
        }
        if (min != null)
        {
            predator_pos = min.transform;
        }else
        {
            min = this.GetComponent<Collider2D>();
        }
        vec = min.transform.position - transform.position;
        angle = Vector2.SignedAngle(transform.up, vec);
        input_arr.Add(angle / 180f);
        input_arr.Add(Vector2.Distance(transform.position, min.transform.position) / sight_radius);
        return input_arr.ToArray();
    }
    ///Destroys GameObject sending according signal to EnvironmentController
    public override void Die(int fitness)
    {
        this.network.AddFitness(fitness);
        EC.AddPrey(this.network);
        if (EC.UImgr.camera_pos_ref == this.gameObject)
        {
            EC.UImgr.ReleaseCamera();
        }
        Destroy(gameObject);
    }

    public override void drawLine()
    {
        if(target == null || predator_pos == null)
        {
            line.enabled = false;
            return;
        } 
        line.positionCount = 3;
        line.SetPosition(0, target.position);
        line.SetPosition(1, transform.position);
        line.SetPosition(2, predator_pos.position);
        line.enabled = true;
    }
}
