using Assets.Scripts.Entities.Behaviour;
using Assets.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class BasicEnemyController : Entity
    {
        [SerializeField] int HealthBarTimer = 10;
        [SerializeField] BarController barControll = null;
        [SerializeField] float ShootingRange = 5f;
        [SerializeField] float KillReward = 500f;
        [SerializeField] List<ResistanceObject> Resistances = null;
        [SerializeField] GameObject ResistantText;

        private IBehaviour behaviour;

        private int timer = 0;
        private float ResistantTimer = 0f;

        private new void Start()
        {
            base.Start();
            barControll.SetValues(HP, false, 0, HP);
            behaviour = gameObject.GetComponent<IBehaviour>();

            foreach (ResistanceObject obj in Resistances)
            {
                Resistancies.Add(obj.DamageType, obj);
            }
        }
        private new void Update()
        {
            if (!IsAlive) return;

            base.Update();

            if (barControll != null)
            {
                if (timer > 1)
                {
                    timer--;
                }
                else if (timer == 1)
                {
                    barControll.SetValues(HP, false);
                    timer = 0;
                }
            }
        }

        private new void FixedUpdate()
        {
            if (ResistantTimer < 0 && ResistantTimer > -0.5f)
                ResistantText.SetActive(false);
            else if(ResistantTimer  > 0)
                ResistantTimer -= Time.deltaTime;

            if (!IsAlive) return;

            var player = GameManager.Instance.PlayerController;
            var dir = -1 * ( gameObject.transform.position - player.transform.position);
            var distance = ( player.transform.position - transform.position ).magnitude;

            if (curTimeBetweenAttacks > minTimeBetweenMelee)
            {
                foreach (var melee in MeleeWeapons)
                {
                    if (distance < melee.Range)
                    {
                        OnMelee(melee, dir);
                        return;
                    }
                }
            }

            if (distance < ShootingRange && curTimeBetweenAttacks > minTimeBetweenBullets)
            {
                var weaponCount = RangeWeapons.Count;
                if (weaponCount == 0)
                    return;

                var randomChoose = Random.Range(0, weaponCount);
                OnShoot(RangeWeapons[randomChoose], dir);   
            }


            curTimeBetweenAttacks += Time.fixedDeltaTime;
        }

        public override void RecieveDamage(IDamager weapon)
        {
            if (Resistancies.ContainsKey(weapon.Type) && ResistantText != null)
            {
                ResistantText.SetActive(true);
                ResistantTimer = 0.3f;
            }

            base.RecieveDamage(weapon);
            if (HP <= 0)
                return;

            barControll.SetValues(HP, true);
            timer = HealthBarTimer;
        }

        public override void OnDeath(bool destroy = true)
        {
            if (behaviour != null)
                behaviour.Pause(true);

            barControll.SetValues(0, false, 0, HP);

            base.OnDeath();
            var player = GameManager.Instance.PlayerController;
            if (player != null)
                player.Score += KillReward;
        }
    }
}
