using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupField : InputField
{
    public float min, max;
    public bool is_integer;

    // Update is called once per frame
    new private void Start()
    {
        base.Start();
        if (is_integer)
        {
            onSubmit.AddListener(value => clampInt(Mathf.FloorToInt(min), Mathf.CeilToInt(max)));
        }
        else
        {
            onSubmit.AddListener(value => clampFloat(min, max));
        }
           
    }

    public void clampFloat(float min, float max)
    {
        if (float.TryParse(text, out float floatValue))
        {
            // Clamp the value within the specified range
            floatValue = Mathf.Clamp(floatValue, min, max);

            // Update the input field text with the clamped value
            text = floatValue.ToString();
        }
        else
        {
            text = min.ToString();
        }
    }

    public void clampInt(int min, int max)
    {
        if (float.TryParse(text, out float rawVal))
        {
            int intValue = Mathf.FloorToInt(rawVal);
            // Clamp the value within the specified range
            intValue = Mathf.Clamp(intValue, min, max);

            // Update the input field text with the clamped value
            text = intValue.ToString();
        }
        else
        {
            text = min.ToString();
        }
    }
}
