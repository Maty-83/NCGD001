using Assets.Core.Enums;
using Assets.Scripts.Entities;
using System;
using UnityEngine;

namespace Assets.Scripts.Objects.Spells
{
    public class ResistanceSpell : MonoBehaviour, IProtectiveSpell, IResistance
    {
        [SerializeField] private DamageType damageType;
        [SerializeField] private int Percent;
        [SerializeField] private float Duration;
        

        private Entity Caster = null;
        private bool casted = false;
        private float timeLeft = 0f;

        private ProtectionViewPanel protectionPanel;

        private void Update()
        {
            if (casted)
            {
                if (Caster != null)
                {
                    transform.position = Caster.transform.position;
                }

                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0f)
                {
                    AfterCast();
                }
            }
        }

        public void AfterCast()
        {
            var player = GameManager.Instance.PlayerController;
            if (player.Resistancies.ContainsKey(damageType))
            {
                player.Resistancies.Remove(damageType);
            }

            GameManager.Instance.ProtectionPanel.RemoveProtection(damageType);
            Destroy(gameObject);
        }

        public void Cast(Entity caster, ScriptableObjects.Weapon spell)
        {
            Caster = caster;

            var player = GameManager.Instance.PlayerController;
            if (player.Resistancies.TryGetValue(damageType, out var existingResistance))
            {
                existingResistance.SetDuration(existingResistance.GetDuration() + Duration);
                Destroy(gameObject);
            }
            else
            {
                player.Resistancies.Add(damageType, this);
                timeLeft = Duration;
                casted = true;
            }

            GameManager.Instance.ProtectionPanel.AddOrUpdateProtection(
                damageType,
                spell.PreviewImage,
                Duration,
                "Melee Protection"
            );
        }

        public float GetDuration()
        {
            return timeLeft;
        }

        public void SetDuration(float duration)
        {
            timeLeft = duration;
        }

        public int GetResistance()
        {
            return Percent;
        }
    }
}