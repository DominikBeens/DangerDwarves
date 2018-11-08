using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{

    private float duration;
    private bool updateDescPanel;

    public StatusEffects.StatusEffectType type;

    [SerializeField] private Image durationFill;
    [TextArea] [SerializeField] private string description;

    public void Activate(float? duration = null)
    {
        gameObject.SetActive(true);

        if (durationFill)
        {
            durationFill.fillAmount = 1f;
            this.duration = duration != null ? (float)duration : 0;
        }
    }

    public void ToggleDesc(bool toggle)
    {
        if (!string.IsNullOrEmpty(description))
        {
            updateDescPanel = toggle;
            UIManager.instance.playerStatusCanvas.ToggleBuffDescPanel(toggle, description + "\nRemaining: <color=red>" + (durationFill.fillAmount * duration).ToString("F1"));
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && durationFill)
        {
            durationFill.fillAmount -= 1f / duration * Time.deltaTime;
        }

        if (updateDescPanel)
        {
            UIManager.instance.playerStatusCanvas.ToggleBuffDescPanel(true, description + "\nRemaining: <color=red>" + (durationFill.fillAmount * duration).ToString("F1"));
        }
    }

    private void OnDisable()
    {
        ToggleDesc(false);
    }
}
