using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.TicTacToe
{
    public class TicTacToeCellController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _trisOImage;
        [SerializeField] private Image _trisXImage;
        private CellState _currentState = CellState.Empty;
        private int _cellID;
        public CellState CurrentState { get { return _currentState; } }
        public int CellID { get { return _cellID; } set { _cellID = value; } }

        public void OnPointerClick(PointerEventData eventData)
        {
            TicTacToeGameManager.Instance.CellClicked(_cellID);
        }

        public void SetState(CellState _newState)
        {
            if (_newState == CellState.Empty)
            {
                _trisOImage.gameObject.SetActive(false);
                _trisXImage.gameObject.SetActive(false);
            }
            else if (_newState == CellState.O)
            {
                _trisOImage.gameObject.SetActive(true);
                _trisXImage.gameObject.SetActive(false);
            }
            else if (_newState == CellState.X)
            {
                _trisOImage.gameObject.SetActive(false);
                _trisXImage.gameObject.SetActive(true);
            }
            _currentState = _newState;
        }
    }

    public enum CellState
    {
        Empty,
        X,
        O
    }
}
