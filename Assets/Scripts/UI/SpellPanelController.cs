using Assets.Scripts;
using Assets.Scripts.Objects.ScriptableObjects;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpellPanelController : MonoBehaviour
{
    [SerializeField] GameObject Preview;
    [SerializeField] TMP_Text DescriptionText;
    [SerializeField] TMP_Text PriceText;
    [SerializeField] TMP_Text BoughtText;
    [SerializeField] TMP_Text BindingText;
    [SerializeField] GameObject BuyButton;
    [SerializeField] GameObject BindingPanel;

    private bool IsBinding = false;
    public KeyCode binding = KeyCode.None;
    private PlayerController playerController;
    private Weapon weapon;
    private bool isBought = false;

    public KeyCode[] keysToCheck = { 
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7};

    public void Initialize(PlayerController controll, bool isBought, Weapon weapon)
    {
        playerController = controll;
        this.weapon = weapon;

        if (isBought)
        {
            ChangeToInitialized();
        }

        if (weapon.PreviewImage != null)
        {
            Preview.GetComponent<UnityEngine.UI.Image>().sprite = weapon.PreviewImage;
        }

        DescriptionText.text = weapon.Decription;
        PriceText.text = $"Price: {weapon.BuyCost.ToString()}";

        if(weapon.Binding != KeyCode.None)
            BindingText.text = $"Bind: {weapon.Binding}";
        BindingText.text = $"Bind: Empty";
    }

    public void Buy()
    {
        if (playerController == null)
            return;

        if (playerController.Score < weapon.BuyCost)
            return;

        playerController.Score -= weapon.BuyCost;
        playerController.OwnedWeapons.Add(weapon);
        ChangeToInitialized();
        
        foreach(var key in keysToCheck)
        {
            if(playerController.BindedWeapons.ContainsKey(key))
                continue;

            BindKeyToSpell(key);
            BindingText.text = $"Bind: {weapon.Binding}";
            return;
        }
    }

    public void Bind()
    {
        if(!IsBinding)
        {
            IsBinding = true;
            BindingPanel.SetActive(true);
        }
    }

    private void ChangeToInitialized()
    {
        isBought = true;
        BuyButton.SetActive(false);
        BoughtText.gameObject.SetActive(true);
    }


    private void BindKeyToSpell(KeyCode key)
    {
        if (playerController.BindedWeapons.ContainsKey(key))
        {
            playerController.BindedWeapons[key].Binding = KeyCode.None;
            playerController.BindedWeapons[key] = weapon;
        }
        else
        {
            playerController.BindedWeapons.Add(key, weapon);
        }

        weapon.Binding = key;
        GameManager.Instance.SpellPanel.UpdatePanel(keysToCheck);
    }

    public void Update()
    {
        if (IsBinding)
        {
            foreach (KeyCode key in keysToCheck)
            {
                if (Input.GetKeyDown(key))
                {
                    BindingPanel.SetActive(false);
                    IsBinding = false;

                    BindKeyToSpell(key);
                    return;
                }
            }
        }
    }
}
