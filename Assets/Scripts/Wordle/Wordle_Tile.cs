using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _letterText;
        [SerializeField] private Image _fill;
        [SerializeField] private Outline _outline;
        private State _state;

        private char _letter;
        public char Letter => _letter;

        public void SetLetter(char letter)
        {
            _letter = letter;
            _letterText.text = letter.ToString().ToUpper();

            if (_letter != '\0')
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
                transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 5, 1);
            }
        }
        public void SetState(State state)
        {
            _state = state;
            _letterText.color = state.wordColor;
            _fill.color = state.fillColor;
            _outline.effectColor = state.outlineColor;
        }

        public Sequence AnimateSetState(State state)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOScaleY(0, 0.25f).SetEase(Ease.InQuad));

            seq.AppendCallback(() =>
            {
               SetState(state);
            });

            seq.Append(transform.DOScaleY(1, 0.25f).SetEase(Ease.OutQuad));

            return seq;
        }

        public void ResetTile(State emptyState)
        {
            _letter = '\0';
            _letterText.text = "";
            _letterText.color = emptyState.wordColor;
            _fill.color = emptyState.fillColor;
            _outline.effectColor = emptyState.outlineColor;
            _state = emptyState;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
    }

    [System.Serializable]
    public class State
    {
        public Color wordColor;
        public Color fillColor;
        public Color outlineColor;
    }

}