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
    public int start_gen_count;
    public int start_predator_count;
    public float mutation_rate;
    public GameObject prey_obj;
    public GameObject predator_obj;
    public Transform predator_pos;
    public Transform prey_pos;
    public int best_prey_count;
    public int best_predator_count;
    private PreyController[] prey_objs;
    private PredatorController[] pred_objs;
    ///UIManager reeference
    [Header("UI Manager")]
    public UIManager UImgr;

    ///Agents mutation probability and offset values
    [Header("Mutation settings")]
    public float reproduction_mutation_prob;
    public float spawn_mutation_prob;
    public float maturity_offset_prey;
    public float hunger_offset_prey;
    public float speed_offset_prey;
    public float size_offset_prey;
    public float maturity_offset_predator;
    public float hunger_offset_predator;
    public float speed_offset_predator;
    public float size_offset_predator;

    ///Minimal allowed agent parameters
    [Header("Minimal Allowed values")]
    public float maturity_min_prey;
    public float hunger_min_prey;
    public float speed_min_prey;
    public float maturity_min_predator;
    public float hunger_min_predator;
    public float speed_min_predator;

    ///Default agent parameters
    [Header("Default parameters")]
    public float maturity_def_prey;
    public float hunger_def_prey;
    public float speed_def_prey;
    public Color def_color_prey;
    public Vector2 def_size_prey;
    public float maturity_def_predator;
    public float hunger_def_predator;
    public float speed_def_predator;
    public Color def_color_predator;
    public Vector2 def_size_predator;

    ///Array of Sprites
    [Header("Sprite Array")]
    public Sprite[] SA;

    ///Awake is called when the script instnce is loaded
    private void Awake()
    {
        for (int i = 0; i < start_gen_count; i++)
        {
            GameObject new_obj = Instantiate(prey_obj, prey_pos.position, Quaternion.identity);
            new_obj.GetComponent<PreyController>().network = new NNet(new_obj.GetComponent<PreyController>().layers);
            new_obj.GetComponent<PreyController>().spawn_point = prey_pos;
            MutatePrey(new_obj.GetComponent<PreyController>());
        }
        for(int i = 0; i < start_predator_count; i++)
        {
            GameObject new_obj = Instantiate(predator_obj, predator_pos.position, Quaternion.identity);
            new_obj.GetComponent<PredatorController>().network = new NNet(new_obj.GetComponent<PredatorController>().layers);
            new_obj.GetComponent<PredatorController>().spawn_point = predator_pos;
            MutatePredator(new_obj.GetComponent<PredatorController>());
        }
        prey_count = start_gen_count;
        predator_count = start_predator_count;
    }

    ///Slighty changes some PreyController parameters
    void MutatePrey(PreyController prey)
    {
        float rand = Random.Range(0f, 1f);

        prey.speed = speed_def_prey;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-speed_offset_prey, speed_offset_prey);
            if (prey.speed + t >= speed_min_prey)
            {
                prey.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.max_hunger = hunger_def_prey;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
            if (prey.max_hunger + t >= hunger_min_prey)
            {
                prey.max_hunger += t;
            }
        }
        rand = Random.Range(0f, 1f);
        prey.maturity = maturity_def_prey;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-maturity_offset_prey, maturity_offset_prey);
            if (prey.maturity + t >= maturity_min_prey)
            {
                prey.maturity += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.size = new Vector2(def_size_prey.x, def_size_prey.y);
        if (rand <= spawn_mutation_prob)
        {
            float offst = Random.Range(-size_offset_prey, +size_offset_prey);
            if (prey.size.x - offst >= 0.01 && prey.size.y - offst >= 0.01)
            {
                prey.size = new Vector2(def_size_prey.x + offst, def_size_prey.y + offst);
            }
        }
        Color randcolor = new Color(def_color_prey.r, def_color_prey.g, Random.Range(0f, 1f));
        prey.sprite = SA[Random.Range(0, SA.Length)];
        prey.color = randcolor;
    }

    ///Slighty changes some PredatorController parameters
    void MutatePredator(PredatorController predator)
    {
        float rand = Random.Range(0f, 1f);

        predator.speed = speed_def_predator;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-speed_offset_predator, speed_offset_predator);
            if (predator.speed + t >= speed_min_predator)
            {
                predator.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.max_hunger = hunger_def_predator;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
            if (predator.max_hunger + t >= hunger_min_predator)
            {
                predator.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.maturity = maturity_def_predator;
        if (rand <= spawn_mutation_prob)
        {
            float t = Random.Range(-maturity_offset_predator, maturity_offset_predator);
            if (predator.maturity + t >= maturity_min_predator)
            {
                predator.maturity += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.size = new Vector2(def_size_predator.x, def_size_predator.y);
        if (rand <= spawn_mutation_prob)
        {
            float offst = Random.Range(-size_offset_predator, +size_offset_predator);
            if (predator.size.x - offst >= 0.01 && predator.size.y - offst >= 0.01)
            {
                predator.size = new Vector2(def_size_predator.x + offst, def_size_predator.y + offst);
            }
        }
        Color randcolor = new Color(def_color_predator.r, def_color_predator.g, Random.Range(0f, 1f));
        predator.sprite = SA[Random.Range(0, SA.Length)];
        predator.color = randcolor;
    }

    ///Slighty changes certain PreyConttroller scripts parameters according to the parent script parameters
    public void ReproducePrey(PreyController parent, PreyController prey)
    {
        float rand = Random.Range(0f, 1f);

        prey.speed = parent.speed;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-speed_offset_prey, speed_offset_prey);
            if (prey.speed + t >= speed_min_prey)
            {
                prey.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.max_hunger = parent.max_hunger;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
            if (prey.max_hunger + t >= hunger_min_prey)
            {
                prey.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.maturity = parent.maturity;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-maturity_offset_prey, maturity_offset_prey);
            if (prey.maturity + t >= maturity_min_prey)
            {
                prey.maturity += t;
            }
        }

        prey.size = parent.size;
        prey.color = parent.color;
        prey.sprite = parent.sprite;
    }

    ///Slighty changes certain PredatorConttroller scripts parameters according to the parent script parameters
    public void ReproducePredator(PredatorController parent, PredatorController predator)
    {
        float rand = Random.Range(0f, 1f);

        predator.speed = parent.speed;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-speed_offset_predator, speed_offset_predator);
            if (predator.speed + t >= speed_min_predator)
            {
                predator.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.max_hunger = parent.max_hunger;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
            if (predator.max_hunger + t >= hunger_min_predator)
            {
                predator.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.maturity = parent.maturity;
        if (rand <= reproduction_mutation_prob)
        {
            float t = Random.Range(-maturity_offset_predator, maturity_offset_predator);
            if (predator.maturity + t >= maturity_min_predator)
            {
                predator.maturity += t;
            }
        }

        predator.size = parent.size;
        predator.color = parent.color;
        predator.sprite = parent.sprite;
    }

    ///Sets UIManager camera reference to GameObject that has PreyController script with highest fitness value of NNet attached to it
    public void FiitestPrey()
    {
        PreyController temp = prey_objs[0];
        for(int i = 1; i < prey_objs.Length; i++)
        {
            if(prey_objs[i].network.CompareNNets(temp.network) == 1)
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
            if (pred_objs[i].network.CompareNNets(temp.network) == 1)
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
                prey_objs[i].GetComponent<PreyController>().network.AddFitness(10f);
                prey_objs[i].GetComponent<PreyController>().Die(0);
            }
        }

        if(predator_count > 0)
        {
            GameObject[] pred_objs = GameObject.FindGameObjectsWithTag("Predator");
            for (int i = 0; i < pred_objs.Length; i++)
            {
                pred_objs[i].GetComponent<PredatorController>().network.AddFitness(5f);
                pred_objs[i].GetComponent<PredatorController>().Die(0);
            }
        }
        int num = start_gen_count / best_prey_count;
        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < best_prey_count; i++)
            {
                GameObject obj = Instantiate(prey_obj, prey_pos.position, Quaternion.identity);
                obj.GetComponent<PreyController>().network = new NNet(preys[i]);
                obj.GetComponent<PreyController>().network.SetFitness(0f);
                float rand = Random.Range(0f, 1f);
                if (rand <= mutation_rate * i)
                {
                    obj.GetComponent<PreyController>().network.Mutate();
                }
                MutatePrey(obj.GetComponent<PreyController>());
            }
        }
        num = start_predator_count / best_predator_count;
        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < best_predator_count; i++)
            {
                GameObject obj = Instantiate(predator_obj, predator_pos.position, Quaternion.identity);
                obj.GetComponent<PredatorController>().network = new NNet(predators[i]);
                obj.GetComponent<PredatorController>().network.SetFitness(0f);
                float rand = Random.Range(0f, 1f);
                if (rand <= mutation_rate * i)
                {
                    obj.GetComponent<PredatorController>().network.Mutate();
                }
                MutatePredator(obj.GetComponent<PredatorController>());
              
            }
        }
        FS.ResetFoodSpawnRate();
        preys.Clear();
        predators.Clear();
        UImgr.gen_count++;
        prey_count = start_gen_count;
        predator_count = start_predator_count;
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
        if (preys.Count == 0)
        {
            preys.Add(new NNet(net));
        }
        else
        {
            if (preys.Count == best_prey_count)
            {
                if (net.GetFitness() < preys[preys.Count - 1].GetFitness())
                {
                    return;
                }
                else
                {
                    preys.Add(new NNet(net));
                    for (int i = 0; i < preys.Count - 1; i++)
                    {
                        for (int j = i; j < preys.Count; j++)
                        {
                            if (preys[i].GetFitness() < preys[j].GetFitness())
                            {
                                NNet temp = preys[i];
                                preys[i] = preys[j];
                                preys[j] = temp;
                            }
                        }
                    }
                    preys.RemoveAt(preys.Count - 1);
                }
            }
            else
            {
                if (net.GetFitness() < preys[preys.Count - 1].GetFitness())
                {
                    preys.Add(new NNet(net));
                }
                else
                {
                    preys.Add(new NNet(net));
                    for (int i = 0; i < preys.Count - 1; i++)
                    {
                        for (int j = i; j < preys.Count; j++)
                        {
                            if (preys[i].GetFitness() < preys[j].GetFitness())
                            {
                                NNet temp = preys[i];
                                preys[i] = preys[j];
                                preys[j] = temp;
                            }
                        }
                    }
                }
            }
        }
    }

    ///Adds new NNet record of PredatorController script
    public void AddPredator(NNet net)
    {
        if (predators.Count == 0)
        {
            predators.Add(new NNet(net));
        }
        else
        {
            if (predators.Count == best_predator_count)
            {
                if (net.GetFitness() < predators[predators.Count - 1].GetFitness())
                {
                    return;
                }
                else
                {
                    predators.Add(new NNet(net));
                    for (int i = 0; i < predators.Count - 1; i++)
                    {
                        for (int j = i; j < predators.Count; j++)
                        {
                            if (predators[i].GetFitness() < predators[j].GetFitness())
                            {
                                NNet temp = predators[i];
                                predators[i] = predators[j];
                                predators[j] = temp;
                            }
                        }
                    }
                    predators.RemoveAt(predators.Count - 1);
                }
            }
            else
            {
                if (net.GetFitness() < predators[predators.Count - 1].GetFitness())
                {
                    predators.Add(new NNet(net));
                }
                else
                {
                    predators.Add(new NNet(net));
                    for (int i = 0; i < predators.Count - 1; i++)
                    {
                        for (int j = i; j < predators.Count; j++)
                        {
                            if (predators[i].GetFitness() < predators[j].GetFitness())
                            {
                                NNet temp = predators[i];
                                predators[i] = predators[j];
                                predators[j] = temp;
                            }
                        }
                    }
                }
            }
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
