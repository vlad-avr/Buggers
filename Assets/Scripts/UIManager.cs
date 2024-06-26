using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

///Class for UI manipulations
public class UIManager : MonoBehaviour
{
    [Header("Environment Controller")]
    public EnvironmentController EC;

    [Header("UI settings")]
    public TextMeshProUGUI cur_gen_text, population_text;
    public GameObject info_panel;
    public TextMeshProUGUI type_text, speed_text, fit_text, maturity_text, energy_text, accel_text, angle_text;
    public Camera main_camera;
    public float camera_zoom;
    private float def_zoom;
    public GameObject camera_pos_ref;
    public int gen_count;
    /// Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        setCameraFocus();
        gen_count = 0;
        camera_pos_ref = this.gameObject;
        def_zoom = main_camera.orthographicSize;
        info_panel.SetActive(false);
    }

    ///Adjust Camera view field according to map size
    void setCameraFocus()
    {
        if(EC.config.room.x / 1920f <= EC.config.room.y / 1080f)
        {
            main_camera.orthographicSize = EC.config.room.y / 2.0f;
        }
        else
        {
            main_camera.orthographicSize = EC.config.room.x / 4.0f;
        }
        //main_camera.orthographicSize = Mathf.Max(EC.config.room.x/1920f, EC.config.room.y/1080f)/2.0f;
    }

    /// Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReleaseCamera();
        }
    }

    ///Updates info about object that the camera is currently locked on
    public void UpdateInfo(AgentController agent)
    {
        type_text.text = agent.gameObject.tag;
        fit_text.text = agent.network.GetFitness().ToString();
        speed_text.text = agent.speed.ToString();
        maturity_text.text = agent.maturity.ToString();
        energy_text.text = agent.cur_energy.ToString();
        accel_text.text = agent.getOutput()[0].ToString();
        angle_text.text = (agent.getOutput()[1] + 1).ToString();
    }

    ///Locks the camera on the object displaying info about it
    public void LockCamera(AgentController agent)
    {
        main_camera.orthographicSize = def_zoom;
        camera_pos_ref = agent.gameObject;
        info_panel.SetActive(true);
        UpdateInfo(agent);
        main_camera.transform.position = new Vector3(camera_pos_ref.transform.position.x, camera_pos_ref.transform.position.y, -10);
        main_camera.orthographicSize *= camera_zoom;
        agent.locked_on = true;
    }

    ///Cancels lock on 
    public void ReleaseCamera()
    {
        info_panel.SetActive(false);
        if (camera_pos_ref.tag.Equals("Prey"))
        {
            camera_pos_ref.GetComponent<PreyController>().locked_on = false;
        }
        if (camera_pos_ref.tag.Equals("Predator"))
        {
            camera_pos_ref.GetComponent<PredatorController>().locked_on = false;
        }
        camera_pos_ref = this.gameObject;
        main_camera.orthographicSize = def_zoom;
    }


    ///Sends signal to Destroy the object that the Camera is locked on
    public void Kill()
    {
        if (camera_pos_ref.CompareTag("Prey"))
        {
            camera_pos_ref.GetComponent<PreyController>().Die(0);
        }
        else if (camera_pos_ref.CompareTag("Predator"))
        {
            camera_pos_ref.GetComponent<PredatorController>().Die(0);
        }
    }

    ///Increases the fitness of the NNet of the object that the Camera is locked on
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

    ///Updates info about the current generation of Agents
    public void UpdateUI(PreyController[] preys, PredatorController[] predators)
    {
        cur_gen_text.text = gen_count.ToString();
        population_text.text = EC.prey_count.ToString() + " / " + EC.predator_count.ToString();
        main_camera.transform.position = new Vector3(camera_pos_ref.transform.position.x, camera_pos_ref.transform.position.y, -10);
        //EC.mutation_rate = NMC.value;
        //EC.reproduction_mutation_prob = TMR.value;
        //EC.spawn_mutation_prob = TMS.value;
    }
}
