using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.Battleship
{
    public class DraggableItem : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private InfoShip ship;
        [SerializeField] private Board board;

        private Vector3 originalPosition;
        private bool canPlace = false;
        private RectTransform rectTransform;
        private BoxCollider2D boxCollider;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            boxCollider = GetComponent<BoxCollider2D>();

            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            if (board == null)
                board = GetComponentInParent<Board>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = rectTransform.position;
            canPlace = false;
            board.ClearHighlights();

            if (boxCollider != null)
                boxCollider.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            Vector2Int coord = board.Grid.GetCoordFromWorldPos(rectTransform.position);

            ship.Initialize(coord, ship.Length, ship.Direction);
            canPlace = IsValidPlacement();

            board.HighlightShipCells(ship, canPlace ? Color.cyan : Color.red, Color.red);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Re-enable collider
            if (boxCollider != null)
                boxCollider.enabled = true;

            if (!canPlace || !IsWithinGridBounds())
            {
                ReturnToOriginalPosition();
                return;
            }

            SnapToGrid();

            if (board.PlaceShip(ship, ship.StartCoord))
            {
                ship.MarkAsPlaced();
            }
            else
            {
                ReturnToOriginalPosition();
            }

            board.ClearHighlights();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && ship.IsPlaced)
            {
                RotateShip();
            }
        }

        private bool IsValidPlacement()
        {
            foreach (var coord in ship.OccupiedCoords)
            {
                Cell cell = board.Grid.GetCell(coord);
                if (cell == null || cell.HasShipPart)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsWithinGridBounds()
        {
            foreach (var coord in ship.OccupiedCoords)
            {
                if (coord.x < 0 || coord.x >= board.Grid.GetWidth() ||
                    coord.y < 0 || coord.y >= board.Grid.GetHeight())
                {
                    return false;
                }
            }
            return true;
        }

        private void ReturnToOriginalPosition()
        {
            rectTransform.position = originalPosition;
            board.ClearHighlights();
        }

        private void SnapToGrid()
        {
            Cell snapCell = board.Grid.GetCell(ship.StartCoord);
            if (snapCell != null)
            {
                rectTransform.position = snapCell.transform.position;
            }
        }

        private void RotateShip()
        {
            ship.Rotate();
            canPlace = IsValidPlacement() && IsWithinGridBounds();
            board.HighlightShipCells(ship,
                canPlace ? Color.cyan : Color.red,
                Color.red);
        }
    }
}