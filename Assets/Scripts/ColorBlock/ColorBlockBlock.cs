using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBlock_Block : MonoBehaviour
    {
        [SerializeField] private List<ColorBlockTile> tiles;
        public List<ColorBlockTile> Tiles => tiles;
        [SerializeField] private ColorBlock_Block originalPrefab;
        public ColorBlock_Block OriginalPrefab { get { return originalPrefab; } set { originalPrefab = value; } }
        private ColorBlockSlot slot;
        [SerializeField] private SortingGroup sortingGroup;
        public SortingGroup SortingGroup => sortingGroup;
        private Dictionary<ColorBlockTile, Vector3> originalLocalPositions = new Dictionary<ColorBlockTile, Vector3>();

        public void InitBlock(ColorBlockSlot slot)
        {
            this.slot = slot;
            this.slot.SetBlock(this);

            if (originalLocalPositions.Count == 0)
            {
                foreach (var tile in tiles)
                {
                    originalLocalPositions[tile] = tile.transform.localPosition;
                }
            }

            foreach (var tile in tiles)
            {
                tile.gameObject.SetActive(true);
                tile.DragDrop.Init();
            }
        }

        public bool AcceptAddToBoard()
        {
            foreach (var tile in tiles)
            {
                if (!tile.gameObject.activeSelf) continue;

                Collider2D hitCollider = tile.DragDrop.Hit();
                if (hitCollider == null)
                {
                    return false;
                }

                ColorBlockTileCell cell = hitCollider.GetComponent<ColorBlockTileCell>();
                if (cell == null || !cell.IsEmpty || !ColorBoard_BoardManager.Instance.IsValidPosition(cell.coordinates.x, cell.coordinates.y))
                {
                    return false;
                }
            }
            return true;
        }

        public void ResetTilePositions()
        {
            foreach (var tile in tiles)
            {
                if (originalLocalPositions.ContainsKey(tile))
                {
                    tile.transform.localPosition = originalLocalPositions[tile];
                }
            }
        }

        public void SetPosAll()
        {
            foreach (var tile in tiles)
            {
                if (!tile.gameObject.activeSelf) continue;
                tile.DragDrop.SetPosToCell();
            }
        }

        public void OnPointerUp()
        {
            if (AcceptAddToBoard())
            {
                SetPosAll();
                slot?.ResetSlot();
                StartCoroutine(DespawnAfter(GameManager_ColorBlocks.Instance.SnapDuration * 2 + 0.05f));
            }
            else
            {
                ReturnToSlot();
            }
        }

        private void ReturnToSlot()
        {
            if (slot != null)
            {
                transform.DOMove(slot.transform.position, 0.25f).SetEase(Ease.OutBack);
                transform.DOScale(Vector3.one, 0.15f);
            }
        }

        private IEnumerator DespawnAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameManager_ColorBlocks.Instance.BoardManager.CheckGrid();
            GameManager_ColorBlocks.Instance.Spawner.ReturnBlock(slot, this);
        }
    }
}