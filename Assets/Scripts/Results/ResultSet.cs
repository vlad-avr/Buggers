using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultSet : MonoBehaviour
{
    //Results panel object
    public GameObject results_panel;

    //Values to display (updated by Environment Controller)
    public int final_prey_count = 0;
    public int final_pred_count = 0;
    public float[] prey_fitness_params = new float[3];
    public float[] pred_fitness_params = new float[3];
    public int time_elapsed = 0;

    //UI references
    private List<TextMeshProUGUI> prey_results = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> pred_results = new List<TextMeshProUGUI>();
    private TextMeshProUGUI time;

    public void SetRefs()
    {
        Transform prey_transform = results_panel.transform.Find("PREY");
        prey_results.Add(prey_transform.Find("COUNT").GetComponent<TextMeshProUGUI>());
        prey_results.Add(prey_transform.Find("MIN_FIT").GetComponent<TextMeshProUGUI>());
        prey_results.Add(prey_transform.Find("AVERAGE_FIT").GetComponent<TextMeshProUGUI>());
        prey_results.Add(prey_transform.Find("MAX_FIT").GetComponent<TextMeshProUGUI>());
        Transform pred_transform = results_panel.transform.Find("PREDATOR");
        pred_results.Add(pred_transform.Find("COUNT").GetComponent<TextMeshProUGUI>());
        pred_results.Add(pred_transform.Find("MIN_FIT").GetComponent<TextMeshProUGUI>());
        pred_results.Add(pred_transform.Find("AVERAGE_FIT").GetComponent<TextMeshProUGUI>());
        pred_results.Add(pred_transform.Find("MAX_FIT").GetComponent<TextMeshProUGUI>());
        time = results_panel.transform.Find("TIME").Find("TIME_SIM").GetComponent<TextMeshProUGUI>();
    }

    public void FillOut()
    {
        prey_results[0].SetText(final_prey_count.ToString());
        for(int i = 0; i < prey_fitness_params.Length; i++)
        {
            prey_results[i + 1].SetText(prey_fitness_params[i].ToString());
        }
        pred_results[0].SetText(final_pred_count.ToString());
        for (int i = 0; i < pred_fitness_params.Length; i++)
        {
            pred_results[i + 1].SetText(pred_fitness_params[i].ToString());
        }
        time.SetText(time_elapsed.ToString());
    }
}
