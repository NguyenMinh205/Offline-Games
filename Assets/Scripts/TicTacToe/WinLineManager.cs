using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NguyenQuangMinh.TicTacToe
{
    public enum WinLine
    {
        None, VerticalLeft, VerticalMiddle, VerticalRight,
        HorizontalTop, HorizontalMiddle, HorizontalBottom,
        DiagonalUp, DiagonalDown
    }

    public class WinLineManager : MonoBehaviour
    {
        [Header("Line Objects")]
        [SerializeField] private GameObject verticalLeftWin;
        [SerializeField] private GameObject verticalMiddleWin;
        [SerializeField] private GameObject verticalRightWin;
        [SerializeField] private GameObject horizontalTopWin;
        [SerializeField] private GameObject horizontalMiddleWin;
        [SerializeField] private GameObject horizontalBottomWin;
        [SerializeField] private GameObject diagonalUpWin;
        [SerializeField] private GameObject diagonalDownWin;

        [Header("Animation Settings")]
        [SerializeField] private float _animDuration = 0.5f;
        [SerializeField] private Ease _animEase = Ease.OutBack;

        public void SetWinLine(WinLine line)
        {
            DisableAllLines();

            GameObject targetLine = null;

            switch (line)
            {
                case WinLine.VerticalLeft: targetLine = verticalLeftWin; break;
                case WinLine.VerticalMiddle: targetLine = verticalMiddleWin; break;
                case WinLine.VerticalRight: targetLine = verticalRightWin; break;
                case WinLine.HorizontalTop: targetLine = horizontalTopWin; break;
                case WinLine.HorizontalMiddle: targetLine = horizontalMiddleWin; break;
                case WinLine.HorizontalBottom: targetLine = horizontalBottomWin; break;
                case WinLine.DiagonalUp: targetLine = diagonalUpWin; break;
                case WinLine.DiagonalDown: targetLine = diagonalDownWin; break;
                case WinLine.None: return;
            }

            if (targetLine != null)
            {
                targetLine.SetActive(true);
                targetLine.transform.localScale = Vector3.zero;
                targetLine.transform.DOScale(Vector3.one, _animDuration).SetEase(_animEase);
            }
        }

        private void DisableAllLines()
        {
            ResetLine(verticalLeftWin);
            ResetLine(verticalMiddleWin);
            ResetLine(verticalRightWin);
            ResetLine(horizontalTopWin);
            ResetLine(horizontalMiddleWin);
            ResetLine(horizontalBottomWin);
            ResetLine(diagonalUpWin);
            ResetLine(diagonalDownWin);
        }

        private void ResetLine(GameObject lineObj)
        {
            if (lineObj != null)
            {
                lineObj.transform.DOKill();
                lineObj.SetActive(false);
            }
        }
    }
}