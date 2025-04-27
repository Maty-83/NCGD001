using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IObjectController
    {
        public void Play();
        public void Pause();
        public void Enter();
        public void Exit();
    }
}
