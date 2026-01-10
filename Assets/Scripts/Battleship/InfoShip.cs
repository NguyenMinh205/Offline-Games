using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Battleship
{
    public enum ShipDirection { Horizontal, Vertical }

    public class InfoShip : MonoBehaviour
    {
        [SerializeField] private int _length;
        public int Length => _length;
        [SerializeField] private ShipDirection _direction;
        public ShipDirection Direction => _direction;
        [SerializeField] private Vector2Int _startCoord;
        public Vector2Int StartCoord => _startCoord;
        [SerializeField] private List<Vector2Int> _occupiedCoords = new List<Vector2Int>();
        public List<Vector2Int> OccupiedCoords => _occupiedCoords;

        private bool _isPlaced = false;
        public bool IsPlaced => _isPlaced;

        public void Initialize(Vector2Int start, int len, ShipDirection dir)
        {
            _startCoord = start;
            _length = len;
            _direction = dir;
            CalculateOccupiedCoords();
        }

        void CalculateOccupiedCoords()
        {
            _occupiedCoords.Clear();
            for (int i = 0; i < _length; i++)
            {
                Vector2Int coord = _direction == ShipDirection.Horizontal
                    ? new Vector2Int(_startCoord.x + i, _startCoord.y)
                    : new Vector2Int(_startCoord.x, _startCoord.y + i);
                _occupiedCoords.Add(coord);
            }
        }

        public bool IsSunk(List<Vector2Int> hitCoords) //Kiểm tra xem thuyền bị bắn hết chưa
        {
            foreach (var coord in _occupiedCoords)
            {
                if (!hitCoords.Contains(coord))
                    return false;
            }
            return true;
        }

        public void Rotate()
        {
            ShipDirection newDirection = _direction == ShipDirection.Horizontal
                ? ShipDirection.Vertical
                : ShipDirection.Horizontal;

            List<Vector2Int> newCoords = new List<Vector2Int>();
            for (int i = 0; i < _length; i++)
            {
                Vector2Int coord = newDirection == ShipDirection.Horizontal
                    ? new Vector2Int(_startCoord.x + i, _startCoord.y)
                    : new Vector2Int(_startCoord.x, _startCoord.y + i);
                newCoords.Add(coord);
            }

            bool isValid = true;

            if (isValid)
            {
                _direction = newDirection;
                CalculateOccupiedCoords();
                UpdateRotationVisual();
            }
        }

        private void UpdateRotationVisual()
        {
            transform.eulerAngles = new Vector3(0f, 0f, _direction == ShipDirection.Horizontal ? 0f : 90f);
        }

        public void MarkAsPlaced()
        {
            _isPlaced = true;
            GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        public void MarkAsUnplaced()
        {
            _isPlaced = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void UpdateCoordinates(Vector2Int start, List<Vector2Int> coords)
        {
            _startCoord = start;
            _occupiedCoords = new List<Vector2Int>(coords);
        }
    }
}