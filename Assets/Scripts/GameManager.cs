using Assets.Scripts.Objects.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] public EndGamePanelController EndPanel;
        [SerializeField] public PlayerController PlayerController;
        [SerializeField] public List<Spell> damagers = new List<Spell>();

        public CameraController Camera { get; set; }
        //public BackgroundController Background { get; set; }
        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }


        public void Clear()
        {
            ActivePlayers = new Dictionary<int, GameObject> { };
            ActiveObjects = new Dictionary<int, IObjectController> { };
        }

        protected override void Awake()
        {
            base.Awake();
            ActiveObjects = new Dictionary<int, IObjectController>();
            ActivePlayers = new Dictionary<int, GameObject> { };
        }
    }
}
