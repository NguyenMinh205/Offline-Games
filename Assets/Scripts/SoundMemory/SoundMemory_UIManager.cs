using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.SoundMemory
{
    public class SoundMemory_UIManager : MonoBehaviour
    {
        [SerializeField] private List<Button> _soundButtons = new List<Button>();
        [SerializeField] private TextMeshProUGUI _scoreTxt;
        [SerializeField] private Color _clickColor;
        [SerializeField] private List<Color> _originalColors = new List<Color>();
        [SerializeField] private float _animDuration = 0.5f;

        private Coroutine _currentAnimation;

        public void Init()
        {
            _originalColors.Clear();
            for (int i = 0; i < _soundButtons.Count; i++)
            {
                int index = i;
                _soundButtons[i].onClick.RemoveAllListeners();
                _soundButtons[i].onClick.AddListener(() => SoundMemory_GameManager.Instance.OnClickButtonSound(index));
                if (_soundButtons[i] != null && _soundButtons[i].image != null)
                {
                    _originalColors.Add(_soundButtons[i].image.color);
                }
                _soundButtons[i].interactable = false;
            }
        }

        public void EnableButtons(bool enable)
        {
            foreach (Button btn in _soundButtons)
            {
                btn.interactable = enable;
            }
        }

        public void AnimateButton(int index)
        {
            if (index < 0 || index >= _soundButtons.Count || _soundButtons[index].image == null) return;
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
            }
            _currentAnimation = StartCoroutine(AnimateButtonCoroutine(index));
        }

        private IEnumerator AnimateButtonCoroutine(int index)
        {
            Image img = _soundButtons[index].image;
            Color original = _originalColors[index];

            img.color = _clickColor;
            yield return new WaitForSeconds(_animDuration);

            img.color = original;
        }

        public void UpdateScore(int score)
        {
            _scoreTxt.text = score.ToString();
        }
    }
}
