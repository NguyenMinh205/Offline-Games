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

        public void UpdateKeyColor(char letter, State newState, State correctState, State wrongSpotState)
        {
            letter = char.ToUpper(letter);
            if (!_keyMap.ContainsKey(letter)) return;

            Wordle_KeyCap key = _keyMap[letter];
            State currentKeyState = key.CurrentState;

            if (currentKeyState == correctState) return;
            if (currentKeyState == wrongSpotState && newState != correctState) return;

            key.SetState(newState);
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