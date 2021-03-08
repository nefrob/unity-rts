using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImg = null;

    private void Awake()
    {
        health.ClientOnHealthUpdate += HandleHealthUpdate;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdate -= HandleHealthUpdate;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    private void HandleHealthUpdate(float curHealth, float maxHealth)
    {
        healthBarImg.fillAmount = curHealth / maxHealth;
    }
}
