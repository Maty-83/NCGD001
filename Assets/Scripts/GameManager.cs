using Assets.Scripts.Entities;
using Assets.Scripts.Objects.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        [SerializeField] public TMP_Text MissonDesciprion;
        [SerializeField] public string MissonDesciprionText;
        [SerializeField] public ProtectionPanelManager ProtectionPanel;

        [SerializeField] private Image Mask;
        [SerializeField] private float MaskTransitionDuration;


        public CameraController Camera { get; set; }
        //public BackgroundController Background { get; set; }
        public Dictionary<int, IObjectController> ActiveObjects { get; set; }
        public Dictionary<int, GameObject> ActivePlayers { get; set; }

        private int lairCount = 0;

        public static Entity Player = null;
        public static Vector3 PlayerDeathPosition = Vector3.negativeInfinity;
        public static Vector3 LastCheckpoint = Vector3.negativeInfinity;
        public static CheckPointController CheckPointController = null;
        public static float DroppedScore = 0;

        public void DestroyLair(GameObject lair)
        {
            for (int i = 0; i < Lairs.Count; i++)
            {
                if (Lairs[i].gameObject.GetInstanceID() == lair.GetInstanceID())
                {
                    Lairs.RemoveAt(i);
                    MissonDesciprion.text = $"{MissonDesciprionText} {Lairs.Count}/{lairCount}";
                }
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

        public void LoadNextLevel()
        {
            PlayerDeathPosition = Vector3.negativeInfinity;
            CheckPointController = null;
            LastCheckpoint = Vector3.negativeInfinity;
            DroppedScore = 0;
        }

        public void ClearCheckpoint()
        {
            Player = null;
            PlayerDeathPosition = Vector3.negativeInfinity;
            CheckPointController = null;
            LastCheckpoint = Vector3.negativeInfinity;
            DroppedScore = 0;
        }

        public void Reset()
        {
            ClearCheckpoint();
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1;
        }

        public void Quit()
        {
            Application.Quit();
        }

        public IEnumerator FadeMask(float startAlpha = 0f, float endAlpha = 1f)
        {
            if (Mask == null) yield break;

            Color color = Mask.color;
            float time = 0f;

            while (time < MaskTransitionDuration)
            {
                float t = time / MaskTransitionDuration;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                Mask.color = color;
                time += Time.deltaTime;
                yield return null;
            }

            color.a = endAlpha;
            Mask.color = color;
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
            lairCount = Lairs.Count;
            MissonDesciprion.text = $"{MissonDesciprionText} {lairCount}/{Lairs.Count}";

            if (PlayerDeathPosition.y != float.NegativeInfinity)
            {
                var controller = Instantiate(PlayerDeathPlacePrefab).GetComponent<SoulsDropController>();
                controller.gameObject.transform.position = new Vector3(PlayerDeathPosition.x, PlayerDeathPosition.y, 0);
                if (controller != null)
                    controller.Init(DroppedScore);
            }

            if (Mask != null)
            {
                Mask.color = new Color(0, 0, 0, 1f); // Ensure fully opaque
                StartCoroutine(FadeMask(1f, 0f)); // fade from 1 → 0 over 1 second
            }
        }
    }
}
