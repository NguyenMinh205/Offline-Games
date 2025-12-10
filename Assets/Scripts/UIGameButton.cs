using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameButton : MonoBehaviour
{
    [SerializeField] private GameObject _game;
    [SerializeField] private Color _backgroundColor;
    public GameObject Game
    {
        get { return _game; }
        set { _game = value; }
    }    

    public Color BackgroundColor
    {
        get { return _backgroundColor; }
        set { _backgroundColor = value; }
    }
}
