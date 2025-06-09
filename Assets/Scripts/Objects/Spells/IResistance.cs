using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Spells
{
    public interface IResistance
    {
        public float GetDuration();
        public void SetDuration(float duration);
        public int GetResistance();
    }
}
