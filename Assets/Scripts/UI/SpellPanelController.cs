using Assets.Scripts.Objects.ScriptableObjects;
using TMPro;
using UnityEngine;


public class SpellPanelController : MonoBehaviour
{
    [SerializeField] GameObject Preview;
    [SerializeField] TMP_Text DescriptionText;
    [SerializeField] TMP_Text PriceText;
    [SerializeField] TMP_Text BoughtText;
    [SerializeField] TMP_Text BindingText;
    [SerializeField] GameObject BuyButton;

    private PlayerController playerController;
    private Weapon weapon;
    private bool isBought = false;

    public void Initialize(PlayerController controll, bool isInitialized, Weapon weapon)
    {
        playerController = controll;
        this.weapon = weapon;

        if (isInitialized)
        {
            ChangeToInitialized();
        }

        if (weapon.PreviewImage != null)
        {
            Preview.GetComponent<UnityEngine.UI.Image>().sprite = weapon.PreviewImage;
        }

        DescriptionText.text = weapon.Decription;
        PriceText.text = $"Price: {weapon.BuyCost.ToString()}";
        BindingText.text = $"Bind: {weapon.Binding.ToString()}";
    }

    public void Buy()
    {
        if (playerController == null)
            return;

        if (playerController.Score < weapon.BuyCost)
            return;

        playerController.Score -= weapon.BuyCost;
        playerController.Weapons.Add(weapon.Binding, weapon);
        ChangeToInitialized();
    }

    private void ChangeToInitialized()
    {
        isBought = true;
        BuyButton.SetActive(false);
        BoughtText.gameObject.SetActive(true);
    }
}
