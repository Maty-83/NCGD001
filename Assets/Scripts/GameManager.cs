using Assets.Scripts.Objects.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] public BackgroundController BackgroundController;
        [SerializeField] public BulletInfoPanelController BulletInfoController;
        [SerializeField] public EndGamePanelController EndPanel;
        [SerializeField] public SpellPanel SpellPanel;
        [SerializeField] public PlayerController PlayerController;
        [SerializeField] public List<Weapon> damagers = new List<Weapon>();

        public CameraController Camera { get; set; }
        //public BackgroundController Background { get; set; }
        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }


        public void Clear()
        {
            ActivePlayers = new Dictionary<int, GameObject> { };
            ActiveObjects = new Dictionary<int, IObjectController> { };
        }

        public List<Weapon> GetAvalibleWeapons()
        {
            var avalibleWeapons = new List<Weapon>();
            foreach (var weapon in GameManager.Instance.damagers)
            {
                if (PlayerController.OwnedWeapons.Contains(weapon))
                {
                    continue;
                }
                avalibleWeapons.Add(weapon);
            }
            return avalibleWeapons;
        }


        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }

        public void Reset()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1;
        }

        public void Quit()
        {
            Application.Quit();
        }

        protected override void Awake()
        {
            base.Awake();
            ActiveObjects = new Dictionary<int, IObjectController>();
            ActivePlayers = new Dictionary<int, GameObject> { };
            damagers = damagers.OrderBy(x => x.BuyCost).ToList();
        }
    }
}
