using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyController : MonoBehaviour
{
    public NNet network;

    [Header("gameObject Settings")]
    public float max_hunger = 10;
    public float cur_hunger;
    public float maturity = 20;
    private float cur_maturity;
    public float speed;
    public float sight_radius;
    public Vector2 size;
    public Color color;
    public Sprite sprite;
    public SpriteRenderer spr;
    public GameObject succesor;
    private EnvironmentController EC;
    /*[Header("Fitness")]
    public float overallFitness;*/
    /* public float distanceMultipler = 1.4f;*/
    /*public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;*/

    [Header("Network Options")]
    public int[] layers;
    public float mutation_rate;
    //  private Vector3 lastPosition;
    /* private float totalDistanceTravelled;
     private float avgSpeed;*/

    //  private float aSensor, bSensor, cSensor;

    /* private void Awake()
     {
         network = GetComponent<NNet>();
         //TEST CODE
         //network.Initialise(LAYERS, NEURONS);
         network = new NNet(layers);
     }*/
    [Header("Respawn point")]
    public Transform spawn_point;


    /*public void Reset()
    {

        //TEST CODE
        network.Initialise(LAYERS, NEURONS);

        cur_hunger = 0f;
        cur_maturity = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }*/

    /*private void OnCollisionEnter2D(Collision2D collision)
      {
          Die();
      }*/

    private void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        EC = FindObjectOfType<EnvironmentController>();
        spawn_point = EC.prey_pos;
        transform.localScale = size;
        spr.sprite = sprite;
        spr.color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = spawn_point.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Predator"))
        {
            GetEaten(collision.collider.GetComponent<PredatorController>());
        }
    }

    private void GetEaten(PredatorController predator)
    {
        predator.cur_hunger = 0f;
        predator.network.AddFitness(1f);
        Die(-5);
    }

    private void Update()
    {
        float[] output = network.FeedForward(InputSensors());
        Move(output[0]/*, output[1]*/);
    }

    private void FixedUpdate()
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

        if(EC.camera_pos_ref == this.gameObject)
        {
            EC.UpdateInfo("Prey", network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString());
        }
    }


    public void Reproduce()
    {
        GameObject obj = Instantiate(succesor, transform.position, transform.rotation);
        obj.GetComponent<PreyController>().network = new NNet(this.network);
        obj.GetComponent<PreyController>().network.SetFitness(0);
        if(UnityEngine.Random.Range(0f, 1f) <= mutation_rate)
        {
            obj.GetComponent<PreyController>().network.Mutate();
        }
        EC.ReproducePrey(this, obj.GetComponent<PreyController>());
        EC.prey_count++;
        obj.GetComponent<PreyController>().spawn_point = EC.prey_pos;
      //  FindObjectOfType<EnvironmentController>().AddNNet(new NNet(obj.GetComponent<PreyController>().network));
    }

    
    private float[] InputSensors()
    {
        Collider2D[] hit_food = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Food"));
        Collider2D[] hit_predator = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Predator"));
        List<float> input_arr = new List<float>();
        Collider2D min = null;
        for (int i = 0; i < hit_food.Length; i++)
        {
          //  Debug.DrawLine(transform.position, hit_food[i].transform.position);
            if(min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_food[i].transform.position))
            {
                min = hit_food[i];
            }
        }
        if (min == null)
        {
            /*input_arr.Add(UnityEngine.Random.Range(-1f, 1f));
            input_arr.Add(UnityEngine.Random.Range(-1f, 1f));*/
          //  input_arr.Add(UnityEngine.Random.Range(-1f, 1f));
            /* input_arr.Add(0f);*/
             input_arr.Add(0f);
             input_arr.Add(0f);
        }
        else
        {
            /*input_arr.Add(min.transform.position.x / sight_radius);
            input_arr.Add(min.transform.position.y / sight_radius);*/
            // input_arr.Add(min.transform.position.x / 100f);
            Vector2 vec = min.transform.position - transform.position;
            float angle = Vector2.Angle(transform.up, vec);
            if (angle >= 180f)
            {
                angle = -1f * (360f - angle);
            }
            input_arr.Add(angle / 180f);
            input_arr.Add(Vector3.Distance(transform.position, min.transform.position) / sight_radius);
        }
        min = null;
        for (int i = 0; i < hit_predator.Length; i++)
        {
           // Debug.DrawLine(transform.position, hit_predator[i].transform.position);
            if (min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_predator[i].transform.position))
            {
                min = hit_predator[i];
            }
        }
        if (min == null)
        {
            /*input_arr.Add(UnityEngine.Random.Range(-1f,1f));
            input_arr.Add(UnityEngine.Random.Range(-1f, 1f));*/
            //input_arr.Add(UnityEngine.Random.Range(-1f, 1f));
            /* input_arr.Add(0f);*/
             input_arr.Add(0f);
             input_arr.Add(0f);
        }
        else
        {
            /* input_arr.Add(min.transform.position.x / sight_radius);
             input_arr.Add(min.transform.position.y / sight_radius);*/

            //  input_arr.Add(min.transform.position.x / 100f);
            Vector2 vec = min.transform.position - transform.position;
            float angle = Vector2.Angle(transform.up, vec);
            if(angle >= 180f)
            {
                angle = -1f * (360f - angle);
            }
            input_arr.Add(angle / 180f);
            input_arr.Add(Vector3.Distance(transform.position, min.transform.position) / sight_radius);
        }

        return input_arr.ToArray();

    }
    public void Move(float v/*, float h*/)
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed /** v*/;
        GetComponent<Rigidbody2D>().angularVelocity = 500f * v;
    }

    public void GetLockedOn()
    {
        EC.LockCamera("Prey", network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString(),  this.gameObject);
    }
    public void Die(int fitness)
    {
        this.network.AddFitness(fitness);
        EC.AddPrey(this.network);
        if (EC.camera_pos_ref == this.gameObject)
        {
            EC.ReleaseCamera();
        }
        Destroy(gameObject);
    }
}
