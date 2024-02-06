using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Parent class for Agents
public abstract class AgentController : MonoBehaviour
{
    ///Reference to NNet script
    [Header("Neural Network")]
    public NNet network;

    ///gameObject physical parameters setting
    [Header("gameObject Settings")]
    public float energy;
    public float cur_energy;
    public float maturity;
    protected float cur_maturity;
    public float speed;
    public float angular_drag;
    public float sight_radius;
    public Vector2 size;
    public Color color;
    public Sprite sprite;
    public SpriteRenderer spr;

    ///Successor object prefab
    public GameObject succesor;
    [Header("Environment Controller reference")]
    ///Reference to EnvironmentController script in the scene
    protected EnvironmentController EC;

    [Header("Line Renderer")]
    public LineRenderer line;
    protected Transform target;
    public bool locked_on = false;

    [Header("Decision making")]
    public float delta_decision_time = 0.0001f;
    private float current_decision_time = 0;
    private float[] output;
    ///NNet script settings
    [Header("Network Options")]
    public int[] layers;
    ///Spawn point in the scene reference
    [Header("Respawn point")]
    public Transform spawn_point;
    ///Boolean varaible to switch on\off testing mode (dev tool)s
    [Header("TESTING")]
    public bool is_test;

    /// Start is called before the first frame update
    void Start()
    {
        cur_energy = energy;
        setLine();
        sight_radius = Mathf.Max(EC.config.room.x, EC.config.room.y);
        transform.localScale = size;
        spr.sprite = sprite;
        spr.color = color;
    }

    protected void setLine()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.enabled = false;
        Material lineMaterial = new Material(Shader.Find("Standard"));
        lineMaterial.color = Color.white;
        line.material = lineMaterial;
    }

    ///Detects if Agent has entered a Trigger collider
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = spawn_point.position;
        }
    }

    public float[] getOutput()
    {
        return output;
    }

    /// Update is called every frame
    void Update()
    {
        if (!is_test)
        {
            if (current_decision_time <= 0)
            {
                output = network.FeedForward(InputSensors());
                current_decision_time = delta_decision_time;
            }
            else
            {
                current_decision_time -= Time.deltaTime;
            }
            Move(output[0], output[1] + 1f);
            if (locked_on)
            {
                drawLine();
            }
            else
            {
                line.enabled = false;
            }
        }
    }
    /// FixedUpdate is called every fixed framerate frame
    void FixedUpdate()
    {
        if (!is_test)
        {
            if (cur_maturity >= maturity)
            {
                Reproduce();
                cur_maturity = 0f;
            }
            else
            {
                cur_maturity += Time.deltaTime;
            }

            if (cur_energy <= 0)
            {
                Die(-10);
            }
            else
            {
                cur_energy -= Time.deltaTime;
            }


            if (locked_on)
            {
                EC.UImgr.UpdateInfo(this);
            }
        }
    }

    ///Passes Agent data to the UI elements and resets Camera position
    public void GetLockedOn()
    {
        EC.UImgr.LockCamera(this);
    }

    ///Move function
    ///@param v stands for angular velocity 
    ///@param a stands for acceleration
    public void Move(float v, float a)
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed * a;
        GetComponent<Rigidbody2D>().angularVelocity = angular_drag * v;
    }

    ///Abstract function for assuring propper destrucyion of the Agent object
    ///@param fitness allows adjust fitness value of corresponding to this agent NNet according to the way the agent was destroyed
    public abstract void Die(int fitness);

    ///Abstract function for allowing agent to clone itself
    public abstract void Reproduce();

    ///Abstract function that stands for sensory input 
    public abstract float[] InputSensors();

    ///Abstract function for drawing line when followed by camera
    public abstract void drawLine();
}
