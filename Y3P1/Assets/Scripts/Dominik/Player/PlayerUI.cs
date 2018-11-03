using UnityEngine;
using TMPro;
using Y3P1;

public class PlayerUI : MonoBehaviour 
{

    private bool isInitialised;
    private Player target;
    public Player Target { get { return target; } }

    public enum UIType { WorldSpace, ScreenSpace}
    public UIType uiType;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI itemLevelText;

    private void Update()
    {
        if (!target && isInitialised)
        {
            Destroy(gameObject);
        }
    }

    public void Initialise(Player target, bool local, Entity entity)
    {
        if (!target)
        {
            return;
        }

        if (uiType == UIType.ScreenSpace)
        {
            UIManager.instance.playerScreenSpaceUIs.Add(this);
        }

        this.target = target;
        if (nameText)
        {
            nameText.text = local ? "" : target.photonView.Owner.NickName;
        }

        isInitialised = true;
    }

    public void UpdateHealthText(int currentHealth, int maxHealth, int itemLevel)
    {
        healthText.text = currentHealth + "/" + maxHealth;
        itemLevelText.text = "ILvl " + itemLevel;

        if (healthBar)
        {
            healthBar.SetCustomValues(new Health.HealthData { percentageHealth = (float)currentHealth / maxHealth });
        }
    }

    private void OnDestroy()
    {
        if (UIManager.instance.playerScreenSpaceUIs.Contains(this))
        {
            UIManager.instance.playerScreenSpaceUIs.Remove(this);
        }
    }
}
