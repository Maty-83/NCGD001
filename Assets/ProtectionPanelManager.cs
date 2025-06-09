using Assets.Core.Enums;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionPanelManager : MonoBehaviour
{
    [SerializeField] private ProtectionViewPanel protectionPanelPrefab;
    [SerializeField] private Transform panelContainer;

    private Dictionary<DamageType, ProtectionViewPanel> activePanels = new();

    public void AddOrUpdateProtection(DamageType type, Sprite icon, float duration, string description)
    {
        if (activePanels.TryGetValue(type, out var panel))
        {
            panel.ExtendDuration(duration);
        }
        else
        {
            var panelInstance = Instantiate(protectionPanelPrefab, panelContainer);
            panelInstance.Init(icon, duration, description);
            activePanels.Add(type, panelInstance);
        }
    }

    public void RemoveProtection(DamageType type)
    {
        if (activePanels.TryGetValue(type, out var panel))
        {
            Destroy(panel.gameObject);
            activePanels.Remove(type);
        }
    }
}
