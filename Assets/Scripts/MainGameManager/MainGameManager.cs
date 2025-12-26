using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainGameManager : Singleton<MainGameManager>
{
    [Header("UI Main Menu")]
    [SerializeField] private GameObject _uiMainMenu;
    [SerializeField] private GameObject _uiListGame;
    [SerializeField] private GameObject _settingPopup;
    [SerializeField] private GameObject _notificationUpdateGamePopup;
    [SerializeField] private Transform _loading;
    [SerializeField] private float _timeLoading = 0.35f;
    [SerializeField] private TextMeshProUGUI _numberText;

    [Header("UI In Game")]
    [SerializeField] private GameObject _uiInGame;
    [SerializeField] private GameObject _curScoreGObj;
    [SerializeField] private GameObject _highScoreGObj;
    [SerializeField] private TextMeshProUGUI _curScoreTxt;
    [SerializeField] private TextMeshProUGUI _highScoreTxt;
    [SerializeField] private Button _backMenuBtn;
    [SerializeField] private Button _replayBtn;
    
    private GameObject _currentGame;
    public GameObject CurrentGame => _currentGame;
    private IGameManager _curGameManager;
    private Coroutine _countDownCoroutine;
    private GameState _gameState;
    public GameState GameState => _gameState;

    private void Start()
    {
        Application.targetFrameRate = 120;
        _gameState = GameState.MainMenu;
    }

    private void DoTransition(Action changeGameScene, Action startGame = null)
    {
        _loading.localScale = Vector3.zero;
        _loading.gameObject.SetActive(true);

        _gameState = GameState.Loading;

        Sequence seq = DOTween.Sequence();

        seq.Append(_loading.DOScale(2f, _timeLoading).SetEase(Ease.OutQuad));

        seq.AppendCallback(() => {
            changeGameScene?.Invoke();
        });

        seq.Append(_loading.DOScale(0f, _timeLoading).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            _loading.gameObject.SetActive(false);
            if (startGame != null)
            {
                startGame.Invoke();
                _gameState = GameState.InGame;
            }
        });
        seq.SetUpdate(true);
    }

    public void OpenGame(GameObject game)
    {
        AudioManager.Instance.PlaySoundButtonClick();
        if (game == null)
        {
            OpenNotificationUpdateGame();
            return;
        } 

        DoTransition(() =>
        {
            _uiMainMenu.SetActive(false);
            _uiInGame.SetActive(true);

            _currentGame = game;
            _currentGame.SetActive(true);

            _curGameManager = _currentGame.GetComponentInChildren<IGameManager>();

            AudioManager.Instance.StopMusic();
        }, () => 
        {
            if (_curGameManager != null)
            {
                _curGameManager.StartNewGame();
            }
        });
            
    }

    public void Restart()
    {
        if (_curGameManager == null) return;

        DoTransition(() =>
        {
            _curGameManager.Restart();
        });
    }

    public void BackToMainMenu()
    {
        DoTransition(() =>
        {
            ShowScore(false);
            _uiInGame.SetActive(false);
            _uiMainMenu.SetActive(true);

            if (_currentGame != null)
            {
                _currentGame.SetActive(false);
            }

            _currentGame = null;
            _curGameManager = null;

            AudioManager.Instance.PlayMusicInMenu();
        });
        _gameState = GameState.MainMenu;
    }

    public void ShowScore(bool isShow)
    {
        if (_curScoreGObj != null) _curScoreGObj.SetActive(isShow);
        if (_highScoreGObj != null) _highScoreGObj.SetActive(isShow);
    }

    public void OpenSetting()
    {
        AudioManager.Instance.PlaySoundClick();
        _uiListGame.SetActive(false);
        _settingPopup.SetActive(true);
    }

    public void CloseSetting()
    {
        AudioManager.Instance.PlaySoundClick();
        _settingPopup.SetActive(false);
        _uiListGame.SetActive(true);
    }

    public void OpenNotificationUpdateGame()
    {
        AudioManager.Instance.PlaySoundClick();
        _notificationUpdateGamePopup.SetActive(true);
        _notificationUpdateGamePopup.transform.localScale = Vector3.zero;
        _notificationUpdateGamePopup.transform.DOScale(Vector3.one, _timeLoading).SetEase(Ease.OutBack);
    }

    public void CloseNotificationUpdateGame()
    {
        AudioManager.Instance.PlaySoundClick();
        _notificationUpdateGamePopup.transform.DOScale(Vector3.zero, _timeLoading).SetEase(Ease.InBack).OnComplete(() => _notificationUpdateGamePopup.SetActive(false));
    }

    public void CountDown()
    {
        Time.timeScale = 0f;
        _numberText.gameObject.SetActive(true);
        if (_countDownCoroutine != null) StopCoroutine(_countDownCoroutine);
        _countDownCoroutine = StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            _numberText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        _numberText.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateCurScore(int score)
    {
        if (_curScoreTxt != null) _curScoreTxt.text = score.ToString();
    }

    public void UpdateHighScore(int score)
    {
        if (_highScoreTxt != null) _highScoreTxt.text = score.ToString();
    }
}