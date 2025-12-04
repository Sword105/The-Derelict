using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image healthBarFill;

    void Update()
    {
        if (PlayerHPManager.instance != null)
        {
            float fillValue = PlayerHPManager.instance.currentHP / PlayerHPManager.instance.maxHP;
            healthBarFill.fillAmount = Mathf.Clamp01(fillValue);
        }
    }
}
