using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Entities
{
    public class BasicEnemyController : Entity
    {
        [SerializeField] int HealthBarTimer = 10;
        [SerializeField] BarController barControll = null;
        [SerializeField] float ShootingRange = 5f;
        [SerializeField] float KillReward = 500f;

        private int timer = 0;

        private new void Start()
        {
            base.Start();
            barControll.SetValues(HP, false, 0, HP);
        }
        private new void Update()
        {
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
            base.RecieveDamage(weapon);
            if (HP <= 0)
                return;

            barControll.SetValues(HP, true);
            timer = HealthBarTimer;
        }

        public override void OnDeath(bool destroy = true)
        {
            base.OnDeath();
            var player = GameManager.Instance.PlayerController;
            if (player != null)
                player.Score += KillReward;
        }
    }
}
