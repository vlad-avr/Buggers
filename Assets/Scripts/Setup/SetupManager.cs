using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager
{

    public class Trait
    {
        public float default_value;
        public float offset;
        public float probability;
    }
    public class Config
    {
        public string name;

        public Vector2 room;

        public float food_drop_rate;
        public int food_drop_num;
        public float food_lifespan;

        public int prey_count;
        public int predator_count;
        public float prey_chosen_ratio;
        public float predator_chosen_ratio;

        public int prey_layers;
        public int predator_layers;
        public int prey_neurons;
        public int predator_neurons;
        public float prey_neuron_mutation_chance;
        public float predator_neuron_mutation_chance;

        public Trait prey_speed;
        public Trait predator_speed;
        public Trait prey_energy;
        public Trait predator_energy;
        public Trait prey_maturity;
        public Trait predator_maturity;
        public Trait prey_size_mod;
        public Trait predator_size_mod;
    }

    //Defined constants (sort of config for default enviroment)
    private Vector2 min_room = new Vector2(10,10);
    private Vector2 max_room = new Vector2(100, 100);

    private const float min_food_drop_rate = 5;
    private const float max_food_drop_rate = 100;
    private const int min_food_drop_num = 5;
    private const int max_food_drop_num = 200;
    private const float min_food_lifespan = 20;
    private const float max_food_lisfespan = 100;

    private const int min_agent_count = 1;
    private const int max_agent_count = 150;
    private const float min_chosen_ratio = 0.05f;
    private const float max_chosen_ratio = 1;

    private const float min_speed = 0.5f;
    private const float max_speed = 20f;
    private const float min_energy = 5;
    private const float max_energy = 100;
    private const float min_maturity = 10;
    private const float max_maturity = 100;
    private const float min_size_mod = 0.1f;
    private const float max_size_mod = 4;

    private const int min_layers = 1;
    private const int max_layers = 10;
    private const int min_neurons = 1;
    private const int max_neurons = 16;
    private const string config_path = "Assets/Resources/Config/enironment_config.txt";
    
}
