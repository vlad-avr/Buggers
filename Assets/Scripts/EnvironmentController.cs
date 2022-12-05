using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;

public class EnvironmentController : MonoBehaviour
{
    // Name of file with some trained networks
    const string file_name = "NNet.dat";


    [Header("Food Spawn Settings")]
    public float rand_spawn_rate;
    public int spawn_num;
    private float cur_rand_spawn_rate;
    public Food food_obj;
    public Vector3 spawn_room;

    [Header("Entities Controller / Environment Settings")]
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

    [Header("UI settings")]
    public TextMeshProUGUI cur_gen_text, population_text;
    public GameObject info_panel;
    public TextMeshProUGUI type_text, speed_text, fit_text, maturity_text, sight_text;
    public Camera main_camera;
    public float camera_zoom;
    private float def_zoom;
    public GameObject camera_pos_ref;
    //public TMP_Dropdown prey_list, predator_list;
    private int gen_count;


    [Header("Mutation settings")]
    public float reproduction_mutation_prob_prey;
    public float mutation_prob_prey;
    public float maturity_offset_prey;
    public float hunger_offset_prey;
    public float speed_offset_prey;
    public float size_offset_prey;
    public float sight_offset_prey;
    public float reproduction_mutation_prob_predator;
    public float mutation_prob_predator;
    public float maturity_offset_predator;
    public float hunger_offset_predator;
    public float speed_offset_predator;
    public float size_offset_predator;
    public float sight_offset_predator;

    [Header("Minimal Allowed values")]
    public float maturity_min_prey;
    public float hunger_min_prey;
    public float speed_min_prey;
    public float sight_min_prey;
    public float maturity_min_predator;
    public float hunger_min_predator;
    public float speed_min_predator;
    public float sight_min_predator;

    [Header("Default parameters")]
    public float maturity_def_prey;
    public float hunger_def_prey;
    public float speed_def_prey;
    public float sight_def_prey;
    public Color def_color_prey;
    public Vector2 def_size_prey;
    public float maturity_def_predator;
    public float hunger_def_predator;
    public float speed_def_predator;
    public float sight_def_predator;
    public Color def_color_predator;
    public Vector2 def_size_predator;

    [Header("Sprite Array")]
    public Sprite[] SA;
    private void Awake()
    {
        gen_count = 0;
        for (int i = 0; i < start_gen_count; i++)
        {
            GameObject new_obj = Instantiate(prey_obj, prey_pos.position, Quaternion.identity);
            new_obj.GetComponent<PreyController>().network = new NNet(new_obj.GetComponent<PreyController>().layers);
            new_obj.GetComponent<PreyController>().spawn_point = prey_pos;
            MutatePrey(new_obj.GetComponent<PreyController>());
           // preys.Add(new NNet(new_obj.GetComponent<PreyController>().network));
        }
        for(int i = 0; i < start_predator_count; i++)
        {
            GameObject new_obj = Instantiate(predator_obj, predator_pos.position, Quaternion.identity);
            new_obj.GetComponent<PredatorController>().network = new NNet(new_obj.GetComponent<PredatorController>().layers);
            new_obj.GetComponent<PredatorController>().spawn_point = predator_pos;
            MutatePredator(new_obj.GetComponent<PredatorController>());
        }
        cur_rand_spawn_rate = rand_spawn_rate;
        prey_count = start_gen_count;
        predator_count = start_predator_count;
        camera_pos_ref = this.gameObject;
        def_zoom = main_camera.orthographicSize;
        info_panel.SetActive(false);
    }

