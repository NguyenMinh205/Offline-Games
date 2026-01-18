using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    public AudioSource MusicSource => _musicSource;

    [SerializeField] private AudioSource _soundSource;
    public AudioSource SoundSource => _soundSource;

    [Header("Music")]
    [SerializeField] private AudioClip _musicInMenu;

    [Header("General UI")]
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _loseSound;

    [Header("TicTacToe")]
    [SerializeField] private AudioClip _playerClick;
    [SerializeField] private AudioClip _enemyClick;
    [SerializeField] private AudioClip _finishGame;

    [Header("Memory Card")]
    [SerializeField] private List<AudioClip> _cardPlaceSounds;
    [SerializeField] private AudioClip _cardFlipSound;
    [SerializeField] private AudioClip _cardMatchSound;

    [Header("Sliding Puzzle")]
    [SerializeField] private AudioClip _tileMoveSound;

    [Header("NumberSlide")]
    [SerializeField] private AudioClip _numberSlideSound;
    [SerializeField] private AudioClip _numberMergeSound;

    [Header("Wordle")]
    [SerializeField] private AudioClip _wordleClickSound;
    [SerializeField] private AudioClip _wordleErrorSound;
    [SerializeField] private AudioClip _wordleSubmitSound;

    [Header("Minesweeper")]
    [SerializeField] private AudioClip _msTileClick;
    [SerializeField] private AudioClip _msTileExpand;
    [SerializeField] private AudioClip _msExplosionBomb;

    [Header("DOTRescue")]
    [SerializeField] private AudioClip _dotMoveSound;
    [SerializeField] private AudioClip _dotPointSound;
    [SerializeField] private AudioClip _dotColliderSound;

    [Header("Sudoku")]
    [SerializeField] private AudioClip _sudokuCellClick;
    [SerializeField] private AudioClip _sudokuNumberCorrectSound;
    [SerializeField] private AudioClip _sudokuNumberWrongSound;

    [Header("Color Block")]
    [SerializeField] private AudioClip _colorBlockSetSound;
    [SerializeField] private AudioClip _colorBlockClearSound;

    [Header("Fruit Merge")]
    [SerializeField] private AudioClip _fruitSpawnSound;
    [SerializeField] private AudioClip _fruitDropSound;
    [SerializeField] private AudioClip _fruitMergeSound;

    [Header("Tetris")]
    [SerializeField] private AudioClip _tetrisDropSound;
    [SerializeField] private AudioClip _tetrisClearRowSound;

    [Header("Color Connect")]
    [SerializeField] private AudioClip _colorConnectLineSound;

    [Header("WaterSort")]
    [SerializeField] private AudioClip _selectedBottle;
    [SerializeField] private AudioClip _unselectedBottle;
    [SerializeField] private AudioClip _waterSortPourSound;
    [SerializeField] private AudioClip _waterSortCompleteBottleSound;

    protected override void Awake()
    {
        base.KeepActive(true);
        base.Awake();

        if (_musicSource == null || _soundSource == null)
        {
            Debug.LogError("AudioManager: AudioSource chưa được gán trong Inspector!");
        }
    }

    private void Start()
    {
        SetMusicVolume(DataManager.Instance.GameData.MusicVolume);
        SetSoundVolume(DataManager.Instance.GameData.SoundVolume);
        PlayMusicInMenu();
    }

    public void SetMusicVolume(float volume)
    {
        if (_musicSource != null)
        {
            _musicSource.volume = volume;
            if (DataManager.Instance != null && DataManager.Instance.GameData != null)
            {
                DataManager.Instance.GameData.MusicVolume = volume;
            }
        }
    }

    public void SetSoundVolume(float volume)
    {
        if (_soundSource != null)
        {
            _soundSource.volume = volume;
            if (DataManager.Instance != null && DataManager.Instance.GameData != null)
            {
                DataManager.Instance.GameData.SoundVolume = volume;
            }
        }
    }

    public void PlayMusicInMenu()
    {
        if (_musicInMenu != null)
        {
            PlayMusicGame(_musicInMenu);
        }
    }

    public void PlayMusicGame(AudioClip clip)
    {
        if (_musicSource == null || clip == null) return;

        if (_musicSource.isPlaying && _musicSource.clip == clip) return;

        _musicSource.loop = true;
        _musicSource.clip = clip;
        _musicSource.volume = 0f;
        _musicSource.Play();
        _musicSource.DOFade(DataManager.Instance.GameData.MusicVolume, 0.5f).SetUpdate(true);
    }

    public void StopMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _musicSource.Stop();
            }).SetUpdate(true);
        }
    }

    public void PlaySFX(AudioClip sound, bool repeat = false)
    {
        if (sound == null || _soundSource == null) return;

        if (repeat)
        {
            _soundSource.loop = true;
            _soundSource.clip = sound;
            _soundSource.Play();
        }
        else
        {
            _soundSource.loop = false;
            _soundSource.PlayOneShot(sound, _soundSource.volume);
        }
    }

    public void StopSFX()
    {
        if (_soundSource != null && _soundSource.isPlaying)
        {
            _soundSource.Stop();
            _soundSource.loop = false;
            _soundSource.clip = null;
        }
    }

    #region Specific Game Play Methods

    // --- General UI ---
    public void PlaySoundButtonClick() => PlaySFX(_buttonClick);
    public void PlaySoundClick() => PlaySFX(_click);
    public void PlaySoundWin() => PlaySFX(_winSound);
    public void PlaySoundLose() => PlaySFX(_loseSound);

    // --- TicTacToe ---
    public void PlayTicTacToePlayerClick() => PlaySFX(_playerClick);
    public void PlayTicTacToeEnemyClick() => PlaySFX(_enemyClick);
    public void PlayTicTacToeFinishGame() => PlaySFX(_finishGame);

    // --- Memory Card ---
    public void PlayCardPlaceSound()
    {
        if (_cardPlaceSounds != null && _cardPlaceSounds.Count > 0)
        {
            AudioClip randomClip = _cardPlaceSounds[Random.Range(0, _cardPlaceSounds.Count)];
            PlaySFX(randomClip);
        }
    }
    public void PlayCardFlipSound() => PlaySFX(_cardFlipSound);
    public void PlayCardMatchSound() => PlaySFX(_cardMatchSound);

    // --- Sliding Puzzle ---
    public void PlaySlidingTileMoveSound() => PlaySFX(_tileMoveSound);

    // --- NumberSlide ---
    public void PlayNumberSlideSound() => PlaySFX(_numberSlideSound);
    public void PlayNumberMergeSound() => PlaySFX(_numberMergeSound);

    // --- Wordle ---
    public void PlayWordleClickSound() => PlaySFX(_wordleClickSound);
    public void PlayWordleErrorSound() => PlaySFX(_wordleErrorSound);
    public void PlayWordleSubmitSound() => PlaySFX(_wordleSubmitSound);

    // --- Minesweeper ---
    public void PlayMinesweeperTileClick() => PlaySFX(_msTileClick);
    public void PlayMinesweeperTileExpand() => PlaySFX(_msTileExpand);
    public void PlayMinesweeperExplosion() => PlaySFX(_msExplosionBomb);

    // --- DOTRescue ---
    public void PlayDotMoveSound() => PlaySFX(_dotMoveSound);
    public void PlayDotPointSound() => PlaySFX(_dotPointSound);
    public void PlayDotColliderSound() => PlaySFX(_dotColliderSound);

    // --- Sudoku ---
    public void PlaySudokuCellClickSound() => PlaySFX(_sudokuCellClick);
    public void PlaySudokuNumberCorrectSound() => PlaySFX(_sudokuNumberCorrectSound);
    public void PlaySudokuNumberWrongSound() => PlaySFX(_sudokuNumberWrongSound);

    // --- Color Block ---
    public void PlayColorBlockPlaceSound() => PlaySFX(_colorBlockSetSound);
    public void PlayColorBlockClearSound() => PlaySFX(_colorBlockClearSound);

    // --- Fruit Merge ---
    public void PlayFruitSpawnSound() => PlaySFX(_fruitSpawnSound);
    public void PlayFruitDropSound() => PlaySFX(_fruitDropSound);
    public void PlayFruitMergeSound() => PlaySFX(_fruitMergeSound);

    // --- Tetris ---
    public void PlayTetrisDropSound() => PlaySFX(_tetrisDropSound);
    public void PlayTetrisClearRowSound() => PlaySFX(_tetrisClearRowSound);

    // --- Color Connect ---
    public void PlayColorConnectLineSound() => PlaySFX(_colorConnectLineSound);

    // --- WaterSort ---
    public void PlayWaterSortSelectedBottleSound() => PlaySFX(_selectedBottle);
    public void PlayWaterSortUnselectedBottleSound() => PlaySFX(_unselectedBottle);
    public void PlayWaterSortPouringSound() => PlaySFX(_waterSortPourSound);
    public void PlayWaterSortCompleteBottleSound() => PlaySFX(_waterSortCompleteBottleSound);
    #endregion
}