using Assets.Core.Enums;
using Assets.Helpers.Enums;
using Assets.Scripts.Objects.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResistanceScriptableObject", order = 1)]
    public class ResistanceObject : ScriptableObject, IResistance
    {
        [SerializeField] public DamageType DamageType;
        [SerializeField] public int Percent = 50;
        [SerializeField] public float Duration = -1;

        public float GetDuration()
        {
            if(Duration  < 0)
                return float.MaxValue;
            return Duration;
        }

        public int GetResistance()
        {
            return Percent;
        }

        public void SetDuration(float duration)
        {
            Duration = duration;
        }
    }
}