    void MutatePrey(PreyController prey)
    {
        float rand = Random.Range(0f, 1f);

        prey.speed = speed_def_prey;
        if (rand <= mutation_prob_prey)
        {
            float t = Random.Range(-speed_offset_prey, speed_offset_prey);
            if (prey.speed + t >= speed_min_prey)
            {
                prey.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.max_hunger = hunger_def_prey;
        if (rand <= mutation_prob_prey)
        {
            float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
            if (prey.max_hunger + t >= hunger_min_prey)
            {
                prey.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.sight_radius = sight_def_prey;
        if (rand <= mutation_prob_prey)
        {
            float t = Random.Range(-sight_offset_prey, sight_offset_prey);
            if (prey.sight_radius + t >= sight_min_prey)
            {
                prey.sight_radius += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.maturity = maturity_def_prey;
        if (rand <= mutation_prob_prey)
        {
            float t = Random.Range(-maturity_offset_prey, maturity_offset_prey);
            if (prey.maturity + t >= maturity_min_prey)
            {
                prey.maturity += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.size = new Vector2(def_size_prey.x, def_size_prey.y);
        if (rand <= mutation_prob_prey)
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

    void MutatePredator(PredatorController predator)
    {
        float rand = Random.Range(0f, 1f);

        predator.speed = speed_def_predator;
        if (rand <= mutation_prob_predator)
        {
            float t = Random.Range(-speed_offset_predator, speed_offset_predator);
            if (predator.speed + t >= speed_min_predator)
            {
                predator.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.max_hunger = hunger_def_predator;
        if (rand <= mutation_prob_predator)
        {
            float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
            if (predator.max_hunger + t >= hunger_min_predator)
            {
                predator.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.sight_radius = sight_def_predator;
        if (rand <= mutation_prob_predator)
        {
            float t = Random.Range(-sight_offset_predator, sight_offset_predator);
            if (predator.sight_radius + t >= sight_min_predator)
            {
                predator.sight_radius += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.maturity = maturity_def_predator;
        if (rand <= mutation_prob_predator)
        {
            float t = Random.Range(-maturity_offset_predator, maturity_offset_predator);
            if (predator.maturity + t >= maturity_min_predator)
            {
                predator.maturity += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.size = new Vector2(def_size_predator.x, def_size_predator.y);
        if (rand <= mutation_prob_predator)
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

    public void ReproducePrey(PreyController parent, PreyController prey)
    {
        float rand = Random.Range(0f, 1f);

        prey.speed = parent.speed;
        if (rand <= reproduction_mutation_prob_prey)
        {
            float t = Random.Range(-speed_offset_prey, speed_offset_prey);
            if (prey.speed + t >= speed_min_prey)
            {
                prey.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.max_hunger = parent.max_hunger;
        if (rand <= reproduction_mutation_prob_prey)
        {
            float t = Random.Range(-hunger_offset_prey, hunger_offset_prey);
            if (prey.max_hunger + t >= hunger_min_prey)
            {
                prey.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.sight_radius = parent.sight_radius;
        if (rand <= reproduction_mutation_prob_prey)
        {
            float t = Random.Range(-sight_offset_prey, sight_offset_prey);
            if (prey.sight_radius + t >= sight_min_prey)
            {
                prey.sight_radius += t;
            }
        }

        rand = Random.Range(0f, 1f);
        prey.maturity = parent.maturity;
        if (rand <= reproduction_mutation_prob_prey)
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

    public void ReproducePredator(PredatorController parent, PredatorController predator)
    {
        float rand = Random.Range(0f, 1f);

        predator.speed = parent.speed;
        if (rand <= reproduction_mutation_prob_predator)
        {
            float t = Random.Range(-speed_offset_predator, speed_offset_predator);
            if (predator.speed + t >= speed_min_predator)
            {
                predator.speed += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.max_hunger = parent.max_hunger;
        if (rand <= reproduction_mutation_prob_predator)
        {
            float t = Random.Range(-hunger_offset_predator, hunger_offset_predator);
            if (predator.max_hunger + t >= hunger_min_predator)
            {
                predator.max_hunger += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.sight_radius = parent.sight_radius;
        if (rand <= reproduction_mutation_prob_predator)
        {
            float t = Random.Range(-sight_offset_predator, sight_offset_predator);
            if (predator.sight_radius + t >= sight_min_predator)
            {
                predator.sight_radius += t;
            }
        }

        rand = Random.Range(0f, 1f);
        predator.maturity = parent.maturity;
        if (rand <= reproduction_mutation_prob_predator)
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

    public void UpdateUI(PreyController[] preys, PredatorController[] predators)
    {
        /*prey_list.ClearOptions();
        predator_list.ClearOptions();*/
        cur_gen_text.text = gen_count.ToString();
        population_text.text = prey_count.ToString() + " / " + predator_count.ToString();
        main_camera.transform.position = new Vector3(camera_pos_ref.transform.position.x, camera_pos_ref.transform.position.y, -10);
       /* for (int i = 0; i < preys.Length; i++)
        {
            prey_list.options.Add(new TMP_Dropdown.OptionData() { text = " prey " + i.ToString() + " fit " + preys[i].network.GetFitness().ToString() });
        }
        for (int i = 0; i < predators.Length; i++)
        {
            predator_list.options.Add(new TMP_Dropdown.OptionData() { text = " predator " + i.ToString() + " fit " + predators[i].network.GetFitness().ToString() });
        }*/
    }

    public void Kill()
    {
        if (camera_pos_ref.CompareTag("Prey"))
        {
            camera_pos_ref.GetComponent<PreyController>().Die(0);
        }
        else if(camera_pos_ref.CompareTag("Predator"))
        {
            camera_pos_ref.GetComponent<PredatorController>().Die(0);
        }
    }

    public void Praise()
    {
        if (camera_pos_ref.CompareTag("Prey"))
        {
            camera_pos_ref.GetComponent<PreyController>().network.AddFitness(1);
        }
        else if (camera_pos_ref.CompareTag("Predator"))
        {
            camera_pos_ref.GetComponent<PredatorController>().network.AddFitness(1);
        }
    }

 /*   public void CrossBreed()
    {
        Pause();
        is_breeding = true;
        GameObject parent1 = camera_pos_ref;
        if (camera_pos_ref.CompareTag("Prey"))
        {
            hint_obj.SetActive(true);
        }
    }
    */
    public void UpdateInfo(string type, string fitness, string speed, string maturity, string sight)
    {
        type_text.text = type;
        fit_text.text = fitness;
        speed_text.text = speed;
        maturity_text.text = maturity;
    }

    public void LockCamera(string type, string fitness, string speed, string maturity, string sight, GameObject obj)
    {
        main_camera.orthographicSize = def_zoom;
        info_panel.SetActive(true);
        type_text.text = type;
        fit_text.text = fitness;
        speed_text.text = speed;
        maturity_text.text = maturity;
        sight_text.text = sight;
        camera_pos_ref = obj;
        main_camera.transform.position = new Vector3(camera_pos_ref.transform.position.x, camera_pos_ref.transform.position.y, -10);
        main_camera.orthographicSize *= camera_zoom;
    }

    public void ReleaseCamera()
    {
        info_panel.SetActive(false);
        camera_pos_ref = this.gameObject;
        main_camera.orthographicSize = def_zoom;
    }

    public void FiitestPrey()
    {
        PreyController temp = prey_objs[0];
        for(int i = 1; i < prey_objs.Length; i++)
        {
            if(temp.network.fitness < prey_objs[i].network.fitness)
            {
                temp = prey_objs[i];
            }
        }
        temp.GetLockedOn();
    }

    public void FiitestPredator()
    {
        PredatorController temp = pred_objs[0];
        for (int i = 1; i < pred_objs.Length; i++)
        {
            if (temp.network.fitness < pred_objs[i].network.fitness)
            {
                temp = pred_objs[i];
            }
        }
        temp.GetLockedOn();
    }

    /*private void UpdateAgentCount()
    {
        PreyController[] prey_objs = FindObjectsOfType<PreyController>();
        prey_count = prey_objs.Length;
        PredatorController[] pred_objs = FindObjectsOfType<PredatorController>();
        predator_count = pred_objs.Length;
    }*/

    private void FixedUpdate()
    {
        if(prey_count <= 0 || predator_count <= 0)
        {
            // Ecosystem is dead, recreate
            Repopulate();
            Debug.Log("ECOSYSTEM IS DEAD");
        }

        if (cur_rand_spawn_rate >= rand_spawn_rate)
        {
            Spawn_food(spawn_num);
            cur_rand_spawn_rate = 0;
        }
        else
        {
            cur_rand_spawn_rate += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReleaseCamera();
        }
        prey_objs = FindObjectsOfType<PreyController>();
        prey_count = prey_objs.Length;
        pred_objs = FindObjectsOfType<PredatorController>();
        predator_count = pred_objs.Length;
        UpdateUI(prey_objs, pred_objs);
    }

    public void Repopulate()
    {
        ReleaseCamera();
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
                // prey_list.Add(new NNet(obj.GetComponent<PreyController>().network));
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
                // prey_list.Add(new NNet(obj.GetComponent<PreyController>().network));
            }
        }
        cur_rand_spawn_rate = rand_spawn_rate;
        preys.Clear();
        predators.Clear();
        gen_count++;
        prey_count = start_gen_count;
        predator_count = start_predator_count;
        //  return prey_list;
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
    
    public void Pause()
    {
        Time.timeScale = 0f;
    }


    public void UnPause()
    {
        Time.timeScale = 1f;
    }

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
     /*   Debug.Log(preys.Count.ToString() + " PREYS");
        Debug.Log(preys[0].GetFitness());
        Debug.Log(preys[preys.Count - 1].GetFitness());*/
    }

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
      /*  Debug.Log(predators.Count.ToString() + " PREDATORS");
        Debug.Log(predators[0].GetFitness());
        Debug.Log(predators[predators.Count - 1].GetFitness());*/

    }

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
