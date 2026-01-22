using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NguyenQuangMinh.WaterSort
{
    public class BottleController : MonoBehaviour
    {
        [Header("Color Configuration")]
        [SerializeField] private List<BottleColorSO> _colorLayers;
        public List<BottleColorSO> ColorLayers => _colorLayers;

        [Header("Bottle Settings")]
        [SerializeField] private List<float> _fillAmountBottle;
        [SerializeField] private List<float> _rotationValueBottle;
        [SerializeField] private float _timeRotateBottle;
        [SerializeField] private float _moveSpeedMultiplier = 2.0f;

        [Tooltip("Khoảng cách bình nhảy lên khi được chọn")]
        [SerializeField] private float _selectMoveUpAmount = 0.5f;

        [Header("References")]
        [SerializeField] private SpriteRenderer _bottleSR;
        [SerializeField] private Transform _waterTargetPoint;
        public Transform WaterTargetPoint => _waterTargetPoint;

        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve _rotateAndScaleMultiplierCurve;
        [SerializeField] private AnimationCurve _fillAmountCurve;
        [SerializeField] private AnimationCurve _speedRorateCurve;

        [Header("Runtime Info (Read Only)")]
        [Range(0, 4)]
        [SerializeField] private int _numOfColorInBottle;
        public int NumOfColorInBottle { get { return _numOfColorInBottle; } set { _numOfColorInBottle = value; } }
        [SerializeField] private int _numOfTopColorLayer;
        [SerializeField] private Transform _leftRotationPoint;
        public Transform LeftRotationPoint => _leftRotationPoint;
        [SerializeField] private Transform _rightRotationPoint;
        public Transform RightRotationPoint => _rightRotationPoint;

        private Transform _chosenRotationPoint;
        private float _directionMultiplier = 1;

        private BottleColorSO _topColorSO;
        public BottleColorSO TopColor => _topColorSO;
        private int _rotationValueIndex;
        private BottleController _bottleControllerRef;
        public BottleController BottleControllerRef { get { return _bottleControllerRef; } set { _bottleControllerRef = value; } }
        private int _numOfColorsToTransfer = 0;

        private Vector3 _originalPos;
        private Vector3 _startPos;
        private Vector3 _endPos;

        private bool _isCompleted = false;
        public bool IsCompleted => _isCompleted;

        // --- Biến mới để quản lý Tween ---
        private Tween _selectionTween;

        public void SetupBottle(DataWaterSortObject bottleData)
        {
            transform.rotation = Quaternion.identity;
            _bottleSR.transform.localPosition = Vector3.zero;
            _bottleSR.transform.localRotation = Quaternion.identity;

            GetComponent<SpriteRenderer>().sortingOrder = 1;
            _bottleSR.sortingOrder = 0;

            _numOfColorInBottle = bottleData.numOfColorInBottle;
            SetTargetPoint();

            _colorLayers = new List<BottleColorSO>(bottleData._colorsBottle);
            Init();

            SilentCheckCompleteState();
        }

        public void Init()
        {
            _originalPos = transform.position;
            SnapFillAmountToCorrectValue(); // FIX LỖI HIỂN THỊ
            UpdateColor();
            SetTopColorValue();
        }

        public void SnapFillAmountToCorrectValue()
        {
            if (_fillAmountBottle != null && _numOfColorInBottle >= 0 && _numOfColorInBottle < _fillAmountBottle.Count)
            {
                _bottleSR.material.SetFloat("_FillAmount", _fillAmountBottle[_numOfColorInBottle]);
            }
        }

        public void UpdateColor()
        {
            _bottleSR.material.SetColor("_Color1", Color.clear);
            _bottleSR.material.SetColor("_Color2", Color.clear);
            _bottleSR.material.SetColor("_Color3", Color.clear);
            _bottleSR.material.SetColor("_Color4", Color.clear);

            if (_colorLayers.Count >= 1) _bottleSR.material.SetColor("_Color1", _colorLayers[0].colorValue);
            if (_colorLayers.Count >= 2) _bottleSR.material.SetColor("_Color2", _colorLayers[1].colorValue);
            if (_colorLayers.Count >= 3) _bottleSR.material.SetColor("_Color3", _colorLayers[2].colorValue);
            if (_colorLayers.Count >= 4) _bottleSR.material.SetColor("_Color4", _colorLayers[3].colorValue);
        }

        public void StartTransferColor()
        {
            ChosenRotationPointAndDirection();
            _numOfColorsToTransfer = Mathf.Min(_numOfTopColorLayer, 4 - _bottleControllerRef.NumOfColorInBottle);

            for (int i = 0; i < _numOfColorsToTransfer; i++)
            {
                _bottleControllerRef.ColorLayers.Add(_topColorSO);
            }
            _bottleControllerRef.UpdateColor();

            CalculateRotationIndex(4 - _bottleControllerRef.NumOfColorInBottle);

            transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
            _bottleSR.sortingOrder += 2;

            StartCoroutine(MoveBottle());
        }

        public void SetTopColorValue()
        {
            if (_numOfColorInBottle == 0)
            {
                _numOfTopColorLayer = 0;
                _topColorSO = null;
                return;
            }

            _numOfTopColorLayer = 1;
            _topColorSO = _colorLayers[_numOfColorInBottle - 1];
            for (int i = _numOfColorInBottle - 2; i >= 0; i--)
            {
                if (_colorLayers[i] == _topColorSO)
                {
                    _numOfTopColorLayer++;
                }
                else
                {
                    break;
                }
            }

            int remainingColors = _numOfColorInBottle - _numOfTopColorLayer;
            remainingColors = Mathf.Clamp(remainingColors, 0, _rotationValueBottle.Count - 1);
            _rotationValueIndex = _rotationValueBottle.Count - 1 - remainingColors;
        }

        public bool CheckFillColor(BottleColorSO newColorLayer)
        {
            if (_numOfColorInBottle == 0) return true;
            else
            {
                if (_numOfColorInBottle == 4) return false;
                else
                {
                    if (_topColorSO == newColorLayer) return true;
                }
            }
            return false;
        }

        public void CheckCompleteState()
        {
            if (_numOfColorInBottle != 4)
            {
                _isCompleted = false;
                return;
            }

            BottleColorSO firstColor = _colorLayers[0];
            bool sameColor = true;
            for (int i = 1; i < _colorLayers.Count; i++)
            {
                if (_colorLayers[i] != firstColor)
                {
                    sameColor = false;
                    break;
                }
            }

            if (sameColor)
            {
                if (!_isCompleted)
                {
                    _isCompleted = true;
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayWaterSortCompleteBottleSound();
                }
            }
            else
            {
                _isCompleted = false;
            }
        }

        private void SilentCheckCompleteState()
        {
            if (_numOfColorInBottle != 4)
            {
                _isCompleted = false;
                return;
            }
            BottleColorSO firstColor = _colorLayers[0];
            bool sameColor = true;
            for (int i = 1; i < _colorLayers.Count; i++)
            {
                if (_colorLayers[i] != firstColor)
                {
                    sameColor = false; break;
                }
            }
            _isCompleted = sameColor;
        }

        public void SetSelected(bool isSelected)
        {
            if (_selectionTween != null && _selectionTween.IsActive())
                _selectionTween.Kill();

            Vector3 targetPos;
            if (isSelected)
            {
                targetPos = _originalPos + Vector3.up * _selectMoveUpAmount;
            }
            else
            {
                targetPos = _originalPos;
            }

            _selectionTween = transform.DOMove(targetPos, 0.25f)
                .SetEase(isSelected ? Ease.OutBack : Ease.OutQuad);
        }

        public void PlayErrorShakeEffect()
        {
            transform.DOKill(true);

            transform.position = _originalPos + Vector3.up * _selectMoveUpAmount;

            transform.DOShakeRotation(0.4f, new Vector3(0, 0, 15f), 20, 90, true)
                .OnComplete(() => transform.rotation = Quaternion.identity);
        }

        public void CalculateRotationIndex(int numOfEmptySpaceInSecondBottle)
        {
            int val = _numOfColorInBottle - Mathf.Min(numOfEmptySpaceInSecondBottle, _numOfTopColorLayer);
            _rotationValueIndex = _rotationValueBottle.Count - 1 - Mathf.Clamp(val, 0, _rotationValueBottle.Count - 1);
        }

        public void FillUp(float fillAmountToAdd)
        {
            _bottleSR.material.SetFloat("_FillAmount", _bottleSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
        }

        public void ChosenRotationPointAndDirection()
        {
            if (transform.position.x >= _bottleControllerRef.transform.position.x)
            {
                _chosenRotationPoint = _leftRotationPoint;
                _directionMultiplier = -1;
            }
            else
            {
                _chosenRotationPoint = _rightRotationPoint;
                _directionMultiplier = 1;
            }
        }

        public IEnumerator RotateBottleDown()
        {
            float time = 0, lerpValue, angleValue, lastAngleValue = 0;
            AudioManager.Instance.PlayWaterSortPouringSound();

            while (time <= _timeRotateBottle)
            {
                lerpValue = time / _timeRotateBottle;
                angleValue = Mathf.Lerp(0, _directionMultiplier * _rotationValueBottle[_rotationValueIndex], lerpValue);

                transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                _bottleSR.material.SetFloat("_ScaleAndRotationMultiple", _rotateAndScaleMultiplierCurve.Evaluate(0));

                if (_fillAmountBottle.Count > _numOfColorInBottle && _fillAmountBottle[_numOfColorInBottle] > _fillAmountCurve.Evaluate(angleValue) + 0.005f)
                {
                    Vector3 targetMouthPos = _bottleControllerRef.WaterTargetPoint.position;

                    WaterSortGameManager.Instance.UpdateWaterLine(true, _chosenRotationPoint.position, targetMouthPos, _topColorSO.colorValue);

                    _bottleSR.material.SetFloat("_FillAmount", _fillAmountCurve.Evaluate(angleValue));
                    _bottleControllerRef.FillUp(_fillAmountCurve.Evaluate(lastAngleValue) - _fillAmountCurve.Evaluate(angleValue));
                }

                time += Time.deltaTime * _speedRorateCurve.Evaluate(angleValue);
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }

            angleValue = _directionMultiplier * _rotationValueBottle[_rotationValueIndex];
            _bottleSR.material.SetFloat("_ScaleAndRotationMultiple", _rotateAndScaleMultiplierCurve.Evaluate(angleValue));
            _bottleSR.material.SetFloat("_FillAmount", _fillAmountCurve.Evaluate(angleValue));

            for (int i = 0; i < _numOfTopColorLayer; i++)
            {
                if (_colorLayers.Count > 0)
                {
                    _colorLayers.RemoveAt(_colorLayers.Count - 1);
                }
            }

            _numOfColorInBottle -= _numOfColorsToTransfer;
            _bottleControllerRef.NumOfColorInBottle += _numOfColorsToTransfer;

            if (_numOfColorInBottle < 0) _numOfColorInBottle = 0;

            SnapFillAmountToCorrectValue();
            if (_bottleControllerRef != null) _bottleControllerRef.SnapFillAmountToCorrectValue();

            UpdateColor();
            _bottleControllerRef.CheckCompleteState();

            WaterSortGameManager.Instance.UpdateWaterLine(false);

            StartCoroutine(RotateBottleBack());
        }

        public IEnumerator RotateBottleBack()
        {
            float time = 0, lerpValue, angleValue;
            float lastAngleValue = _directionMultiplier * _rotationValueBottle[_rotationValueIndex];

            if (_numOfColorInBottle == 0)
            {
                _bottleSR.material.SetFloat("_ScaleAndRotationMultiple", _rotateAndScaleMultiplierCurve.Evaluate(0));
            }

            while (time < _timeRotateBottle)
            {
                lerpValue = time / _timeRotateBottle;
                angleValue = Mathf.Lerp(_directionMultiplier * _rotationValueBottle[_rotationValueIndex], 0, lerpValue);

                transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                if (_numOfColorInBottle != 0)
                {
                    _bottleSR.material.SetFloat("_ScaleAndRotationMultiple", _rotateAndScaleMultiplierCurve.Evaluate(angleValue));
                }

                lastAngleValue = angleValue;
                time += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            angleValue = 0;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            _bottleSR.material.SetFloat("_ScaleAndRotationMultiple", _rotateAndScaleMultiplierCurve.Evaluate(angleValue));

            if (_numOfColorInBottle < _fillAmountBottle.Count && _numOfColorInBottle >= 0)
            {
                _bottleSR.material.SetFloat("_FillAmount", _fillAmountBottle[_numOfColorInBottle]);
            }

            SetTopColorValue();
            StartCoroutine(MoveBottleBack());
        }

        public IEnumerator MoveBottle()
        {
            _startPos = transform.position;

            if (_chosenRotationPoint == _leftRotationPoint)
            {
                _endPos = _bottleControllerRef.RightRotationPoint.position;
            }
            else if (_chosenRotationPoint == _rightRotationPoint)
            {
                _endPos = _bottleControllerRef.LeftRotationPoint.position;
            }

            float time = 0;
            float moveDuration = _timeRotateBottle / _moveSpeedMultiplier;

            while (time < moveDuration)
            {
                float t = time / moveDuration;
                transform.position = Vector3.Lerp(_startPos, _endPos, t);

                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            transform.position = _endPos;
            StartCoroutine(RotateBottleDown());
        }

        public IEnumerator MoveBottleBack()
        {
            _startPos = transform.position;
            _endPos = _originalPos;

            float time = 0;
            float moveDuration = _timeRotateBottle / _moveSpeedMultiplier;

            while (time < moveDuration)
            {
                float t = time / moveDuration;
                transform.position = Vector3.Lerp(_startPos, _endPos, t);

                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            transform.position = _endPos;
            transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
            _bottleSR.sortingOrder -= 2;

            ObserverManager<WaterSortEvent>.PostEvent(WaterSortEvent.OnTransferFinished, null);
        }

        public void SetTargetPoint()
        {
            if (_fillAmountBottle != null && _numOfColorInBottle >= 0 && _numOfColorInBottle < _fillAmountBottle.Count)
            {
                _waterTargetPoint.localPosition = new Vector3(_waterTargetPoint.localPosition.x, _fillAmountBottle[_numOfColorInBottle], _waterTargetPoint.localPosition.z);
            }
        }
    }
}