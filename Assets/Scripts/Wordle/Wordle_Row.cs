using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_Row : MonoBehaviour
    {
        [SerializeField] private List<Wordle_Tile> _tiles;
        public List<Wordle_Tile> Tiles { get { return _tiles; } }
    }
}
