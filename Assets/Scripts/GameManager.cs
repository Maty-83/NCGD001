using Assets.Scripts.Objects.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditorInternal;
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
        [SerializeField] public GameObject PlayerDeathPlacePrefab;

        [SerializeField] public List<LairController> Lairs = new List<LairController>();
        [SerializeField] public TMP_Text text;

        public CameraController Camera { get; set; }
        //public BackgroundController Background { get; set; }
        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }


        public static Vector3 PlayerDeathPosition = Vector3.negativeInfinity;
        public static Vector3 LastCheckpoint = Vector3.negativeInfinity;
        public static float DroppedScore = 0;

        public void DestroyLair(GameObject lair)
        {
            for (int i = 0; i < GameManager.Instance.Lairs.Count; i++)
            {
                if (GameManager.Instance.Lairs[i].GetInstanceID() == lair.GetInstanceID())
                    GameManager.Instance.Lairs.RemoveAt(i);
            }
        }

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

        private void Start()
        {
            if (PlayerDeathPosition.y != float.NegativeInfinity)
            {
                var controller = Instantiate(PlayerDeathPlacePrefab).GetComponent<SoulsDropController>();
                controller.gameObject.transform.position = new Vector3(PlayerDeathPosition.x, PlayerDeathPosition.y, 0);
                if (controller != null)
                    controller.Init(DroppedScore);
            }
        }
    }
}
