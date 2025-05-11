using Assets.Scripts;
using Assets.Scripts.Objects.ScriptableObjects;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MagicPanelController : MonoBehaviour
{
    [SerializeField] GameObject SpellPanelPrefab;
    [SerializeField] GameObject Parent;
    [SerializeField] TMP_Text ScoreText;

    private PlayerController playerController;
    public void Display()
    {
        playerController = GameManager.Instance.PlayerController;
        if (playerController == null)
            return;

        var avalibleWeapons = new List<Weapon>();
        foreach (var weapon in GameManager.Instance.damagers)
        {
            if (playerController.Weapons.Values.Contains(weapon))
            {
                continue;
            }
            avalibleWeapons.Add(weapon);
        }

        ScoreText.text = $"XP: {playerController.Score.ToString()}";
        Initialize(avalibleWeapons, playerController.Weapons.Values.ToList());
    }

    private void Initialize(List<Weapon> avalibleWeapons, List<Weapon> owned)
    {
        foreach(var weapon in avalibleWeapons)
        {
            var panel = Instantiate(SpellPanelPrefab, Parent.transform).GetComponent<SpellPanelController>();
            panel.Initialize(playerController, false, weapon);
        }

        foreach (var weapon in owned)
        {
            var panel = Instantiate(SpellPanelPrefab, Parent.transform).GetComponent<SpellPanelController>();
            panel.Initialize(playerController, false, weapon);
        }
    }
}
