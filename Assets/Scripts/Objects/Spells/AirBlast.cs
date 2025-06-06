using Assets.Helpers;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Behaviour;
using Assets.Scripts.Objects.Weapon;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace Assets.Scripts.Objects.Spells
{
    public class AirBlast : MonoBehaviour, IProjectile
    {
        public Rigidbody2D rBody;
        public AnimationClip AnimationClip;
        public float ScaleCoefficient = 0.3f;
        public float PushForce = 5.0f;

        public float Speed { get; set; } = 1000f;
        public float TTL { get; set; } = 1f;
        public Vector2 Direction { get; set; }
        public GameObject Shooter { get; set; }

        private IDamager Weapon = new Gun();

        public void Init(GameObject shooter, IDamager weapon, Rigidbody2D rigid, Vector2 direction)
        {
            Shooter = shooter;
            Direction = direction;
            Weapon = weapon;
            rBody = rigid;

            //if(AnimationClip != null)
            //{
            //    TTL = AnimationClip.length;
            //}
        }

        public void AfterHit()
        {
            Destroy();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnHit()
        { 
        }

        public void Shoot()
        {
        }

        void Update()
        {
            if (TTL < 0)
            {
                Destroy();
                return;
            }
            transform.localScale = new Vector3(transform.localScale.x + ScaleCoefficient, transform.localScale.y + ScaleCoefficient, 1);
            transform.position = Shooter.transform.position;
            TTL -= 0.1f;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Shooter == null)
                return;

            var collided = collision.gameObject;
            if (collided.gameObject == Shooter)
                return;

            var entity = collided.GetComponent<Entity>();
            if(entity != null)
            {
                entity.RecieveDamage(Weapon);
            }
            
            var leaf = collided.GetComponent<SakuraLeafController>();
            if(leaf || entity)
            {
                Blow(collided);
            }
        }

        private void Blow(GameObject collided)
        {
            var entityRB = collided.GetComponent<Rigidbody2D>();
            var dir = (entityRB.position - new Vector2(transform.position.x, transform.position.y)) * PushForce;

            var behaviour = collided.GetComponent<IBehaviour>();
            if(behaviour != null)
            {
                behaviour.Push(dir);
            }
            else
            {
                entityRB.AddForce(dir, ForceMode2D.Force);
            }
        }
    }
}
