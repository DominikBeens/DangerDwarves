﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;
    private Animator anim;

    [SerializeField] private Image foregroundHealthBar;
    [SerializeField] private Image backgroundHealthBar;
    [SerializeField] private float backgroundLerpTime = 1;
    [SerializeField] private bool showDamageText = true;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (!initialised)
        {
            Initialise(transform.root.GetComponentInChildren<Entity>());
        }
    }

    public void Initialise(Entity entity)
    {
        if (entity)
        {
            myEntity = entity;

            myEntity.health.OnHealthModified += Health_OnHealthModified;
            initialised = true;
        }
    }

    private void Health_OnHealthModified(Health.HealthData healthData)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // Check if health got decreased or added and play the according animation.
        // Optional TODO: Add 'IncreaseHealth' animation, it just plays the decrease animation now.
        if (anim)
        {
            anim.SetTrigger(foregroundHealthBar.fillAmount > healthData.percentageHealth ? "DecreaseHealth" : "DecreaseHealth");
        }

        foregroundHealthBar.fillAmount = healthData.percentageHealth;
        StartCoroutine(LerpBackgroundHealthBar(healthData.percentageHealth));

        if (healthText)
        {
            healthText.text = healthData.currentHealth + "/" + healthData.maxHealth;
        }

        if (healthData.amountHealthChanged != null && showDamageText)
        {
            DamageText newDamageText = ObjectPooler.instance.GrabFromPool("DamageText", transform.position, transform.rotation).GetComponent<DamageText>();
            if (healthData.isInvinsible && healthData.amountHealthChanged <= 0)
            {
                newDamageText.Initialise("Dodge");
            }
            else
            {
                newDamageText.Initialise(healthData);
            }
        }
    }

    // Used for manually setting healthbar values for when this object is not initialised but used somewhere else as UI.
    public void SetCustomValues(Health.HealthData healthData)
    {
        if (anim)
        {
            anim.SetTrigger(foregroundHealthBar.fillAmount > healthData.percentageHealth ? "DecreaseHealth" : "DecreaseHealth");
        }

        foregroundHealthBar.fillAmount = healthData.percentageHealth;
        StartCoroutine(LerpBackgroundHealthBar(healthData.percentageHealth));
    }

    private IEnumerator LerpBackgroundHealthBar(float percentage)
    {
        float startPercentage = backgroundHealthBar.fillAmount;
        float progress = 0f;

        while (progress < backgroundLerpTime)
        {
            progress += Time.deltaTime;
            backgroundHealthBar.fillAmount = Mathf.Lerp(startPercentage, percentage, progress / backgroundLerpTime);
            yield return null;
        }

        backgroundHealthBar.fillAmount = percentage;
    }

    private void OnDisable()
    {
        if (initialised)
        {
            myEntity.health.OnHealthModified -= Health_OnHealthModified;
        }
    }
}
