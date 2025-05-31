using Assets.Scripts;
using Assets.Scripts.Objects.ScriptableObjects;
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
    private List<SpellPanelController> panels = new List<SpellPanelController>();
    private bool wasDisplayedFromUI = true;
    public void Display(bool fromUI = true)
    {
        wasDisplayedFromUI = fromUI;
        if (!fromUI)
        {
            GameManager.Instance.Pause();
        }
        
        Clear();

        playerController = GameManager.Instance.PlayerController;
        if (playerController == null)
            return;

        ScoreText.text = $"XP: {playerController.Score.ToString()}";
        Initialize(GameManager.Instance.GetAvalibleWeapons(), playerController.OwnedWeapons.ToList());
    }

    private void Initialize(List<Weapon> avalibleWeapons, List<Weapon> owned)
    {
        foreach(var weapon in avalibleWeapons)
        {
            var panel = Instantiate(SpellPanelPrefab, Parent.transform).GetComponent<SpellPanelController>();
            panel.Initialize(playerController, false, weapon);
            panels.Add(panel);
        }

        foreach (var weapon in owned)
        {
            var panel = Instantiate(SpellPanelPrefab, Parent.transform).GetComponent<SpellPanelController>();
            panel.Initialize(playerController, true, weapon);
            panels.Add(panel);
        }
    }

    private void Clear()
    {
        foreach(var panel in panels)
            Destroy(panel.gameObject);

        panels.Clear(); 
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Alpha0)) && !wasDisplayedFromUI)
        {
            gameObject.SetActive(false);
            GameManager.Instance.Resume();
        }
    }
}
