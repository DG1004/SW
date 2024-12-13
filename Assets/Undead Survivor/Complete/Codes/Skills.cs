using Goldmetal.UndeadSurvivor;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Skills : MonoBehaviour
{
    public GameObject player;
    public Button ghostSkillButton;
    public Button healSkillButton;
    public Button enhanceSkillButton;
    public Image ghostSkillLockImage;
    public Image healSkillLockImage;
    public Image enhanceSkillLockImage;
    public int skillCost = 50000; // Cost for each skill

    private Player playerScript;

    void Awake()
    {
        playerScript = player.GetComponent<Player>();

        // Add button listeners
        ghostSkillButton.onClick.AddListener(() => PurchaseSkill(SkillType.Ghost));
        healSkillButton.onClick.AddListener(() => PurchaseSkill(SkillType.Heal));
        enhanceSkillButton.onClick.AddListener(() => PurchaseSkill(SkillType.Enhance));

        // Ensure buttons are disabled if skills are already unlocked
        if (playerScript.canGhost)
        {
            ghostSkillButton.interactable = false;
            ghostSkillLockImage.gameObject.SetActive(false);
        }
        if (playerScript.canHeal)
        {
            healSkillButton.interactable = false;
            healSkillLockImage.gameObject.SetActive(false);
        }
        if (playerScript.canEnhence)
        {
            enhanceSkillButton.interactable = false;
            enhanceSkillLockImage.gameObject.SetActive(false);
        }
    }

    public enum SkillType
    {
        Ghost,
        Heal,
        Enhance
    }

    public void PurchaseSkill(SkillType skillType)
    {
        if (CoinManager.playerCoins >= skillCost)
        {
            CoinManager.playerCoins -= skillCost; // Deduct skill cost

            switch (skillType)
            {
                case SkillType.Ghost:
                    UnlockGhostSkill();
                    break;

                case SkillType.Heal:
                    UnlockHealSkill();
                    break;

                case SkillType.Enhance:
                    UnlockEnhanceSkill();
                    break;
            }
        }
        else
        {
            Debug.Log("Not enough coins to purchase skill!");
        }
    }

    private void UnlockGhostSkill()
    {
        playerScript.canGhost = true; // Enable ghost skill on the player
        ghostSkillLockImage.gameObject.SetActive(false); // Remove lock image
        ghostSkillButton.interactable = false; // Disable button to prevent re-purchase
        Debug.Log("Ghost skill unlocked!");
    }

    private void UnlockHealSkill()
    {
        playerScript.canHeal = true; // Enable heal skill on the player
        healSkillLockImage.gameObject.SetActive(false); // Remove lock image
        healSkillButton.interactable = false; // Disable button to prevent re-purchase
        Debug.Log("Heal skill unlocked!");
    }

    private void UnlockEnhanceSkill()
    {
        playerScript.canEnhence = true; // Enable enhance skill on the player
        enhanceSkillLockImage.gameObject.SetActive(false); // Remove lock image
        enhanceSkillButton.interactable = false; // Disable button to prevent re-purchase
        Debug.Log("Enhance skill unlocked!");
    }
}
