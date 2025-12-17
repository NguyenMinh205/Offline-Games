using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Wordle
{
    [CreateAssetMenu(menuName = "Wordle/Word Database")]
    public class WordDatabase : ScriptableObject
    {
        public List<string> solutions;
        public List<string> validWords;
    }
}