using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;

///Class that controls Environment floaw? learning prosses and agent balance, serves as link between other classes
public class EnvironmentController : MonoBehaviour
{
    // Name of file with some trained networks
    const string file_name = "NNet.dat";

    [Header("Entities Controller / Environment Settings")]
    ///Reference to FoodSpawner script
    public FoodSpawner FS;
    ///Lists of agents` NNets
    public List<NNet> preys = new List<NNet>();
    public List<NNet> predators = new List<NNet>();
    public int prey_count;
    public int predator_count;
    //public int start_prey_count;
    //public int start_predator_count;
    //public double best_prey_ratio;
    //public double best_predator_ratio;
    //public float mutation_rate;
    public GameObject prey_obj;
    public GameObject predator_obj;
    public Transform predator_pos;
    public Transform prey_pos;
    //private int best_prey_count;
    //private int best_predator_count;
    private PreyController[] prey_objs;
    private PredatorController[] pred_objs;
    [Header("Configuration")]
    public SetupManager.Config config; 
    ///UIManager reeference
    [Header("UI Manager")]
    public UIManager UImgr;

    ///Agents mutation probability and offset values
    //[Header("Mutation settings")]
    //public float reproduction_mutation_prob;
    //public float spawn_mutation_prob;
    //public float maturity_offset_prey;
    //public float hunger_offset_prey;
    //public float speed_offset_prey;
    //public float size_offset_prey;
    //public float maturity_offset_predator;
    //public float hunger_offset_predator;
    //public float speed_offset_predator;
    //public float size_offset_predator;

    ///Minimal allowed agent parameters
    //[Header("Minimal Allowed values")]
    //public float maturity_min_prey;
    //public float hunger_min_prey;
    //public float speed_min_prey;
    //public float maturity_min_predator;
    //public float hunger_min_predator;
    //public float speed_min_predator;

    ///Default agent parameters
    [Header("Default parameters")]
    //public float maturity_def_prey;
    //public float hunger_def_prey;
    //public float speed_def_prey;
    public Color def_color_prey;
    //public Vector2 def_size_prey;
    //public float maturity_def_predator;
    //public float hunger_def_predator;
    //public float speed_def_predator;
    public Color def_color_predator;
    //public Vector2 def_size_predator;

    ///Array of Sprites
    [Header("Sprite Array")]
    public Sprite[] SA;

    [Header("Room Colliders")]
    //top -> right -> bottom -> left
    public List<GameObject> borders;



    private void Start()
    {
        setConfig();
        updateConfig();
        spawnPreys();
        spawnPredators();
        FS.Spawn_food(FS.spawn_num);
    }

    private void setConfig()
    {
        FS.spawn_room = config.room;
        System.Tuple<Vector2, Vector2> spawns = getSpawnPos(config.room);
        prey_pos.position = spawns.Item1;
        predator_pos.position = spawns.Item2;
        borders[0].transform.position = new Vector2(0 , config.room.y / 2.0f);
        borders[0].transform.localScale = new Vector2(config.room.x, 0.5f);
        borders[1].transform.position = new Vector2(config.room.x / 2.0f, 0);
        borders[1].transform.localScale = new Vector2(0.5f, config.room.y);
        borders[2].transform.position = new Vector2(0, -config.room.y / 2.0f);
        borders[2].transform.localScale = new Vector2(config.room.x, 0.5f);
        borders[3].transform.position = new Vector2(-config.room.x / 2.0f, 0);
        borders[3].transform.localScale = new Vector2(0.5f, config.room.y);
    }

    private void updateConfig()
    {
        FS.spawn_num = config.food_drop_num;
        FS.rand_spawn_rate = config.food_drop_rate;
        FS.food_lifespan = config.food_lifespan;
    }

    private System.Tuple<Vector2, Vector2> getSpawnPos(Vector2Int spawnRoom)
    {
        if(spawnRoom.x >= spawnRoom.y)
        {
            return System.Tuple.Create(new Vector2(-spawnRoom.x / 2.0f, 0), new Vector2(spawnRoom.x / 2.0f, 0));
        }
        else
        {
            return System.Tuple.Create(new Vector2(0, -spawnRoom.y / 2.0f), new Vector2(0, spawnRoom.y/2.0f));
        }
    }

    private int getBestAgentCount(int num, float ratio)
    {
        return Mathf.CeilToInt(num * ratio);
    }

