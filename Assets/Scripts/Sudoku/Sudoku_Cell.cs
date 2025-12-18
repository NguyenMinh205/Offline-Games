using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.Sudoku
{
    public class Sudoku_Cell : MonoBehaviour, IPointerClickHandler
    {
        private int _value, _row, _column;
        private bool _isGiven, _isSolved, _isIncorrect;

        [SerializeField] private Image _bgImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        [Space]
        [Header("Default")]
        [SerializeField] private Color _defaultCellColor;
        [SerializeField] private Color _defaultCellText;
        [SerializeField] private Color _defaultCorrectTextColor;
        [SerializeField] private Color _defaultWrongTextColor;

        [Space]
        [Header("Highlight")]
        [SerializeField] private Color _highlightCellColor;

        [Space]
        [Header("Selected")]
        [SerializeField] private Color _selectedCellColor;
        [SerializeField] private Color _selectedTextColor;
        [SerializeField] private Color _selectedWrongCellColor;
        [SerializeField] private Color _selectedCorrectCellColor;


        public int Value { get { return _value; } }
        public int Row { get { return _row; } set { _row = value; } }
        public int Column { get { return _column; } set { _column = value; } }
        public bool IsGiven { get { return _isGiven; } set { this._isGiven = value; } }
        public bool IsIncorrect { get { return _isIncorrect; } set { this._isIncorrect = value; } }

        public void Init(int value)
        {
            _isIncorrect = false;
            _value = value;

            if (value != 0)
            {
                _isGiven = true;
                _valueText.text = value.ToString();
            }
            else
            {
                _isGiven = false;
                _isSolved = false;
                _valueText.text = "";
            }
        }

        public void BackToDefault()
        {
            _bgImage.color = _defaultCellColor;
            _valueText.color = _defaultCellText;
            if (_isIncorrect && _isSolved)
            {
                _valueText.color = _defaultWrongTextColor;
            }
            else if (!_isIncorrect && _isSolved)
            {
                _valueText.color = _defaultCorrectTextColor;
            }
        }

        public void Highlight()
        {
            _bgImage.color = _highlightCellColor;
        }

        public void Select()
        {
            _bgImage.color = _selectedCellColor;
            _valueText.color = _selectedTextColor;
            if (_isIncorrect)
            {
                _bgImage.color = _selectedWrongCellColor;
                _valueText.color = _defaultWrongTextColor;
            }
            else
            {
                _bgImage.color = _selectedCorrectCellColor;
                _valueText.color = _defaultCorrectTextColor;
            }
        }

        public void UpdateValue(int newValue)
        {
            if (_isGiven) return;
            _value = newValue;
            if (newValue != 0)
            {
                _valueText.text = newValue.ToString();
                _isSolved = true;
            }
            else
            {
                _valueText.text = "";
                _isSolved = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Sudoku_GameManager.Instance.OnCellClicked(this);
        }
    }
}
