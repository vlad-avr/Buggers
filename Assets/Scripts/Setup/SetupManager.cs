using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class SetupManager : MonoBehaviour
{

    public class AgentMap
    {
        public InputField name;
        public Tuple<InputField, InputField> pop;
        public List<List<InputField>> traits = new List<List<InputField>>();
        public List<InputField> net = new List<InputField>();
        public Tuple<InputField> net_mods;
    }

    public class InputMap 
    {
        public InputField name;

        public Tuple<InputField, InputField> room;

        public List<InputField> food_inputs = new List<InputField>();

        public List<Tuple<InputField, InputField>> population_settings = new List<Tuple<InputField, InputField>>();

        public List<List<InputField>> prey_traits = new List<List<InputField>>();

        public List<List<InputField>> predator_traits = new List<List<InputField>>();

        public List<Tuple<InputField, InputField>> net_mods = new List<Tuple<InputField, InputField>>();

        public List<InputField> prey_net = new List<InputField>();
        public List<InputField> predator_net = new List<InputField>();
        //public Tuple<List<AgentMap>, List<AgentMap>> agent_maps = Tuple.Create(new List<AgentMap>(), new List<AgentMap>());

        public void initialize(GameObject setupUI)
        {
            Transform env_panel = setupUI.transform.Find("ENV_PANEL");
            name = env_panel.Find("CONFIG").Find("NAME").GetComponent<InputField>();
            InputField first = env_panel.Find("ROOM").Find("ROOM_X").GetComponent<InputField>();
            InputField second = env_panel.Find("ROOM").Find("ROOM_Y").GetComponent<InputField>();
            room = Tuple.Create(first, second);
            food_inputs.Add(env_panel.Find("FOOD").Find("FOOD_RATE").GetComponent<InputField>());
            food_inputs.Add(env_panel.Find("FOOD").Find("FOOD_NUM").GetComponent<InputField>());
            food_inputs.Add(env_panel.Find("FOOD").Find("FOOD_TIME").GetComponent<InputField>());
            Transform prey_panel = setupUI.transform.Find("AGENT_PANEL").Find("PREY_PANEL");
            first = prey_panel.Find("START_NUM").Find("NUM").GetComponent<InputField>();
            second = prey_panel.Find("CHOSEN").Find("RATIO").GetComponent<InputField>();
            population_settings.Add(Tuple.Create(first, second));
            Transform predator_panel = setupUI.transform.Find("AGENT_PANEL").Find("PREDATOR_PANEL");
            first = predator_panel.Find("START_NUM").Find("NUM").GetComponent<InputField>();
            second = predator_panel.Find("CHOSEN").Find("RATIO").GetComponent<InputField>();
            population_settings.Add(Tuple.Create(first, second));

            for(int i = 0; i < 4; i++)
            {
                Transform traitUI = prey_panel.Find("TRAITS").Find("FIELD_" + i);
                prey_traits.Add(new List<InputField>());
                prey_traits[i].Add(traitUI.Find("DEFAULT").GetComponent<InputField>());
                prey_traits[i].Add(traitUI.Find("OFFSET").GetComponent<InputField>());
                prey_traits[i].Add(traitUI.Find("PROB").GetComponent<InputField>());
            }

            for (int i = 0; i < 4; i++)
            {
                Transform traitUI = predator_panel.Find("TRAITS").Find("FIELD_" + i);
                predator_traits.Add(new List<InputField>());
                predator_traits[i].Add(traitUI.Find("DEFAULT").GetComponent<InputField>());
                predator_traits[i].Add(traitUI.Find("OFFSET").GetComponent<InputField>());
                predator_traits[i].Add(traitUI.Find("PROB").GetComponent<InputField>());
            }

            first = prey_panel.Find("N_CHANGE").Find("NEURON").GetComponent<InputField>();
            second = prey_panel.Find("D_RATE").Find("DECISION").GetComponent<InputField>();
            net_mods.Add(Tuple.Create(first, second));
            first = predator_panel.Find("N_CHANGE").Find("NEURON").GetComponent<InputField>();
            second = predator_panel.Find("D_RATE").Find("DECISION").GetComponent<InputField>();
            net_mods.Add(Tuple.Create(first, second));

            //prey_net.Add(prey_panel.Find("NET").Find("FIRST").GetComponent<InputField>());
            //prey_net.Add(prey_panel.Find("NET").Find("LAST").GetComponent<InputField>());
            //predator_net.Add(predator_panel.Find("NET").Find("FIRST").GetComponent<InputField>());
            //predator_net.Add(predator_panel.Find("NET").Find("LAST").GetComponent<InputField>());
        }

        public void setListeners()
        {
            room.Item1.onEndEdit.AddListener(value => clampInt(room.Item1, min_room.x, max_room.x));
            room.Item2.onEndEdit.AddListener(value => clampInt(room.Item2, min_room.y, max_room.y));
            food_inputs[0].onEndEdit.AddListener(value => clampFloat(food_inputs[0], min_food_drop_rate, max_food_drop_rate));
            food_inputs[1].onEndEdit.AddListener(value => clampInt(food_inputs[1], min_food_drop_num, max_food_drop_num));
            food_inputs[2].onEndEdit.AddListener(value => clampFloat(food_inputs[2], min_food_lifespan, max_food_lifespan));
            population_settings[0].Item1.onEndEdit.AddListener(value => clampInt(population_settings[0].Item1, min_agent_count, max_agent_count));
            population_settings[1].Item1.onEndEdit.AddListener(value => clampInt(population_settings[1].Item1, min_agent_count, max_agent_count));
            population_settings[0].Item2.onEndEdit.AddListener(value => clampFloat(population_settings[0].Item2, min_chosen_ratio, max_chosen_ratio));
            population_settings[1].Item2.onEndEdit.AddListener(value => clampFloat(population_settings[1].Item2, min_chosen_ratio, max_chosen_ratio));
            net_mods[0].Item1.onEndEdit.AddListener(value => clampFloat(net_mods[0].Item1, 0f, 1f));
            net_mods[1].Item1.onEndEdit.AddListener(value => clampFloat(net_mods[1].Item1, 0f, 1f));
            net_mods[0].Item2.onEndEdit.AddListener(value => clampFloat(net_mods[0].Item2, 0.1f, 1f));
            net_mods[1].Item2.onEndEdit.AddListener(value => clampFloat(net_mods[1].Item2, 0.1f, 1f));

            prey_traits[0][0].onEndEdit.AddListener(value => clampFloat(prey_traits[0][0], min_speed, max_speed));
            prey_traits[1][0].onEndEdit.AddListener(value => clampFloat(prey_traits[1][0], min_maturity, max_maturity));
            prey_traits[2][0].onEndEdit.AddListener(value => clampFloat(prey_traits[2][0], min_energy, max_energy));
            prey_traits[3][0].onEndEdit.AddListener(value => clampFloat(prey_traits[3][0], min_size_mod, max_size_mod));
            prey_traits[0][1].onEndEdit.AddListener(value => clampFloat(prey_traits[0][1], 0f, 0.5f));
            prey_traits[0][2].onEndEdit.AddListener(value => clampFloat(prey_traits[0][2], 0f, 1f));
            prey_traits[1][1].onEndEdit.AddListener(value => clampFloat(prey_traits[1][1], 0f, 0.5f));
            prey_traits[1][2].onEndEdit.AddListener(value => clampFloat(prey_traits[1][2], 0f, 1f));
            prey_traits[2][1].onEndEdit.AddListener(value => clampFloat(prey_traits[2][1], 0f, 0.5f));
            prey_traits[2][2].onEndEdit.AddListener(value => clampFloat(prey_traits[2][2], 0f, 1f));
            prey_traits[3][1].onEndEdit.AddListener(value => clampFloat(prey_traits[3][1], 0f, 0.5f));
            prey_traits[3][2].onEndEdit.AddListener(value => clampFloat(prey_traits[3][2], 0f, 1f));

            predator_traits[0][0].onEndEdit.AddListener(value => clampFloat(predator_traits[0][0], min_speed, max_speed));
            predator_traits[1][0].onEndEdit.AddListener(value => clampFloat(predator_traits[1][0], min_maturity, max_maturity));
            predator_traits[2][0].onEndEdit.AddListener(value => clampFloat(predator_traits[2][0], min_energy, max_energy));
            predator_traits[3][0].onEndEdit.AddListener(value => clampFloat(predator_traits[3][0], min_size_mod, max_size_mod));
            predator_traits[0][1].onEndEdit.AddListener(value => clampFloat(predator_traits[0][1], 0f, 0.5f));
            predator_traits[0][2].onEndEdit.AddListener(value => clampFloat(predator_traits[0][2], 0f, 1f));
            predator_traits[1][1].onEndEdit.AddListener(value => clampFloat(predator_traits[1][1], 0f, 0.5f));
            predator_traits[1][2].onEndEdit.AddListener(value => clampFloat(predator_traits[1][2], 0f, 1f));
            predator_traits[2][1].onEndEdit.AddListener(value => clampFloat(predator_traits[2][1], 0f, 0.5f));
            predator_traits[2][2].onEndEdit.AddListener(value => clampFloat(predator_traits[2][2], 0f, 1f));
            predator_traits[3][1].onEndEdit.AddListener(value => clampFloat(predator_traits[3][1], 0f, 0.5f));
            predator_traits[3][2].onEndEdit.AddListener(value => clampFloat(predator_traits[3][2], 0f, 1f));

        }

        public void addNetLayerInput(List<InputField> inputs, InputField toAdd)
        {
            toAdd.onEndEdit.AddListener(value => clampInt(toAdd, min_neurons, max_neurons));
            if(inputs.Count <= 1)
            {
                inputs.Add(toAdd);
            }
            else
            {
                inputs.Insert(inputs.Count - 1, toAdd);
            }
            
        }

        public Config getConfig()
        {
            Config conf = new Config();

            conf.name = name.text;

            conf.room = new Vector2Int(int.Parse(room.Item1.text), int.Parse(room.Item2.text));
            conf.food_drop_rate = float.Parse(food_inputs[0].text);
            conf.food_drop_num = int.Parse(food_inputs[1].text);
            conf.food_lifespan = float.Parse(food_inputs[2].text);

            conf.prey_count = int.Parse(population_settings[0].Item1.text);
            conf.prey_chosen_ratio = float.Parse(population_settings[0].Item2.text);
            conf.predator_count = int.Parse(population_settings[1].Item1.text);
            conf.predator_chosen_ratio = float.Parse(population_settings[1].Item2.text);

            conf.prey_net_mods = Tuple.Create(float.Parse(net_mods[0].Item1.text), float.Parse(net_mods[0].Item2.text));
            conf.predator_net_mods = Tuple.Create(float.Parse(net_mods[1].Item1.text), float.Parse(net_mods[1].Item2.text));

            conf.prey_net = new List<int>();
            for(int i = 0; i < prey_net.Count; i++)
            {
                conf.prey_net.Add(int.Parse(prey_net[i].text));
            }
            conf.predator_net = new List<int>();
            for (int i = 0; i < predator_net.Count; i++)
            {
                conf.predator_net.Add(int.Parse(predator_net[i].text));
            }

            conf.prey_traits = new List<Trait>();
            foreach(List<InputField> traitInput in prey_traits)
            {
                conf.prey_traits.Add(new Trait(float.Parse(traitInput[0].text), float.Parse(traitInput[1].text), float.Parse(traitInput[2].text)));
            }
            conf.predator_traits = new List<Trait>();
            foreach (List<InputField> traitInput in predator_traits)
            {
                conf.predator_traits.Add(new Trait(float.Parse(traitInput[0].text), float.Parse(traitInput[1].text), float.Parse(traitInput[2].text)));
            }

            return conf;
        }

        public void setConfig(Config config)
        {
            name.text = config.name;

            room.Item1.text = config.room.x.ToString();
            room.Item2.text = config.room.y.ToString();

            food_inputs[0].text = config.food_drop_rate.ToString();
            food_inputs[1].text = config.food_drop_num.ToString();
            food_inputs[2].text = config.food_lifespan.ToString();

            population_settings[0].Item1.text = config.prey_count.ToString();
            population_settings[0].Item2.text = config.prey_chosen_ratio.ToString();
            population_settings[1].Item1.text = config.predator_count.ToString();
            population_settings[1].Item2.text = config.predator_chosen_ratio.ToString();

            for(int i = 0; i < prey_traits.Count; i++)
            {
                prey_traits[i][0].text = config.prey_traits[i].default_value.ToString();
                prey_traits[i][1].text = config.prey_traits[i].offset.ToString();
                prey_traits[i][2].text = config.prey_traits[i].probability.ToString();
            }
            for (int i = 0; i < predator_traits.Count; i++)
            {
                predator_traits[i][0].text = config.predator_traits[i].default_value.ToString();
                predator_traits[i][1].text = config.predator_traits[i].offset.ToString();
                predator_traits[i][2].text = config.predator_traits[i].probability.ToString();
            }
            net_mods[0].Item1.text = config.prey_net_mods.Item1.ToString();
            net_mods[0].Item2.text = config.prey_net_mods.Item2.ToString();
            net_mods[1].Item1.text = config.predator_net_mods.Item1.ToString();
            net_mods[1].Item2.text = config.predator_net_mods.Item2.ToString();
        }
    }

    public class Trait
    {
        public float default_value;
        public float offset;
        public float probability;

        public Trait(float default_value, float offset, float probability)
        {
            this.default_value = default_value;
            this.offset = offset;
            this.probability = probability;
        }

        public float getValue()
        {
            return getValue(default_value);
        }

        public float getValue(float value)
        {
            if(probability == 0)
            {
                return value;
            }
            else
            {
                float roll = UnityEngine.Random.Range(0f, 1f);
                if(roll <= probability)
                {
                    return value + value * UnityEngine.Random.Range(-offset, offset);
                }
                else
                {
                    return value;
                }
            }
        }

        public override string ToString()
        {
            return default_value + " " + offset + " " + probability;
        }
    }
    public class Config
    {

        /* FPRMATTING OF CONFIG ENTRIES
         * !name
         * roomX rooomY
         * fdr fdn fl
         * preyN preyR
         * predN predR
         * mut prey net layers 
         * mut pred net layer
         * for each trait:
         * prey val offset prob
         * pred val offset prob
         */
        public string name;

        public Vector2Int room;

        public float food_drop_rate;
        public int food_drop_num;
        public float food_lifespan;

        public int prey_count;
        public int predator_count;
        public float prey_chosen_ratio;
        public float predator_chosen_ratio;

        public List<int> prey_net;
        public List<int> predator_net;
        public Tuple<float, float> prey_net_mods;
        public Tuple<float, float> predator_net_mods;

        //TRAIT SEQUENCE: speed -> energy -> maturity -> size
        public List<Trait> prey_traits;
        public List<Trait> predator_traits;

        public void parseChunk(string chunk)
        {
            string[] parts = chunk.Split('\n');
            name = parts[0].Substring(1);
            string[] subparts = parts[1].Split(' ');
            room = new Vector2Int(int.Parse(subparts[0]), int.Parse(subparts[1]));
            subparts = parts[2].Split(' ');
            food_drop_rate = float.Parse(subparts[0]);
            food_drop_num = int.Parse(subparts[1]);
            food_lifespan = float.Parse(subparts[2]);

            subparts = parts[3].Split(' ');
            prey_count = int.Parse(subparts[0]);
            prey_chosen_ratio = float.Parse(subparts[1]);

            subparts = parts[4].Split(' ');
            predator_count = int.Parse(subparts[0]);
            predator_chosen_ratio = float.Parse(subparts[1]);

            subparts = parts[5].Split(' ');
            prey_net_mods = Tuple.Create(float.Parse(subparts[0]), float.Parse(subparts[1]));
            prey_net = new List<int>();
            for(int i = 2; i < subparts.Length; i++)
            {
                prey_net.Add(int.Parse(subparts[i]));
            }

            subparts = parts[6].Split(' ');
            predator_net_mods = Tuple.Create(float.Parse(subparts[0]), float.Parse(subparts[1]));
            predator_net = new List<int>();
            for (int i = 2; i < subparts.Length; i++)
            {
                predator_net.Add(int.Parse(subparts[i]));
            }

            prey_traits = new List<Trait>();
            for (int i = 7; i <= 10; i++)
            {
                subparts = parts[i].Split(' ');
                prey_traits.Add(new Trait(float.Parse(subparts[0]), float.Parse(subparts[1]), float.Parse(subparts[2])));
            }

            predator_traits = new List<Trait>();
            for (int i = 11; i <= 14; i++)
            {
                subparts = parts[i].Split(' ');
                predator_traits.Add(new Trait(float.Parse(subparts[0]), float.Parse(subparts[1]), float.Parse(subparts[2])));
            }
        }

        public string parseIntoChunk()
        {
            string res = "!" + name + "\n"
                + room.x + " " + room.y + "\n"
                + food_drop_rate + " " + food_drop_num + " " + food_lifespan + "\n"
                + prey_count + " " + prey_chosen_ratio + "\n"
                + predator_count + " " + predator_chosen_ratio + "\n";
            res += prey_net_mods.Item1 + " " + prey_net_mods.Item2;
            for(int i = 1; i < prey_net.Count-1; i++)
            {
                res += " " + prey_net[i];
            }
            res += "\n" + predator_net_mods.Item1 + " " + predator_net_mods.Item2;
            for (int i = 1; i < predator_net.Count - 1; i++)
            {
                res += " " + predator_net[i];
            }
            res += "\n";
            foreach(Trait trait in prey_traits)
            {
                res += trait.ToString() + "\n";
            }
            foreach (Trait trait in predator_traits)
            {
                res += trait.ToString() + "\n";
            }
            res += "~\n";
            return res;
        }
    }

    //Defined constants (sort of config for default enviroment)
    private static Vector2Int min_room = new Vector2Int(20,20);
    private static Vector2Int max_room = new Vector2Int(200, 200);

    private const float min_food_drop_rate = 5;
    private const float max_food_drop_rate = 100;
    private const int min_food_drop_num = 5;
    private const int max_food_drop_num = 200;
    private const float min_food_lifespan = 20;
    private const float max_food_lifespan = 100;

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

    private const int min_layers = 0;
    private const int max_layers = 10;
    private const int min_neurons = 1;
    private const int max_neurons = 16;

    private const string config_path = "Assets/Resources/Config/enironment_config.txt";
    private string[] configs;

    private InputMap inputMap;

    private GameObject prey_net_UI;
    private GameObject predator_net_UI;
    private List<GameObject> prey_layers = new List<GameObject>();
    private List<GameObject> predator_layers = new List<GameObject>();
    [Header("UI references (autocompleted)")]
    public List<Tuple<Button, Button>> net_buttons = new List<Tuple<Button, Button>>();
    public Button save_config;
    public Button load_config;
    public Button update_config;
    public Button delete_config;
    public Dropdown config_dropdown;

    [Header("EnvironmentController entity prefab")]
    public GameObject environmentController;

    [Header("Panels References")]
    public List<GameObject> canvases = new List<GameObject>();
    public List<Button> buttons = new List<Button>();
    public GameObject main_canvas;

    private void Awake()
    {
        main_canvas = GameObject.Find("MAIN");
        GameObject obj = GameObject.Find("SETUP_CANVAS");
        obj.SetActive(false);
        canvases.Add(obj);
        //obj = GameObject.Find("DATASET_CANVAS");
        //obj.SetActive(false);
        //canvases.Add(obj);
        buttons.Add(GameObject.Find("MAIN").transform.Find("MAIN_PANEL").Find("TRAINING").GetComponent<Button>());
        buttons[0].onClick.AddListener(onTrainingCanvasLoad);
        buttons.Add(GameObject.Find("MAIN").transform.Find("MAIN_PANEL").Find("DATASET").GetComponent<Button>());
    }

    public void onTrainingCanvasLoad()
    {
        setActiveCanvas(0);
        inputMap = new InputMap();
        GameObject setupUI = GameObject.Find("SETUP");
        inputMap.initialize(setupUI);
        inputMap.setListeners();
        setupNetRefs(setupUI);
        setupConfigUI(setupUI);
        loadConfig("default");
    }

    public void setActiveCanvas(int selected)
    {
        main_canvas.SetActive(false);
        canvases[selected].SetActive(true);
    }

    private void setupConfigUI(GameObject setupUI)
    {
        Transform config_panel = setupUI.transform.Find("ENV_PANEL").Find("CONFIG");
        save_config = config_panel.Find("SAVE").GetComponent<Button>();
        save_config.onClick.AddListener(saveConfig);
        load_config = config_panel.Find("LOAD").GetComponent<Button>();
        load_config.onClick.AddListener(loadConfig);
        update_config = config_panel.Find("UPDATE").GetComponent<Button>();
        update_config.onClick.AddListener(updateConfig);
        delete_config = config_panel.Find("DELETE").GetComponent<Button>();
        delete_config.onClick.AddListener(deleteConfig);
        config_dropdown = config_panel.Find("CONFIGS").GetComponent<Dropdown>();
        config_dropdown.onValueChanged.AddListener(updateNameField);
        configs = getConfigNames();
        updateDropdown();
    }

    public void initEnvironment()
    {
        GameObject controller = Instantiate(environmentController, transform.position, Quaternion.identity);
        controller.transform.SetParent(this.transform);
        controller.GetComponent<EnvironmentController>().config = inputMap.getConfig();
        GameObject.Find("SETUP_CANVAS").SetActive(false);
    }

    private void setupNetRefs(GameObject setupUI)
    {
        Transform agent_panel = setupUI.transform.Find("AGENT_PANEL");
        prey_net_UI = agent_panel.Find("PREY_PANEL").Find("NET").gameObject;
        predator_net_UI = agent_panel.Find("PREDATOR_PANEL").Find("NET").gameObject;
        prey_layers.Add(prey_net_UI.transform.Find("FIRST").gameObject);
        inputMap.addNetLayerInput(inputMap.prey_net, prey_layers[0].GetComponent<InputField>());
        prey_layers.Add(prey_net_UI.transform.Find("LAST").gameObject);
        inputMap.addNetLayerInput(inputMap.prey_net, prey_layers[1].GetComponent<InputField>());
        predator_layers.Add(predator_net_UI.transform.Find("FIRST").gameObject);
        inputMap.addNetLayerInput(inputMap.predator_net, predator_layers[0].GetComponent<InputField>());
        predator_layers.Add(predator_net_UI.transform.Find("LAST").gameObject);
        inputMap.addNetLayerInput(inputMap.predator_net, predator_layers[1].GetComponent<InputField>());
        Button add_button = agent_panel.Find("PREY_PANEL").Find("NET").Find("ADD").GetComponent<Button>();
        add_button.onClick.AddListener(addListenerPrey);
        Button remove_button = agent_panel.Find("PREY_PANEL").Find("NET").Find("REMOVE").GetComponent<Button>();
        remove_button.onClick.AddListener(removeListenerPrey);
        net_buttons.Add(Tuple.Create(add_button, remove_button));
        add_button = agent_panel.Find("PREDATOR_PANEL").Find("NET").Find("ADD").GetComponent<Button>();
        add_button.onClick.AddListener(addListenerPredator);
        remove_button = agent_panel.Find("PREDATOR_PANEL").Find("NET").Find("REMOVE").GetComponent<Button>();
        remove_button.onClick.AddListener(removeListenerPredator);
        net_buttons.Add(Tuple.Create(add_button, remove_button));
    }

    public void loadConfig()
    {
        loadConfig(inputMap.name.text);
    }

    public void updateNameField(int index)
    {
        inputMap.name.text = config_dropdown.options[index].text;
    }

    public void loadConfig(string name)
    {
        string chunk = loadChunkFromConfigFile(name);
        if(chunk == null)
        {
            return;
        }
        Config loadedConfig = new Config();
        loadedConfig.parseChunk(chunk);
        inputMap.setConfig(loadedConfig);
        loadNets(loadedConfig);
    }

    public void saveConfig()
    {
        Config config = inputMap.getConfig();
        if (configs.Contains(config.name))
        {
            Debug.Log(config.name + " already exists!");
            return;
        }
        writeToConfigFile(config.parseIntoChunk(), true);
        configs = getConfigNames();
        updateDropdown();
    }

    private void updateDropdown()
    {
        config_dropdown.ClearOptions();
        List<OptionData> options = new List<OptionData>();
        foreach (var config in configs)
        {
            options.Add(new OptionData(config));
        }
        config_dropdown.AddOptions(options);
    }

    public void deleteConfig()
    {
        deleteConfig(false);
    }

    public void deleteConfig(bool isUpdate)
    {
        if (inputMap.name.text.Equals("default"))
        {
            Debug.Log("You have no write to fool around with default config, swashbuckler!");
            return;
        }
        string chunks = "";
        foreach(var config in configs)
        {
            if (config.Equals(inputMap.name.text))
            {
                Debug.Log("FOUND");
                continue;
            }
            string chunk = loadChunkFromConfigFile(config);
            if (chunk != null)
            {
                chunks += chunk;
            }
        }
        writeToConfigFile(chunks, false);
        if (!isUpdate)
        {
            configs = getConfigNames();
            updateDropdown();
        }
    }

    public void updateConfig()
    {
        string configName = inputMap.name.text;
        deleteConfig(true);
        Config config = inputMap.getConfig();
        config.name = configName;
        writeToConfigFile(config.parseIntoChunk(), true);
    }

    private void loadNets(Config config)
    {
        while(prey_layers.Count - 2 > min_layers)
        {
            removeNetLayer(prey_layers, inputMap.prey_net);
        }
        while (predator_layers.Count - 2 > min_layers)
        {
            removeNetLayer(predator_layers, inputMap.predator_net);
        }
        for (int i = 0; i < config.prey_net.Count; i++){
            addNetLayer(prey_layers, inputMap.prey_net, config.prey_net[i].ToString());
        }
        for (int i = 0; i < config.predator_net.Count; i++)
        {
            addNetLayer(predator_layers, inputMap.predator_net, config.predator_net[i].ToString());
        }
    }

    private void addListenerPrey()
    {
        addNetLayer(prey_layers, inputMap.prey_net, min_neurons.ToString());
    }

    private void removeListenerPrey()
    {
        removeNetLayer(prey_layers, inputMap.prey_net);
    }

    private void addListenerPredator()
    {
        addNetLayer(predator_layers, inputMap.predator_net, min_neurons.ToString());
    }

    private void removeListenerPredator()
    {
        removeNetLayer(predator_layers, inputMap.predator_net);
    }
    private void addNetLayer(List<GameObject> net_objs, List<InputField> input_fileds, string value)
    {
        if (net_objs.Count - 2 >= max_layers)
        {
            return;
        }
        GameObject copy_layer = Instantiate(net_objs[0]);
        copy_layer.transform.SetParent(net_objs[0].transform.parent, false);
        RectTransform ref_rect_transform = net_objs[0].GetComponent<RectTransform>();
        copy_layer.GetComponent<RectTransform>().anchoredPosition = new Vector2(ref_rect_transform.anchoredPosition.x + (net_objs.Count - 1) * ref_rect_transform.sizeDelta.x, ref_rect_transform.anchoredPosition.y);
        copy_layer.GetComponent<InputField>().readOnly = false;
        copy_layer.GetComponent<InputField>().text = value;
        net_objs.Insert(net_objs.Count - 1, copy_layer);
        inputMap.addNetLayerInput(input_fileds, copy_layer.GetComponent<InputField>());
    }

    private void removeNetLayer(List<GameObject> net_objs, List<InputField> input_fileds)
    {
        if (net_objs.Count - 2 <= min_layers)
        {
            return;
        }
        GameObject obj_ref = net_objs[net_objs.Count - 2];
        net_objs.RemoveAt(net_objs.Count - 2);
        input_fileds.RemoveAt(input_fileds.Count - 2);
        Destroy(obj_ref);
    }
    public static void clampFloat(InputField field, float min, float max)
    {
        if (float.TryParse(field.text, out float floatValue))
        {
            // Clamp the value within the specified range
            floatValue = Mathf.Clamp(floatValue, min, max);

            // Update the input field text with the clamped value
            field.text = floatValue.ToString();
        }
        else
        {
            field.text = min.ToString();
        }
    }

    public static void clampInt(InputField field, int min, int max)
    {
        if (float.TryParse(field.text, out float rawVal))
        {
            int intValue = Mathf.FloorToInt(rawVal);
            // Clamp the value within the specified range
            intValue = Mathf.Clamp(intValue, min, max);

            // Update the input field text with the clamped value
            field.text = intValue.ToString();
        }
        else
        {
            field.text = min.ToString();
        }
    }

    private void writeToConfigFile(string toWrite, bool toAppend)
    {

        try
        {
            // Open the file with a StreamWriter in Append mode
            using (StreamWriter writer = new StreamWriter(config_path, toAppend))
            {
                // Append the new content to the file
                writer.WriteLine(toWrite);
            }
        }
        catch (IOException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private string loadChunkFromConfigFile(string configName)
    {
        string loaded = null;
        try
        {
            // Open the file with a StreamReader
            using (StreamReader reader = new StreamReader(config_path))
            {
                string line;

                // Read lines until the end of the file
                while ((line = reader.ReadLine()) != null)
                {
                    // Process each line as needed
                    if (line.Equals("!" + configName))
                    {
                        break;
                    }
                }
                loaded = "";
                do
                {
                    loaded += line + "\n";
                } while ((line = reader.ReadLine()) != null && !line.Equals("~"));
                loaded += "~\n";
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return loaded;
    }

    private string[] getConfigNames()
    {
        List<string> names = new List<string>();
        try
        {
            // Open the file with a StreamReader
            using (StreamReader reader = new StreamReader(config_path))
            {
                string line;
                // Read lines until the end of the file
                while ((line = reader.ReadLine()) != null)
                {
                    // Process each line as needed
                    if (line.StartsWith("!"))
                    {
                        names.Add(line.Substring(1, line.Length-1));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return names.ToArray();
    }

}
