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
    public float max_hunger = 10;
    public float cur_hunger;
    public float maturity = 20;
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
    ///Reference to EnvironmentController script in the scene
    protected EnvironmentController EC;

    ///NNet script settings
    [Header("Network Options")]
    public int[] layers;
    public float mutation_rate;
    ///Spawn point in the scene reference
    [Header("Respawn point")]
    public Transform spawn_point;
    ///Boolean varaible to switch on\off testing mode (dev tool)s
    [Header("TESTING")]
    public bool is_test;

    /// Start is called before the first frame update
    void Start()
    {
        transform.localScale = size;
        spr.sprite = sprite;
        spr.color = color;
    }

    ///Detects if Agent has entered a Trigger collider
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = spawn_point.position;
        }
    }

    /// Update is called every frame
    void Update()
    {
        if (!is_test)
        {
            float[] output = network.FeedForward(InputSensors());
            Move(output[0], output[1] + 1f);
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

            if (cur_hunger >= max_hunger)
            {
                Die(-10);
            }
            else
            {
                cur_hunger += Time.deltaTime;
            }


            if (EC.UImgr.camera_pos_ref == this.gameObject)
            {
                EC.UImgr.UpdateInfo(gameObject.tag, network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString());
            }
        }
    }

    ///Passes Agent data to the UI elements and resets Camera position
    public void GetLockedOn()
    {
        EC.UImgr.LockCamera(gameObject.tag, network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString(), this.gameObject);
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
}
