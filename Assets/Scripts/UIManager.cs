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
    public TextMeshProUGUI type_text, speed_text, fit_text, maturity_text, sight_text;
    public Camera main_camera;
    public float camera_zoom;
    private float def_zoom;
    public GameObject camera_pos_ref;
    public int gen_count;
    public Slider NMC, TMS, TMR;
    [Header("Setup UI reference")]
    public GameObject setupUI;
    /// Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        setCameraFocus();
        setupUI.SetActive(false);
        gen_count = 0;
        camera_pos_ref = this.gameObject;
        def_zoom = main_camera.orthographicSize;
        info_panel.SetActive(false);
    }

    ///Adjust Camera view field according to map size
    void setCameraFocus()
    {
        main_camera.orthographicSize = Mathf.Max(EC.config.room.x, EC.config.room.y)/2.0f;
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
    public void UpdateInfo(string type, string fitness, string speed, string maturity, string sight)
    {
        type_text.text = type;
        fit_text.text = fitness;
        speed_text.text = speed;
        maturity_text.text = maturity;
        sight_text.text = sight;
    }

    ///Locks the camera on the object displaying info about it
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

    ///Cancels locck on 
    public void ReleaseCamera()
    {
        info_panel.SetActive(false);
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
