using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace MyProjects.Scripts.Player
{
    [System.Serializable]
    public class PlayerMove
    {
        [SerializeField] private float cameraDistance;
        [SerializeField] private float cameraDeltaY;
        
        [SerializeField] private float accelForce;
        [SerializeField] private float maxHorizontalVelocity;

        [SerializeField] private float jumpForce;
        [SerializeField] private int maxJumpCount = 2;

        private PlayerController _owner;

        private Transform _ownerTransform;
        private Rigidbody _ownerRigidbody;
        private Vector3 _startedPosition;
        private Animator _ownerAnimator => _owner.Animator;
        private MainCamera camera => GameManager.Instance.MainCamera;
        private float _cameraHorizontalDeg;
        private GroundChecker _groundChecker;
        private int currJumpCount = 0;
        
        public void Init(PlayerController owner)
        {
            _owner = owner;
            _ownerTransform = owner.transform;
            _ownerRigidbody = owner.GetComponent<Rigidbody>();
            _startedPosition = owner.transform.position;
            _groundChecker = new GroundChecker(_ownerTransform, _owner.gameObject.GetComponent<BoxCollider>());
            
            resetCamera();
        }

        public void Update()
        {
            _groundChecker.UpdateChecking();
            changeTransform();
            rotateCameraByInput();
            checkJump();
        }
        
        private void resetCamera()
        {
            // camera.transform.rotation = _ownerTransform.rotation;
            var forward = _ownerTransform.forward;
            _cameraHorizontalDeg = Mathf.Atan2(forward.z, forward.x) * 180 / Mathf.PI;
            resetCameraDistance(_cameraHorizontalDeg);
        }

        private void resetCameraDistance(float forwardDeg)
        {
            camera.transform.rotation = Quaternion.Euler(new Vector3(0, 90 - forwardDeg, 0));

            var forwardRad = forwardDeg / 180 * Mathf.PI;
            camera.transform.position = _ownerTransform.position - cameraDistance * new Vector3(Mathf.Cos(forwardRad), 0, Mathf.Sin(forwardRad)) +
                                        new Vector3(0, cameraDeltaY, 0);
        }

        private void changeTransform()
        {
            var deltaVec = GetDeltaVecFromKey();
            if (deltaVec != Vector3.zero) rotateByDeltaVec(deltaVec);

            checkAnimWalkByVel();
            
            bool isDash = Input.GetKey(KeyCode.LeftShift);
            makeVelocityBelowMax(isDash);
            
            setMotionSpeed(isDash);

            _ownerRigidbody.AddForce(new Vector3(deltaVec.x, 0, deltaVec.z) * accelForce);
        }

        private void setMotionSpeed(bool isDash)
        {
            float speed = getDashSpeed(isDash);
            bool isRising = _ownerRigidbody.velocity.y > 1.0f;
            if (!_groundChecker.IsGrounded &&  isRising) speed *= 2;
            
            _ownerAnimator.SetFloat(_owner.HashMotionSpeed.Code, speed);
        }

        private void rotateByDeltaVec(Vector3 deltaVec)
        {
            var toRot = Quaternion.Euler(0, 90 - (Mathf.Atan2(deltaVec.z, deltaVec.x) * 180 / Mathf.PI), 0);
            float maxDegreesDelta = accelForce * 100 * Time.deltaTime;
            _ownerTransform.rotation = Quaternion.RotateTowards(_ownerTransform.rotation, toRot, maxDegreesDelta);
        }

        private void makeVelocityBelowMax(bool isDash)
        {
            var currVel = _ownerRigidbody.velocity;
            var dashSpeed = getDashSpeed(isDash);
            var maxVel = maxHorizontalVelocity * dashSpeed;
            
            if (new Vector3(currVel.x, 0, currVel.z).sqrMagnitude > maxVel)
            {
                _ownerRigidbody.velocity = new Vector3(currVel.x, 0, currVel.z).normalized * maxVel +
                                           new Vector3(0, currVel.y, 0);
            }
        }

        private static float getDashSpeed(bool isDash)
        {
            return isDash ? 2.0f : 1.0f;
        }

        private void checkAnimWalkByVel()
        {
            if (_ownerRigidbody.velocity.magnitude > 0.01f)
            {
                _ownerAnimator.SetBool(_owner.HashWalk.Code, true);
            }
            else
            {
                _ownerAnimator.SetBool(_owner.HashWalk.Code, false);
            }
        }

        private Vector3 GetDeltaVecFromKey()
        {
            float deltaX = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            float deltaZ = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);

            var deltaVec = getInputDeltaVec(deltaX, deltaZ);
            return deltaVec;
        }

        private Vector3 getInputDeltaVec(float deltaX, float deltaZ)
        {
            var cameraTransform = camera.transform;
            var deltaVec = cameraTransform.right * deltaX + cameraTransform.forward * deltaZ;
            return deltaVec;
        }

        private void rotateCameraByInput()
        {
            float deltaHorizontalDeg = 0;
            const float rotatingSpeed = 100f;
        
            if (Input.GetKey(KeyCode.LeftArrow)) deltaHorizontalDeg += rotatingSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.RightArrow)) deltaHorizontalDeg -= rotatingSpeed * Time.deltaTime;
            
            _cameraHorizontalDeg += deltaHorizontalDeg;
            resetCameraDistance(_cameraHorizontalDeg);
        }


        private void checkJump()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            bool isGrounded = _groundChecker.IsGrounded;
            if (!isGrounded && currJumpCount>=maxJumpCount) return;
            
            if (isGrounded) currJumpCount = 0;

            bool isFirstJump = currJumpCount == 0;
            Debug.Log(currJumpCount);
            _ownerRigidbody.AddForce(Vector3.up * jumpForce * (isFirstJump ? 1.0f : 0.5f));
            currJumpCount++;
        }



        [Button]
        public void ResetPosition()
        {
            _owner.gameObject.transform.position = _startedPosition;
        }
    }
}