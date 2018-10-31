using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{

    private Dungeon myDungeon;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI dungeonNameText;
    [SerializeField] private TextMeshProUGUI dungeonDescText;

    [Header("Buttons")]
    [SerializeField] private Button startDungeonButton;
    [SerializeField] private Button enterDungeonButton;
    [SerializeField] private Button cancelDungeonButton;

    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;

    public void Setup(Dungeon dungeon)
    {
        myDungeon = dungeon;

        dungeonNameText.text = dungeon.dungeonName;
        dungeonDescText.text = dungeon.dungeonDescription;

        SetupButtonListeners();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            startDungeonButton.transform.parent.gameObject.SetActive(DungeonManager.openDungeon != myDungeon);
            enterDungeonButton.transform.parent.gameObject.SetActive(DungeonManager.openDungeon == myDungeon);

            optionsPanel.SetActive(DungeonManager.openDungeon == myDungeon);
        }
    }

    private void SetupButtonListeners()
    {
        startDungeonButton.onClick.AddListener(() => DungeonManager.instance.ActivateDungeon(myDungeon.dungeonName));
        enterDungeonButton.onClick.AddListener(() => DungeonManager.instance.TeleportToDungeon(myDungeon.dungeonName));
        cancelDungeonButton.onClick.AddListener(() => DungeonManager.instance.CancelDungeon(myDungeon.dungeonName));
    }
}
