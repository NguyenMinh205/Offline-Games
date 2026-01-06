using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NguyenQuangMinh.ColorConnect
{
    public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        [Header("UI Components")]
        [SerializeField] private Image _highlight;
        [SerializeField] private Image _dotColor;
        [Header("Edges")] // Đảm bảo gán đúng reference trong Prefab
        [SerializeField] private Image _topEdge;
        [SerializeField] private Image _bottomEdge;
        [SerializeField] private Image _leftEdge;
        [SerializeField] private Image _rightEdge;

        public Vector2Int Coordinates { get; private set; }
        public TypeOfColor CurrentColorType { get; private set; }
        public TypeOfColor PathColorType { get; private set; }
        public bool IsDot { get; private set; }

        public void Init(Vector2Int coordinates)
        {
            Coordinates = coordinates;
            CurrentColorType = TypeOfColor.None;
            PathColorType = TypeOfColor.None;
            IsDot = false;

            _highlight.gameObject.SetActive(false);
            _dotColor.gameObject.SetActive(false);
            ResetEdges();
        }

        public void SetupDotColor(TypeOfColor colorType, Color visualColor)
        {
            IsDot = true;
            CurrentColorType = colorType;
            PathColorType = colorType;

            _dotColor.gameObject.SetActive(true);
            _dotColor.color = visualColor;

            SetEdgeColor(visualColor);
        }

        public void SetPathColor(TypeOfColor colorType, Color visualColor)
        {
            PathColorType = colorType;
            _highlight.gameObject.SetActive(true);
            _highlight.color = new Color(visualColor.r, visualColor.g, visualColor.b, 0.25f);
            SetEdgeColor(visualColor);
        }
        public void ClearPath()
        {
            if (!IsDot) 
                PathColorType = TypeOfColor.None;

            _highlight.gameObject.SetActive(false);
            ResetEdges();
        }

        private void SetEdgeColor(Color c)
        {
            _topEdge.color = c;
            _bottomEdge.color = c;
            _leftEdge.color = c;
            _rightEdge.color = c;
        }

        public void ResetEdges()
        {
            _topEdge.gameObject.SetActive(false);
            _bottomEdge.gameObject.SetActive(false);
            _leftEdge.gameObject.SetActive(false);
            _rightEdge.gameObject.SetActive(false);
        }

        public void EnableEdge(Vector2Int direction)
        {
            if (direction == new Vector2Int(-1,0)) _topEdge.gameObject.SetActive(true);
            else if (direction == new Vector2Int(1, 0)) _bottomEdge.gameObject.SetActive(true);
            else if (direction == new Vector2Int(0, -1)) _leftEdge.gameObject.SetActive(true);
            else if (direction == new Vector2Int(0, 1)) _rightEdge.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ColorConnectGameManager.Instance.OnTilePointerDown(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ColorConnectGameManager.Instance.OnTilePointerEnter(this);
        }
    }
}