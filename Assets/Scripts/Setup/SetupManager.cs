using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{

    public class InputMap 
    {
        public Tuple<InputField, InputField> room;

        public List<InputField> food_inputs = new List<InputField>();

        public List<Tuple<InputField, InputField>> population_settings = new List<Tuple<InputField, InputField>>();

        public List<List<InputField>> prey_traits = new List<List<InputField>>();

        public List<List<InputField>> predator_traits = new List<List<InputField>>();

        public InputField prey_neuron_mutation;
        public InputField predator_neuron_mutation;

        public List<InputField> prey_net = new List<InputField>();
        public List<InputField> predator_net = new List<InputField>();

        public void initialize(GameObject setupUI)
        {
            Transform env_panel = setupUI.transform.Find("ENV_PANEL");
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

            prey_neuron_mutation = prey_panel.Find("N_CHANGE").Find("NEURON").GetComponent<InputField>();
            predator_neuron_mutation = predator_panel.Find("N_CHANGE").Find("NEURON").GetComponent<InputField>();
            prey_net.Add(prey_panel.Find("NET").Find("FIRST").GetComponent<InputField>());
            prey_net.Add(prey_panel.Find("NET").Find("LAST").GetComponent<InputField>());
            predator_net.Add(predator_panel.Find("NET").Find("FIRST").GetComponent<InputField>());
            predator_net.Add(predator_panel.Find("NET").Find("LAST").GetComponent<InputField>());
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
            prey_neuron_mutation.onEndEdit.AddListener(value => clampFloat(prey_neuron_mutation, 0f, 1f));
            predator_neuron_mutation.onEndEdit.AddListener(value => clampFloat(predator_neuron_mutation, 0f, 1f));


            prey_traits[0][0].onEndEdit.AddListener(value => clampFloat(prey_traits[0][0], min_speed, max_speed));
            prey_traits[1][0].onEndEdit.AddListener(value => clampFloat(prey_traits[1][0], min_maturity, max_maturity));
            prey_traits[2][0].onEndEdit.AddListener(value => clampFloat(prey_traits[2][0], min_energy, max_energy));
            prey_traits[3][0].onEndEdit.AddListener(value => clampFloat(prey_traits[3][0], min_size_mod, max_size_mod));
            prey_traits[0][1].onEndEdit.AddListener(value => clampFloat(prey_traits[0][1], 0f, 1f));
            prey_traits[0][2].onEndEdit.AddListener(value => clampFloat(prey_traits[0][2], 0f, 1f));
            prey_traits[1][1].onEndEdit.AddListener(value => clampFloat(prey_traits[1][1], 0f, 1f));
            prey_traits[1][2].onEndEdit.AddListener(value => clampFloat(prey_traits[1][2], 0f, 1f));
            prey_traits[2][1].onEndEdit.AddListener(value => clampFloat(prey_traits[2][1], 0f, 1f));
            prey_traits[2][2].onEndEdit.AddListener(value => clampFloat(prey_traits[2][2], 0f, 1f));
            prey_traits[3][1].onEndEdit.AddListener(value => clampFloat(prey_traits[3][1], 0f, 1f));
            prey_traits[3][2].onEndEdit.AddListener(value => clampFloat(prey_traits[3][2], 0f, 1f));

            predator_traits[0][0].onEndEdit.AddListener(value => clampFloat(predator_traits[0][0], min_speed, max_speed));
            predator_traits[1][0].onEndEdit.AddListener(value => clampFloat(predator_traits[1][0], min_maturity, max_maturity));
            predator_traits[2][0].onEndEdit.AddListener(value => clampFloat(predator_traits[2][0], min_energy, max_energy));
            predator_traits[3][0].onEndEdit.AddListener(value => clampFloat(predator_traits[3][0], min_size_mod, max_size_mod));
            predator_traits[0][1].onEndEdit.AddListener(value => clampFloat(predator_traits[0][1], 0f, 1f));
            predator_traits[0][2].onEndEdit.AddListener(value => clampFloat(predator_traits[0][2], 0f, 1f));
            predator_traits[1][1].onEndEdit.AddListener(value => clampFloat(predator_traits[1][1], 0f, 1f));
            predator_traits[1][2].onEndEdit.AddListener(value => clampFloat(predator_traits[1][2], 0f, 1f));
            predator_traits[2][1].onEndEdit.AddListener(value => clampFloat(predator_traits[2][1], 0f, 1f));
            predator_traits[2][2].onEndEdit.AddListener(value => clampFloat(predator_traits[2][2], 0f, 1f));
            predator_traits[3][1].onEndEdit.AddListener(value => clampFloat(predator_traits[3][1], 0f, 1f));
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
    }

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

        public List<int> prey_net;
        public List<int> predator_net;
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
    private static Vector2Int min_room = new Vector2Int(10,10);
    private static Vector2Int max_room = new Vector2Int(100, 100);

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

    private InputMap inputMap;

    private GameObject prey_net_UI;
    private GameObject predator_net_UI;
    private List<GameObject> prey_layers = new List<GameObject>();
    private List<GameObject> predator_layers = new List<GameObject>();
    public List<Tuple<Button, Button>> net_buttons = new List<Tuple<Button, Button>>();

    private void Awake()
    {
        inputMap = new InputMap();
        GameObject setupUI = GameObject.Find("SETUP");
        inputMap.initialize(setupUI);
        inputMap.setListeners();
        setupNetRefs(setupUI);
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
        copy_layer.transform.parent = net_objs[0].transform.parent;
        copy_layer.transform.position = new Vector3(net_objs[0].transform.position.x + (net_objs.Count - 1) * (net_objs[0].transform.localScale.x), net_objs[0].transform.position.y, net_objs[0].transform.position.z);
        copy_layer.GetComponent<InputField>().readOnly = false;
        copy_layer.GetComponent<InputField>().text = value;
        inputMap.addNetLayerInput(input_fileds, copy_layer.GetComponent<InputField>());
    }

    private void removeNetLayer(List<GameObject> net_objs, List<InputField> input_fileds)
    {
        if (net_objs.Count - 2 <= min_layers)
        {
            return;
        }
        GameObject obj_ref = net_objs[net_objs.Count - 1];
        net_objs.RemoveAt(net_objs.Count - 1);
        input_fileds.RemoveAt(input_fileds.Count - 1);
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

}
