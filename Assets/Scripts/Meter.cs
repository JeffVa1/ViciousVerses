
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Meter
{
    // FIELDS
    private float hpBarValue;
    private float maxHpBarValue;
    private float minHpBarValue;
    private Image healthBar;

    private float audienceBarValue;
    private float maxAudienceBarValue;
    private float minAudienceBarValue;
    private Image audienceBar;



    // CONSTRUCTER
    public Meter(float hpBarValue,
                 float maxHpBarValue,
                 float minHpBarValue, 
                 Image healthBar,
                 float audienceBarValue,
                 float maxAudienceBarValue,
                 float minAudienceBarValue,
                 Image audienceBar)
    {
        this.hpBarValue = hpBarValue;
        this.maxHpBarValue = maxHpBarValue;
        this.minHpBarValue = minHpBarValue;
        this.healthBar = healthBar;
        this.audienceBarValue = audienceBarValue;
        this.maxAudienceBarValue = maxAudienceBarValue;
        this.minAudienceBarValue = minAudienceBarValue;
        this.audienceBar = audienceBar;
    }

    // GETTERS AND SETTERS
    public float GetHpBarValue()
    {
        return hpBarValue;
    }

    public void SetHpBarValue(float value)
    {
        this.hpBarValue = value;
        healthBar.fillAmount = GetHpBarValue() / maxHpBarValue;
    }

    public float GetMaxHpBarValue()
    {
        return maxHpBarValue;
    }

    public void SetMaxHpBarValue(float value)
    {
        this.maxHpBarValue = value;
    }

    public float GetAudienceBarValue()
    {
        return audienceBarValue;
    }

    public void SetAudienceBarValue(float value)
    {
        this.audienceBarValue = value;
        audienceBar.fillAmount = GetAudienceBarValue() / maxAudienceBarValue;
    }

    public float GetMaxAudienceBarValue()
    {
        return maxAudienceBarValue;
    }

    public void SetMaxAudienceBarValue(float value)
    {
        this.maxAudienceBarValue = value;
    }


    // METHODS

    public void AddToBar(string barName, float value)
    {
        float heal = value;
        // always positive for the math
        if (heal < 0)
        {
            heal = value * -1;
        }

        switch (barName)
        {
            case "hp":
                if (GetHpBarValue() + heal > maxHpBarValue)
                {
                    SetHpBarValue(maxHpBarValue);
                    healthBar.fillAmount = maxHpBarValue / maxHpBarValue;
                }
                else
                {
                    SetHpBarValue(GetHpBarValue() + value);
                    healthBar.fillAmount = GetHpBarValue() / maxHpBarValue;
                }
                break;

            case "audience":
                if (GetAudienceBarValue() + heal > maxAudienceBarValue)
                {
                    SetAudienceBarValue(maxAudienceBarValue);
                    audienceBar.fillAmount = maxAudienceBarValue / maxAudienceBarValue;
                }
                else
                {
                    SetAudienceBarValue(GetAudienceBarValue() + heal);
                    audienceBar.fillAmount = GetAudienceBarValue() / maxAudienceBarValue;
                }
                break;
        }

    }

    public void TakeFromBar(string barName, float value)
    {
        float damage = value;
        // always positive for the math
        if (damage < 0)
        {
            damage = value * -1;
        }

        switch (barName)
        {
            case "hp":
                if (GetHpBarValue() - damage < minHpBarValue)
                {
                    SetHpBarValue(minHpBarValue);
                    healthBar.fillAmount = minHpBarValue / maxHpBarValue;
                }
                else
                {
                    SetHpBarValue(GetHpBarValue() - damage);
                    healthBar.fillAmount = GetHpBarValue() / maxHpBarValue;
                }
                break;

            case "audience":
                if (GetAudienceBarValue() - damage < minAudienceBarValue)
                {
                    SetAudienceBarValue(minAudienceBarValue);
                    audienceBar.fillAmount = minAudienceBarValue / maxAudienceBarValue;
                }
                else
                {
                    SetAudienceBarValue(GetAudienceBarValue() - damage);
                    audienceBar.fillAmount = GetAudienceBarValue() / maxAudienceBarValue;
                }
                break;
        }




    }
}
