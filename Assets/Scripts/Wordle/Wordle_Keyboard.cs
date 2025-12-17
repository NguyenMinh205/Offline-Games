using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.Wordle
{
    public class Wordle_Keyboard : MonoBehaviour
    {
        [SerializeField] private List<Wordle_RowKeyCap> _rows;

        private Dictionary<char, Wordle_KeyCap> _keyMap = new Dictionary<char, Wordle_KeyCap>();

        private void Awake()
        {
            InitializeKeyboard();
        }

        private void InitializeKeyboard()
        {
            foreach (Wordle_RowKeyCap row in _rows)
            {
                foreach (Wordle_KeyCap keyCap in row.KeyCaps)
                {
                    if (!_keyMap.ContainsKey(keyCap.Letter))
                    {
                        _keyMap.Add(char.ToUpper(keyCap.Letter), keyCap);
                    }
                }
            }
        }

        public void DisableKey(char letter)
        {
            Debug.Log("Check 2");
            letter = char.ToUpper(letter);
            if (_keyMap.ContainsKey(letter))
            {
                _keyMap[letter].SetState(false);
            }
        }

        public void ResetKeyboard()
        {
            foreach (var key in _keyMap.Values)
            {
                key.ResetKey();
            }
        }
    }
}