    private int[] getLayers(int input_layers, List<int> hidden_layers)
    {
        int[] layers = new int[hidden_layers.Count + 2];
        layers[0] = input_layers;
        layers[layers.Length - 1] = 2;
        for(int i  = 0; i < hidden_layers.Count; i++)
        {
            layers[i+1] = hidden_layers[i];
        }
        return layers;
    }

    private void spawnPreys()
    {
        for (int i = 0; i < config.prey_count; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(transform.position.x - FS.spawn_room.x / 2, transform.position.x + FS.spawn_room.x / 2), Random.Range(transform.position.y - FS.spawn_room.y/2, transform.position.y + FS.spawn_room.y/2));
            GameObject new_obj = Instantiate(prey_obj, spawnPos, Quaternion.identity);
            PreyController controller = new_obj.GetComponent<PreyController>();
            controller.layers = getLayers(4, config.prey_net);
            controller.network = new NNet(controller.layers);
            controller.spawn_point = prey_pos;
            MutateAgent(controller, config.prey_traits, true);
        }
        prey_count = config.prey_count;
    }

    private void spawnPredators()
    {
        for (int i = 0; i < config.predator_count; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(transform.position.x - FS.spawn_room.x/2, transform.position.x + FS.spawn_room.x/2), Random.Range(transform.position.y - FS.spawn_room.y/2, transform.position.y + FS.spawn_room.y/2));
            GameObject new_obj = Instantiate(predator_obj, spawnPos, Quaternion.identity);
            PredatorController controller = new_obj.GetComponent<PredatorController>();
            controller.layers = getLayers(2, config.predator_net);
            controller.network = new NNet(controller.layers);
            controller.spawn_point = predator_pos;
            MutateAgent(controller, config.predator_traits, false);
        }

        predator_count = config.predator_count;
    }

    ///Mutates Agent parameters
    void MutateAgent(AgentController controller, List<SetupManager.Trait> traits, bool is_prey)
    {
        controller.speed = traits[0].getValue();
        controller.energy = traits[1].getValue();
        controller.maturity = traits[2].getValue();
        float size_mod = traits[3].getValue();
        controller.size.x *= size_mod;
        controller.size.y *= size_mod;
        Color randcolor;
        if (is_prey)
        {
            randcolor = new Color(def_color_prey.r, def_color_prey.g, Random.Range(0f, 1f));
        }
        else
        {
            randcolor = new Color(def_color_predator.r, def_color_predator.g, Random.Range(0f, 1f));
        }
        controller.sprite = SA[Random.Range(0, SA.Length)];
        controller.color = randcolor;
    }

    public void MutateAgent(AgentController controller, List<SetupManager.Trait> traits, AgentController parent)
    {
        controller.speed = traits[0].getValue(parent.speed);
        controller.energy = traits[1].getValue(parent.energy);
        controller.maturity = traits[2].getValue(parent.maturity);
        controller.size = parent.size;
        controller.sprite = parent.sprite;
        controller.color = parent.color;
    }

    ///Slighty changes some PreyController parameters
    //void MutatePrey(PreyController prey)
    //{
    //    float rand = Random.Range(0f, 1f);

    //    prey.speed = speed_def_prey;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-speed_offset_prey, speed_offset_prey);
    //        if (prey.speed + t >= speed_min_prey)
    //        {
    //            prey.speed += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    prey.energy = hunger_def_prey;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
    //        if (prey.energy + t >= hunger_min_prey)
    //        {
    //            prey.energy += t;
    //        }
    //    }
    //    rand = Random.Range(0f, 1f);
    //    prey.maturity = maturity_def_prey;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-maturity_offset_prey, maturity_offset_prey);
    //        if (prey.maturity + t >= maturity_min_prey)
    //        {
    //            prey.maturity += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    prey.size = new Vector2(def_size_prey.x, def_size_prey.y);
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float offst = Random.Range(-size_offset_prey, +size_offset_prey);
    //        if (prey.size.x - offst >= 0.01 && prey.size.y - offst >= 0.01)
    //        {
    //            prey.size = new Vector2(def_size_prey.x + offst, def_size_prey.y + offst);
    //        }
    //    }
    //    Color randcolor = new Color(def_color_prey.r, def_color_prey.g, Random.Range(0f, 1f));
    //    prey.sprite = SA[Random.Range(0, SA.Length)];
    //    prey.color = randcolor;
    //}

    /////Slighty changes some PredatorController parameters
    //void MutatePredator(PredatorController predator)
    //{
    //    float rand = Random.Range(0f, 1f);

    //    predator.speed = speed_def_predator;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-speed_offset_predator, speed_offset_predator);
    //        if (predator.speed + t >= speed_min_predator)
    //        {
    //            predator.speed += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    predator.energy = hunger_def_predator;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
    //        if (predator.energy + t >= hunger_min_predator)
    //        {
    //            predator.energy += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    predator.maturity = maturity_def_predator;
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float t = Random.Range(-maturity_offset_predator, maturity_offset_predator);
    //        if (predator.maturity + t >= maturity_min_predator)
    //        {
    //            predator.maturity += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    predator.size = new Vector2(def_size_predator.x, def_size_predator.y);
    //    if (rand <= spawn_mutation_prob)
    //    {
    //        float offst = Random.Range(-size_offset_predator, +size_offset_predator);
    //        if (predator.size.x - offst >= 0.01 && predator.size.y - offst >= 0.01)
    //        {
    //            predator.size = new Vector2(def_size_predator.x + offst, def_size_predator.y + offst);
    //        }
    //    }
    //    Color randcolor = new Color(def_color_predator.r, def_color_predator.g, Random.Range(0f, 1f));
    //    predator.sprite = SA[Random.Range(0, SA.Length)];
    //    predator.color = randcolor;
    //}

    ///Slighty changes certain PreyConttroller scripts parameters according to the parent script parameters
    //public void ReproducePrey(PreyController parent, PreyController prey)
    //{
    //    float rand = Random.Range(0f, 1f);

    //    prey.speed = parent.speed;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-speed_offset_prey, speed_offset_prey);
    //        if (prey.speed + t >= speed_min_prey)
    //        {
    //            prey.speed += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    prey.energy = parent.energy;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
    //        if (prey.energy + t >= hunger_min_prey)
    //        {
    //            prey.energy += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    prey.maturity = parent.maturity;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-maturity_offset_prey, maturity_offset_prey);
    //        if (prey.maturity + t >= maturity_min_prey)
    //        {
    //            prey.maturity += t;
    //        }
    //    }

    //    prey.size = parent.size;
    //    prey.color = parent.color;
    //    prey.sprite = parent.sprite;
    //}

    ///Slighty changes certain PredatorConttroller scripts parameters according to the parent script parameters
    //public void ReproducePredator(PredatorController parent, PredatorController predator)
    //{
    //    float rand = Random.Range(0f, 1f);

    //    predator.speed = parent.speed;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-speed_offset_predator, speed_offset_predator);
    //        if (predator.speed + t >= speed_min_predator)
    //        {
    //            predator.speed += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    predator.energy = parent.energy;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
    //        if (predator.energy + t >= hunger_min_predator)
    //        {
    //            predator.energy += t;
    //        }
    //    }

    //    rand = Random.Range(0f, 1f);
    //    predator.maturity = parent.maturity;
    //    if (rand <= reproduction_mutation_prob)
    //    {
    //        float t = Random.Range(-maturity_offset_predator, maturity_offset_predator);
    //        if (predator.maturity + t >= maturity_min_predator)
    //        {
    //            predator.maturity += t;
    //        }
    //    }

    //    predator.size = parent.size;
    //    predator.color = parent.color;
    //    predator.sprite = parent.sprite;
    //}

    ///Sets UIManager camera reference to GameObject that has PreyController script with highest fitness value of NNet attached to it
    public void FiitestPrey()
    {
        PreyController temp = prey_objs[0];
        for(int i = 1; i < prey_objs.Length; i++)
        {
            if(NNet.CompareNNets(prey_objs[i].network, temp.network) == 1)
            {
                temp = prey_objs[i];
            }
        }
        temp.GetLockedOn();
    }

    ///Sets UIManager camera reference to GameObject that has PredatorController script with highest fitness value of NNet attached to it
    public void FiitestPredator()
    {
        PredatorController temp = pred_objs[0];
        for (int i = 1; i < pred_objs.Length; i++)
        {
            if (NNet.CompareNNets(pred_objs[i].network, temp.network) == 1)
            {
                temp = pred_objs[i];
            }
        }
        temp.GetLockedOn();
    }

    /// FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if(prey_count <= 0 || predator_count <= 0)
        {
            // Ecosystem is dead, recreate
            Repopulate();
            Debug.Log("ECOSYSTEM IS DEAD");
        }
        prey_objs = FindObjectsOfType<PreyController>();
        prey_count = prey_objs.Length;
        pred_objs = FindObjectsOfType<PredatorController>();
        predator_count = pred_objs.Length;
        UImgr.UpdateUI(prey_objs, pred_objs);
    }

    ///Destroys any remaining AgentController or Food instances in the scene and Spawns next generation of Agents 
    public void Repopulate()
    {
        UImgr.ReleaseCamera();
        GameObject[] gameobj = GameObject.FindGameObjectsWithTag("Food");
        for(int i = 0; i < gameobj.Length; i++)
        {
            Destroy(gameobj[i]);
        }
        if(prey_count > 0)
        {
            GameObject[] prey_objs = GameObject.FindGameObjectsWithTag("Prey");
            for (int i = 0; i < prey_objs.Length; i++)
            {
                //prey_objs[i].GetComponent<PreyController>().network.AddFitness(10f);
                prey_objs[i].GetComponent<PreyController>().Die(0);
            }
        }

        if(predator_count > 0)
        {
            GameObject[] pred_objs = GameObject.FindGameObjectsWithTag("Predator");
            for (int i = 0; i < pred_objs.Length; i++)
            {
                //pred_objs[i].GetComponent<PredatorController>().network.AddFitness(5f);
                pred_objs[i].GetComponent<PredatorController>().Die(0);
            }
        }
        int best_prey_count = getBestAgentCount(config.prey_count, config.prey_chosen_ratio);
        int num = config.prey_count / best_prey_count;
        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < best_prey_count; i++)
            {
                Vector2 spawnPos = new Vector2(Random.Range(transform.position.x - FS.spawn_room.x, transform.position.x + FS.spawn_room.x), Random.Range(transform.position.y - FS.spawn_room.y, transform.position.y + FS.spawn_room.y));
                GameObject obj = Instantiate(prey_obj, spawnPos, Quaternion.identity);
                obj.GetComponent<PreyController>().network = new NNet(preys[i]);
                obj.GetComponent<PreyController>().network.SetFitness(0f);
                float rand = Random.Range(0f, 1f);
                if (rand <= config.prey_neuron_mutation_chance * i)
                {
                    obj.GetComponent<PreyController>().network.Mutate();
                }
                MutateAgent(obj.GetComponent<PreyController>(), config.prey_traits, true);
            }
        }
        int best_predator_count = getBestAgentCount(config.predator_count, config.predator_chosen_ratio);
        num = config.predator_count / best_predator_count;
        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < best_predator_count; i++)
            {
                Vector2 spawnPos = new Vector2(Random.Range(transform.position.x - FS.spawn_room.x, transform.position.x + FS.spawn_room.x), Random.Range(transform.position.y - FS.spawn_room.y, transform.position.y + FS.spawn_room.y));
                GameObject obj = Instantiate(predator_obj, spawnPos, Quaternion.identity);
                obj.GetComponent<PredatorController>().network = new NNet(predators[i]);
                obj.GetComponent<PredatorController>().network.SetFitness(0f);
                float rand = Random.Range(0f, 1f);
                if (rand <= config.predator_chosen_ratio * i)
                {
                    obj.GetComponent<PredatorController>().network.Mutate();
                }
                MutateAgent(obj.GetComponent<PredatorController>(), config.predator_traits, false);
              
            }
        }
        FS.ResetFoodSpawnRate();
        FS.Spawn_food(FS.spawn_num);
        preys.Clear();
        predators.Clear();
        UImgr.gen_count++;
        prey_count = config.prey_count;
        predator_count = config.predator_count;
    }
    
    ///Pauses simulation
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    ///Unpauses siulation
    public void UnPause()
    {
        Time.timeScale = 1f;
    }

    ///Adds new NNet record of PreyController script 
    public void AddPrey(NNet net)
    {
        preys.Add(new NNet(net));
        preys.Sort((net1, net2) => NNet.CompareNNets(net1, net2));
        if(preys.Count > getBestAgentCount(config.prey_count, config.prey_chosen_ratio))
        {
            preys.RemoveAt(preys.Count - 1);
        }
    }

    ///Adds new NNet record of PredatorController script
    public void AddPredator(NNet net)
    {
        predators.Add(new NNet(net));
        predators.Sort((net1, net2) => NNet.CompareNNets(net1, net2));
        if (predators.Count > getBestAgentCount(config.predator_count, config.predator_chosen_ratio))
        {
            predators.RemoveAt(predators.Count - 1);
        }
    }

    ///Writes NNet Pool to .dat file and repopulates the Environment
    public void WriteFile()
    {
        if (prey_count > 0)
        {
            GameObject[] prey_objs = GameObject.FindGameObjectsWithTag("Prey");
            for (int i = 0; i < prey_objs.Length; i++)
            {
                prey_objs[i].GetComponent<PreyController>().Die(0);
            }
        }

        if (predator_count > 0)
        {
            GameObject[] pred_objs = GameObject.FindGameObjectsWithTag("Predator");
            for (int i = 0; i < pred_objs.Length; i++)
            {
                pred_objs[i].GetComponent<PredatorController>().Die(0);
            }
        }
        using (var stream = File.Open(file_name, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(preys.Count);
                for(int i = 0; i < preys.Count; i++)
                {
                    
                    int[] t_layers = preys[i].GetLayers();
                    float[][][] t_weights = preys[i].GetWeights();
                    writer.Write(t_layers.Length);
                    for (int j = 0; j < t_layers.Length; j++)
                    {
                        writer.Write(t_layers[j]);
                    }
                    writer.Write(t_weights.Length);
                    for(int j = 0; j < t_weights.Length; j++)
                    {
                        writer.Write(t_weights[j].Length);
                        for(int k = 0; k < t_weights[j].Length; k++)
                        {
                            writer.Write(t_weights[j][k].Length);
                            for(int l = 0; l < t_weights[j][k].Length; l++)
                            {
                                writer.Write(t_weights[j][k][l]);
                            }
                        }
                    }
                }
                writer.Write(predators.Count);
                for (int i = 0; i < predators.Count; i++)
                {

                    int[] t_layers = predators[i].GetLayers();
                    float[][][] t_weights = predators[i].GetWeights();
                    writer.Write(t_layers.Length);
                    for (int j = 0; j < t_layers.Length; j++)
                    {
                        writer.Write(t_layers[j]);
                    }
                    writer.Write(t_weights.Length);
                    for (int j = 0; j < t_weights.Length; j++)
                    {
                        writer.Write(t_weights[j].Length);
                        for (int k = 0; k < t_weights[j].Length; k++)
                        {
                            writer.Write(t_weights[j][k].Length);
                            for (int l = 0; l < t_weights[j][k].Length; l++)
                            {
                                writer.Write(t_weights[j][k][l]);
                            }
                        }
                    }
                }
            }
        }
    }

    ///Reads NNet Pool from .dat file and repopulates the Environment
    public void ReadFile()
    {
        if (File.Exists(file_name))
        {
            using(var stream = File.Open(file_name, FileMode.Open))
            {
                using(var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    int obj_len = reader.ReadInt32();
                    for(int i = 0; i < obj_len; i++)
                    {
                        List<int> new_layers = new List<int>();
                        int len = reader.ReadInt32();
                        for(int j = 0; j < len; j++)
                        {
                            new_layers.Add(reader.ReadInt32());
                        }
                        len = reader.ReadInt32();
                        List<float[][]> www_list = new List<float[][]>();
                        for(int j = 0; j < len; j++)
                        {
                            List<float[]> ww_list = new List<float[]>();
                            int len1 = reader.ReadInt32();
                            for(int k = 0; k < len1; k++)
                            {
                                List<float> w_list = new List<float>();
                                int len2 = reader.ReadInt32();
                                for(int l = 0; l < len2; l++)
                                {
                                    w_list.Add(reader.ReadSingle());
                                }
                                ww_list.Add(w_list.ToArray());
                            }
                            www_list.Add(ww_list.ToArray());
                        }
                        preys.Add(new NNet(new_layers.ToArray(), www_list.ToArray()));
                    }

                    obj_len = reader.ReadInt32();
                    for (int i = 0; i < obj_len; i++)
                    {
                        List<int> new_layers = new List<int>();
                        int len = reader.ReadInt32();
                        for (int j = 0; j < len; j++)
                        {
                            new_layers.Add(reader.ReadInt32());
                        }
                        len = reader.ReadInt32();
                        List<float[][]> www_list = new List<float[][]>();
                        for (int j = 0; j < len; j++)
                        {
                            List<float[]> ww_list = new List<float[]>();
                            int len1 = reader.ReadInt32();
                            for (int k = 0; k < len1; k++)
                            {
                                List<float> w_list = new List<float>();
                                int len2 = reader.ReadInt32();
                                for (int l = 0; l < len2; l++)
                                {
                                    w_list.Add(reader.ReadSingle());
                                }
                                ww_list.Add(w_list.ToArray());
                            }
                            www_list.Add(ww_list.ToArray());
                        }
                        predators.Add(new NNet(new_layers.ToArray(), www_list.ToArray()));
                    }
                    Repopulate();
                }
            }
        }
    }

}
