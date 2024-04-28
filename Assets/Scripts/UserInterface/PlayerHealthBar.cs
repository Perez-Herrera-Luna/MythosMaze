using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }

    public void SetHealth(float health)
    {
        Debug.Log("set health");
        slider.value = health;
    }
}
