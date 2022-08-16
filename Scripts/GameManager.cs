using System;
using UnityEngine;

namespace MyProjects.Scripts
{
    public class GameManager : MonoBehaviour
    { 
        public static GameManager Instance { get; private set; }

        private MainCamera _mainCamera;
        public MainCamera MainCamera => _mainCamera;

        public GameManager()
        {
            Instance = this;
        }

        public void Awake()
        {
            _mainCamera = Camera.main.GetComponent<MainCamera>();
        }
    }
}