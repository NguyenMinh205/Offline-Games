using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NguyenQuangMinh.SoundMemory
{
    public class SoundMemory_SequenceManager : MonoBehaviour
    {
        private Queue<int> sequence = new Queue<int>();
        private Queue<int> playerSequence = new Queue<int>();

        public void ResetSequence()
        {
            sequence.Clear();
            playerSequence.Clear();
        }

        public void AddToSequence()
        {
            int nextColor = Random.Range(0, 4);
            sequence.Enqueue(nextColor);
            playerSequence = new Queue<int>(sequence);
        }

        public bool CheckPlayerSequence(int index)
        {
            if (playerSequence.Count == 0) return false;
            int correctNum = playerSequence.Dequeue();
            if (correctNum == index)
            {
                return true;
            }
            return false;
        }

        public IEnumerator PlaySequenceAnswer(System.Action action)
        {
            yield return new WaitForSeconds(SoundMemory_GameManager.Instance.roundDelay);
            foreach (int seq in sequence)
            {
                SoundMemory_GameManager.Instance.UI.AnimateButton(seq);
                SoundMemory_GameManager.Instance.Audio.PlaySound(seq);
                yield return new WaitForSeconds(SoundMemory_GameManager.Instance.Audio.GetSoundLength(seq) + SoundMemory_GameManager.Instance.Audio.SoundDelay);
            }
            action?.Invoke();
        }

        public bool IsRoundComplete()
        {
            return playerSequence.Count == 0;
        }
    }
}