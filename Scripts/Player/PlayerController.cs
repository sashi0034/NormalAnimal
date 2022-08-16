using System.Collections;
using System.Collections.Generic;
using MyProjects.Scripts.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyProjects.Scripts
{
    interface IPlayer
    {
        
    }

    public class PlayerController : MonoBehaviour, IPlayer
    {
        [SerializeField] private PlayerMove playerMove;

        private Vector3 _startedPosition;


        public readonly AnimHash HashWalk = new AnimHash("walk");
        public readonly AnimHash HashEat = new AnimHash("eat");
        public readonly AnimHash HashMotionSpeed = new AnimHash("motionSpeed");

        private Animator _animator;

        public Animator Animator => _animator;
        private Vector3 _pos => gameObject.transform.position;

        void Start()
        {
            _animator = GetComponent<Animator>();
            playerMove.Init(this);
        }

        void Update()
        {
            // if (Input.GetKey(KeyCode.Space))
            // {
            //     _animator.SetTrigger(HashEat.Code);
            //     Debug.Log("trigger eat.");
            // }

            playerMove.Update();
        }

    }
    
}