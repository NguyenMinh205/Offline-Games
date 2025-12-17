using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_KeyCap : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private char _letter;

        public char Letter => _letter;

        public void SetState(bool isActive)
        {
            Debug.Log("check");
            _button.interactable = isActive;
            _canvasGroup.alpha = isActive ? 1f : 0.5f;
        }

        public void ResetKey()
        {
            SetState(true);
        }
    }
}