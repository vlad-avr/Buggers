using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{

    [Header("Food Spawn Settings")]
    public float rand_spawn_rate;
    public int spawn_num;
    private float cur_rand_spawn_rate;
    public Food food_obj;
    public Vector3 spawn_room;
    // Start is called before the first frame update
    void Start()
    {
        cur_rand_spawn_rate = rand_spawn_rate;
    }

    // Update is called once per frame
    void Update()
    {
        if (cur_rand_spawn_rate >= rand_spawn_rate)
        {
            Spawn_food(spawn_num);
            cur_rand_spawn_rate = 0;
        }
        else
        {
            cur_rand_spawn_rate += Time.deltaTime;
        }
    }

    public void Spawn_food(int num)
    {
        while (num > 0)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-spawn_room.x, spawn_room.x), Random.Range(-spawn_room.y, spawn_room.y), 0);
            Instantiate(food_obj, spawn_pos, food_obj.transform.rotation);
            num--;
        }
    }

    public void ResetFoodSpawnRate()
    {
        cur_rand_spawn_rate = rand_spawn_rate;
    }
}
