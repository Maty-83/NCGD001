using Assets.Scripts;
using Assets.Scripts.UI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpellPanel : MonoBehaviour
{
    [SerializeField] MagicPanelController controller;
    [SerializeField] PlayerController playerController;
    [SerializeField] Image SkillTreeImage;
    [SerializeField] Color skillTreeColorAvalible;
    [SerializeField] Color skillTreeColorUnAvalible;
    [SerializeField] List<CardController> Cards;
 

    public void OnSkillTreeClick()
    {
        if(!controller.gameObject.activeSelf)
        {
            controller.gameObject.SetActive(true);
            controller.Display(false);
        }
    }

    public void UpdatePanel(KeyCode[] avalibleKeys)
    { 
        KeyCode[] sortedKeys = avalibleKeys.OrderBy(k => k.ToString()).ToArray();
        var index = 0;
        foreach (var key in avalibleKeys) 
        {
            if(playerController.BindedWeapons.ContainsKey(key))
            {
                var weapon = playerController.BindedWeapons[key];
                if (index < Cards.Count)
                {
                    Cards[index].Set(weapon.PreviewImage, key.ToString());
                }
                index++;
            }
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnSkillTreeClick();
        }

        foreach (var weapon in GameManager.Instance.GetAvalibleWeapons())
        {
            if (weapon.BuyCost > playerController.Score)
            {
                SkillTreeImage.color = skillTreeColorUnAvalible;
                return;
            }

            if (weapon.BuyCost <= playerController.Score)
            {
                SkillTreeImage.color = skillTreeColorAvalible;
                return;
            }
        }
    }
}
