using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_KeyCap : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _letterText;
        [SerializeField] private char _letter;
        [SerializeField] private Color _defaultFillColor = Color.white;
        [SerializeField] private Color _defaultWordColor = Color.black;
        public State CurrentState { get; private set; }

        public char Letter => _letter;

        public void SetState(State state)
        {
            CurrentState = state;

            _letterText.color = state.wordColor;
            _image.color = state.fillColor;
        }

        public void ResetKey()
        {
            CurrentState = null;
            _letterText.color = _defaultWordColor;
            _image.color = _defaultFillColor;
        }
    }
}