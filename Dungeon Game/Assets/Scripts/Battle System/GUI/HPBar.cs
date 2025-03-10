using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider hpSlider; 

    void Start()
    {
        hpSlider = GetComponent<Slider>();    
    }

    public void InitializeHPBar(float currentHP, float maxHealth)
    {
        UpdateHPBar(currentHP, maxHealth);
    }

    public void TakeDamage(float currentHP, float maxHP, float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); 
        UpdateHPBar(currentHP, maxHP);
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHP / maxHP; 
        }
    }
}
