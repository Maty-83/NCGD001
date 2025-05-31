using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class CardController : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TMP_Text binding;


        public void Set(Sprite sprite, string text)
        {
            image.sprite = sprite;
            binding.text = text;
        }
    }
}
