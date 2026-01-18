using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NguyenQuangMinh.MemoryCard
{
    public class MemoryCardGameManager : Singleton<MemoryCardGameManager>, IGameManager
    {
        [Header("Settings")]
        [SerializeField] private Card cardPrefab;
        [SerializeField] private List<Transform> _cardRow; 
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private int numOfPairs;

        [Header("Game Config")]
        [SerializeField] private float _timeCheckCard = 1.0f;

        [Header("Animation")]
        [SerializeField] private float _dealDuration = 0.3f;
        [SerializeField] private float _dealDelay = 0.05f;

        private List<Sprite> _cardSprites;
        private List<Card> _cards;

        private Card _firstCard;
        private Card _secondCard;
        private int _matchedPairsCount;
        private bool _isProcessing;

        public void StartNewGame()
        {
            StopAllCoroutines();

            if (_cards != null && _cards.Count > 0)
            {
                foreach (var card in _cards)
                {
                    if (card != null && card.gameObject != null)
                    {
                        card.transform.DOKill();
                        PoolingManager.Despawn(card.gameObject);
                    }
                }
            }

            _cardSprites = new List<Sprite>();
            _cards = new List<Card>();
            _matchedPairsCount = 0;
            _isProcessing = false;

            _firstCard = null;
            _secondCard = null;

            foreach (var row in _cardRow)
            {
                if (row != null) row.gameObject.SetActive(true);
            }

            RunGame();
        }

        public void Restart()
        {
            StopAllCoroutines();

            StartNewGame();
        }

        public void RunGame()
        {
            PrepareSprites();
            StartCoroutine(CreateCardsRoutine());
        }

        public void PrepareSprites()
        {
            _cardSprites.Clear();
            ShuffleSprite();

            for (int i = 0; i < numOfPairs; i++)
            {
                Sprite s = _sprites[i % _sprites.Count];
                _cardSprites.Add(s);
                _cardSprites.Add(s);
            }
            ShuffleCard();
        }

        public void ShuffleSprite()
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                int randomIndex = Random.Range(i, _sprites.Count);
                Sprite temp = _sprites[i];
                _sprites[i] = _sprites[randomIndex];
                _sprites[randomIndex] = temp;
            }
        }

        public void ShuffleCard()
        {
            for (int i = 0; i < _cardSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, _cardSprites.Count);
                Sprite temp = _cardSprites[i];
                _cardSprites[i] = _cardSprites[randomIndex];
                _cardSprites[randomIndex] = temp;
            }
        }

        private IEnumerator CreateCardsRoutine()
        {
            int totalCards = _cardSprites.Count;
            int totalRows = _cardRow.Count;
            int currentCardIndex = 0;

            for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
            {
                Transform currentRow = _cardRow[rowIndex];
                int limitInThisRow = 0;

                if (rowIndex == 0)
                {
                    limitInThisRow = 3;
                }
                else if (rowIndex == totalRows - 1)
                {
                    limitInThisRow = 3;
                }
                else
                {
                    limitInThisRow = 4;
                }

                if (currentCardIndex < totalCards)
                {
                    if (!currentRow.gameObject.activeSelf) currentRow.gameObject.SetActive(true);

                    for (int j = 0; j < limitInThisRow; j++)
                    {
                        if (currentCardIndex >= totalCards) break;

                        Card newCard = PoolingManager.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, currentRow);
                        AudioManager.Instance.PlayCardPlaceSound();

                        newCard.ResetCard();
                        newCard.SetCardSprite(_cardSprites[currentCardIndex]);

                        _cards.Add(newCard);

                        newCard.transform.localScale = Vector3.zero;
                        newCard.transform.DOScale(Vector3.one, _dealDuration).SetEase(Ease.OutBack);

                        currentCardIndex++;
                        yield return new WaitForSeconds(_dealDelay);
                    }
                }
                else
                {
                    if (currentRow.gameObject.activeSelf) currentRow.gameObject.SetActive(false);
                }
            }
        }

        public void SetSelect(Card card)
        {
            if (_isProcessing || card.IsSelected || card.IsMatched) return;

            card.Show();

            if (_firstCard == null)
            {
                _firstCard = card;
            }
            else if (_secondCard == null)
            {
                _secondCard = card;
                _isProcessing = true;
                StartCoroutine(CheckMatching(_firstCard, _secondCard));
            }
        }

        IEnumerator CheckMatching(Card card1, Card card2)
        {
            yield return new WaitForSeconds(_timeCheckCard);

            if (card1.CardSprite == card2.CardSprite)
            {
                AudioManager.Instance.PlayCardMatchSound();
                card1.SetMatched();
                card2.SetMatched();

                _matchedPairsCount++;
                CheckEndGame();
            }
            else
            {
                card1.Hide();
                card2.Hide();
            }

            _firstCard = null;
            _secondCard = null;
            _isProcessing = false;
        }

        private void CheckEndGame()
        {
            if (_matchedPairsCount >= numOfPairs)
            {
                Debug.Log("GAME OVER.");
            }
        }
    }
}