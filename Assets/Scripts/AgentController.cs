using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    [Header("Neural Network")]
    public NNet network;

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
    public GameObject succesor;
    protected EnvironmentController EC;

    [Header("Network Options")]
    public int[] layers;
    public float mutation_rate;

    [Header("Respawn point")]
    public Transform spawn_point;

    [Header("TESTING")]
    public bool is_test;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = size;
        spr.sprite = sprite;
        spr.color = color;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = spawn_point.position;
        }
    }
    void Update()
    {
        if (!is_test)
        {
            float[] output = network.FeedForward(InputSensors());
            Move(output[0], output[1] + 1.5f);
        }
    }

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
    public void GetLockedOn()
    {
        EC.UImgr.LockCamera(gameObject.tag, network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString(), this.gameObject);
    }

    public void Move(float v, float a)
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed * a;
        GetComponent<Rigidbody2D>().angularVelocity = angular_drag * v;
    }

    public abstract void Die(int fitness);

    public abstract void Reproduce();

    public abstract float[] InputSensors();
}
