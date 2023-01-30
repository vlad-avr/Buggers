using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorController : AgentController
{
 /*   public NNet network;

    [Header("gameObject Settings")]
    public float max_hunger = 10;
    public float cur_hunger;
    public float maturity = 20;
    private float cur_maturity;
    public float speed;
    public float angular_drag;
    public float sight_radius;
    public Vector2 size;
    public Color color;
    public Sprite sprite;
    public SpriteRenderer spr;
    public GameObject succesor;
    private EnvironmentController EC;
    [Header("Network Options")]
    public int[] layers;
    public float mutation_rate;
   */
    /* [Header("Respawn point")]
     public Transform spawn_point;

     [Header("TESTING")]
     public bool is_test;
     */
    /* private void Start()
     {
         if (!is_test)
         {
             GetComponentInChildren<Canvas>().worldCamera = Camera.main;
             EC = FindObjectOfType<EnvironmentController>();
             spawn_point = EC.predator_pos;
         }
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
     }*/

    /*   private void Update()
       {
           if (!is_test)
           {
               float[] output = network.FeedForward(InputSensors());
               Move(output[0]);
           }
       }

       private void FixedUpdate()
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

               if (EC.camera_pos_ref == this.gameObject)
               {
                   EC.UpdateInfo("Predator", network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString());
               }
           }
       }
    */
    void Awake()
    {
        if (!is_test)
        {
            GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            EC = FindObjectOfType<EnvironmentController>();
            spawn_point = EC.predator_pos;
        }
    }
    public override void Reproduce()
    {
        GameObject obj = Instantiate(succesor, transform.position, transform.rotation);
        obj.GetComponent<PredatorController>().network = new NNet(this.network);
        obj.GetComponent<PredatorController>().network.SetFitness(0);
        if (UnityEngine.Random.Range(0f, 1f) <= mutation_rate)
        {
            obj.GetComponent<PredatorController>().network.Mutate();
        }
        EC.ReproducePredator(this, obj.GetComponent<PredatorController>());
        EC.predator_count++;
        obj.GetComponent<PredatorController>().spawn_point = EC.predator_pos;
    }


    public override float[] InputSensors()
    {
        Collider2D[] hit_predator = Physics2D.OverlapCircleAll(transform.position, sight_radius, LayerMask.GetMask("Prey"));
        List<float> input_arr = new List<float>();
        Collider2D min = null;
        for (int i = 0; i < hit_predator.Length; i++)
        {
            if (min == null || Vector2.Distance(transform.position, min.transform.position) > Vector2.Distance(transform.position, hit_predator[i].transform.position))
            {
                min = hit_predator[i];
            }
        }
        if (min == null)
        {
            input_arr.Add(UnityEngine.Random.Range(-180f, 180f) / 180f);
            input_arr.Add(UnityEngine.Random.Range(0f, sight_radius) / sight_radius);
        }
        else
        {
            Vector2 vec = min.transform.position - transform.position;
            float angle = Vector2.Angle(transform.up, vec);
            if (angle >= 180f)
            {
                angle = -1f * (360f - angle);
            }
            input_arr.Add(angle / 180f);
            input_arr.Add(Vector3.Distance(transform.position, min.transform.position) / sight_radius);
        }
       
        return input_arr.ToArray();

    }
   /* public void Move(float v)
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed ;
        GetComponent<Rigidbody2D>().angularVelocity = angular_drag * v;
    }
    */
   /* public void GetLockedOn()
    {
        EC.LockCamera("Predator", network.GetFitness().ToString(), speed.ToString(), maturity.ToString(), sight_radius.ToString(), this.gameObject);
    }
    */
    public override void Die(int fitness)
    {
        this.network.AddFitness(fitness);
        EC.AddPredator(network);
        if (EC.camera_pos_ref == this.gameObject)
        {
            EC.ReleaseCamera();
        }
        Destroy(gameObject);
    }
}
