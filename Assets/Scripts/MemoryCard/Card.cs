using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.MemoryCard
{
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _cardImage;
        [SerializeField] private Sprite _hiddenSprite;
        [SerializeField] private Sprite _cardSprite;
        public Sprite CardSprite => _cardSprite;
        private bool _isSelected;
        public bool IsSelected => _isSelected;

        private bool _isMatched = false;
        public bool IsMatched => _isMatched;

        private bool _isFlipping = false;
        private float _flipDuration = 0.2f;

        private void Awake()
        {
            _cardImage = GetComponent<Image>();
        }

        public void SetCardSprite(Sprite sprite)
        {
            _cardSprite = sprite;
        }

        public void Show()
        {
            _isSelected = true;
            _isFlipping = true;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 90, 0), _flipDuration).SetEase(Ease.Linear));
            seq.AppendCallback(() => { _cardImage.sprite = _cardSprite; });
            seq.Append(transform.DORotate(Vector3.zero, _flipDuration).SetEase(Ease.Linear));
            seq.OnComplete(() => { _isFlipping = false; });
        }

        public void Hide()
        {
            _isSelected = false;
            _isFlipping = true;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 90, 0), _flipDuration).SetEase(Ease.Linear));
            seq.AppendCallback(() => { _cardImage.sprite = _hiddenSprite; });
            seq.Append(transform.DORotate(Vector3.zero, _flipDuration).SetEase(Ease.Linear));

            seq.OnComplete(() => { _isFlipping = false; });
        }

        public void SetMatched()
        {
            _isMatched = true;
            _isSelected = false;

            _cardImage.DOFade(0f, 0.3f).SetEase(Ease.OutBounce).OnComplete(() => { _cardImage.raycastTarget = false; });
        }

        public void ResetCard()
        {
            _isMatched = false;
            _isSelected = false;
            _isFlipping = false;

            transform.rotation = Quaternion.identity;

            var tempColor = _cardImage.color;
            tempColor.a = 1f;
            _cardImage.color = tempColor;

            _cardImage.raycastTarget = true;

            _cardImage.sprite = _hiddenSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isSelected || _isMatched || _isFlipping) return;

            MemoryCardGameManager.Instance.SetSelect(this);
        }
    }
}