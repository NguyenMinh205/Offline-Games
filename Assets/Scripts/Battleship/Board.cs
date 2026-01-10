using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Battleship
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private List<InfoShip> shipList = new List<InfoShip>();
        [SerializeField] private Color colorCell;

        public List<InfoShip> ShipList => shipList;
        public Grid Grid => grid;

        private List<Vector2Int> hitCoordinates = new List<Vector2Int>();

        public void Initialize()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid>();
                if (grid == null)
                {
                    Debug.LogError("Grid component not found!");
                    return;
                }
            }
        }

        public bool PlaceShip(InfoShip ship, Vector2Int startCoord) //Kiểm tra xem thuyền có đặt được không, nếu được thì lưu vào 
        {
            if (ship == null || grid == null) return false;

            foreach (var coord in ship.OccupiedCoords)
            {
                Cell cell = grid.GetCell(coord);
                if (cell == null || cell.HasShipPart)
                {
                    return false;
                }
            }

            foreach (var coord in ship.OccupiedCoords)
            {
                Cell cell = grid.GetCell(coord);
                cell?.SetShipPart(true);
            }

            shipList.Add(ship);
            return true;
        }

        public bool CheckHit(Vector2Int coord) //Kiểm tra xem có bắn trúng thuyền hay không
        {
            if (grid == null) return false;

            Cell cell = grid.GetCell(coord);
            if (cell == null || cell.IsShot) return false;

            cell.SetIsShot(true);
            hitCoordinates.Add(coord);

            bool isHit = cell.HasShipPart;
            if (isHit)
            {
                Debug.Log($"Hit at {coord}!");
                CheckSunkShips();
            }
            else
            {
                Debug.Log($"Miss at {coord}");
            }

            return isHit;
        }

        private void CheckSunkShips()
        {
            foreach (var ship in shipList)
            {
                if (ship.IsSunk(hitCoordinates))
                {
                    Debug.Log($"Ship at {ship.StartCoord} has been sunk!");
                }
            }
        }

        public void ClearBoard()
        {
            if (grid == null) return;

            foreach (var row in grid.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell.SetShipPart(false);
                    cell.SetIsShot(false);
                }
            }

            shipList.Clear();
            hitCoordinates.Clear();
        }

        public void HighlightShipCells(InfoShip ship, Color validColor, Color invalidColor)
        {
            if (ship == null || grid == null) return;

            bool isValidPlacement = true;
            foreach (var coord in ship.OccupiedCoords)
            {
                Cell cell = grid.GetCell(coord);
                if (cell == null || cell.HasShipPart)
                {
                    isValidPlacement = false;
                    break;
                }
            }

            foreach (var coord in ship.OccupiedCoords)
            {
                Cell cell = grid.GetCell(coord);
                cell?.HighLight(isValidPlacement ? validColor : invalidColor);
            }
        }

        public void ClearHighlights()
        {
            if (grid == null) return;

            foreach (var row in grid.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell?.HighLight(colorCell);
                }
            }
        }
    }
}