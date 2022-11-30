using System.Collections;
using UnityEngine;

namespace Helper
{
    public static class Suspension
    {
        public static void ApplySuspensionForce(Rigidbody _rb, RaycastHit _hit, float _springStrength = 100f, float _springDamper = 5f, float _suspensionRestDistance = 1f)
        {
            // Get velocity of rigidbody
            Vector3 velocity = _rb.velocity;
            // Get direction of suspension
            Vector3 suspensionDirection = _rb.transform.TransformDirection(_rb.transform.up);

            // Get the velocity of other rigidbody if raycast hit is hitting one
            Vector3 otherVelocity = Vector3.zero;
            Rigidbody hitBody = _hit.rigidbody;
            if (hitBody != null)
            {
                otherVelocity = hitBody.velocity;
            }

            // Get the direction of both rigidbody's velocity
            float suspensionDirectionVelocity = Vector3.Dot(velocity, suspensionDirection);
            float otherDirectionVelocity = Vector3.Dot(otherVelocity, suspensionDirection);

            float relativeVelocity = suspensionDirectionVelocity - otherDirectionVelocity;

            // Get the offset from rest distance
            float offset = _suspensionRestDistance - _hit.distance;

            // Calculate the force
            float springForce = (_springStrength * offset) - (_springDamper * relativeVelocity);

            _rb.AddForce(suspensionDirection * springForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(suspensionDirection * -springForce, _hit.point);
            }
        }

        public static bool CheckIfGrounded(Transform _feetTransform, Vector3 _downDirection, float _suspensionRestDistance, LayerMask _groundLayer)
        {
            if (Physics.Raycast(_feetTransform.position, _downDirection, out RaycastHit hit, _suspensionRestDistance, _groundLayer))
                return true;
            else
                return false;
        }
    }
}