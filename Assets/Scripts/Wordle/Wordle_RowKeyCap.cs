using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_RowKeyCap : MonoBehaviour
    {
        [SerializeField] private List<Wordle_KeyCap> _KeyCaps;
        public List<Wordle_KeyCap> KeyCaps => _KeyCaps;
    }
}
