using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Class represents food for Prey gameObjects
public class Food : MonoBehaviour
{
    ///How long before the object is destroyed
    [Header("Food settings")]
    public float lifetime;
    /// FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifetime -= Time.deltaTime;
        }
    }
    ///Detects if object has collided with object of type PreyController
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PreyController>())
        {
            GetEaten(collision.gameObject.GetComponent<PreyController>());
        }
    }

    ///Ensures that food object is destroyed properly when collided with object of type PreyController
    private void GetEaten(PreyController prey)
    {
        prey.cur_energy = 0;
        prey.network.AddFitness(1f);
        Destroy(gameObject);
    }
}
