using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.ColorBlock
{
    public class DragAndDropBlock : MonoBehaviour
    {
        private Vector3 _offset;
        [SerializeField] private LayerMask mask;

        private ColorBlockTile _tile;
        private ColorBlock_Block block;
        private Transform _currentMovable;
        private bool isDragging = false;

        public void Init()
        {
            _currentMovable = transform.parent;
            if (_tile == null)
                _tile = GetComponent<ColorBlockTile>();
            if (block == null)
                block = transform.parent.GetComponent<ColorBlock_Block>();
        }

        public void OnMouseDown()
        {
            isDragging = true;
            SetOffset(Input.mousePosition);
            block.transform.SetAsLastSibling();
            block.transform.localScale = Vector3.one * 1.2f;
            block.SortingGroup.sortingOrder = 1000;
        }

        public void OnMouseDrag()
        {
            if (!isDragging) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            _currentMovable.position = mousePos + _offset;

            var allCells = ColorBoard_BoardManager.Instance.AllCells;
            foreach (var cell in allCells)
            {
                cell.HighlightCell(false);
            }

            if (block.AcceptAddToBoard())
            {
                Color blockColor = _tile.SpriteRenderer.color;

                foreach (ColorBlockTile tile in block.Tiles)
                {
                    if (!tile.gameObject.activeSelf) continue;

                    Collider2D hitCollider = tile.DragDrop.Hit();
                    if (hitCollider != null)
                    {
                        ColorBlockTileCell cell = hitCollider.GetComponent<ColorBlockTileCell>();
                        if (cell != null)
                        {
                            cell.HighlightCellWithColor(blockColor);
                        }
                    }
                }
            }
        }

        public void OnMouseUp()
        {
            isDragging = false;
            block.OnPointerUp();

            foreach (ColorBlockTileCell cell in FindObjectsOfType<ColorBlockTileCell>())
            {
                cell.HighlightCell(false);
            }
            block.SortingGroup.sortingOrder = 0;
        }

        private void SetOffset(Vector3 position)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(position);
            mousePos.z = 0;
            _offset = _currentMovable.position - mousePos;
        }

        public Collider2D Hit()
        {
            Vector2 origin = transform.position;
            return Physics2D.OverlapPoint(origin, mask);
        }

        public void SetPosToCell()
        {
            Collider2D hit = Hit();
            if (hit && hit.TryGetComponent(out ColorBlockTileCell cell) && cell.IsEmpty)
            {
                transform.DOMove(hit.transform.position, GameManager_ColorBlocks.Instance.SnapDuration)
                    .OnComplete(() => cell.SetBlockSprite(_tile.SpriteRenderer.sprite, _tile.SpriteRenderer.color));
            }
        }
    }
}