using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Food settings")]
    public float lifetime;

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

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PreyController>())
        {
            GetEaten(collision.gameObject.GetComponent<PreyController>());
        }
    }

    private void GetEaten(PreyController prey)
    {
        prey.cur_hunger = 0;
        prey.network.AddFitness(1f);
        Destroy(gameObject);
    }
}
