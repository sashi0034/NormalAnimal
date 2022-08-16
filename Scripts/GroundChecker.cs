using UnityEngine;

namespace MyProjects.Scripts.Player
{
    public class GroundChecker
    {
        private readonly Transform _ownerTransform;


        public const string GroundTag = "Ground";

        public bool IsGrounded { get; private set; } = false;

        private readonly float _localVerticalRayStart;
        private readonly float _verticalRayLength;
        
        
        public GroundChecker(Transform ownerTransform, BoxCollider ownerCollider)
        {
            Debug.Assert(ownerCollider!=null);

            _ownerTransform = ownerTransform;

            _localVerticalRayStart = ownerCollider.center.y;
            _verticalRayLength = ownerCollider.size.y;
        }

        public void UpdateChecking()
        {
            IsGrounded =
                isGrounded(out var hit) &&
                hit.collider.CompareTag(GroundTag);
        }
        
        
        private bool isGrounded(out RaycastHit hit)
        {
            var rayStartPos = _ownerTransform.position + Vector3.up * _localVerticalRayStart;
            var ray = new Ray(rayStartPos, _ownerTransform.up * -1);
            float rayDistance = _verticalRayLength;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance);

            return (Physics.Raycast(ray, out hit, rayDistance));
        }
    }
}