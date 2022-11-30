using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public static class WallRun
    {
        public static bool CheckLeftWall(Vector3 _position, Vector3 _leftDirection, float _wallCheckDistance, LayerMask _wallRunnableLayers)
        {
            return Physics.Raycast(_position, _leftDirection, out RaycastHit hit, _wallCheckDistance, _wallRunnableLayers);
        }

        public static bool CheckRightWall(Vector3 _position, Vector3 _rightDirection, float _wallCheckDistance, LayerMask _wallRunnableLayers)
        {
            return Physics.Raycast(_position, _rightDirection, out RaycastHit hit, _wallCheckDistance, _wallRunnableLayers);
        }

        public static bool CheckIfHighEnoughOffGround(Vector3 _position, Vector3 _downDirection, float _heightOffGround, LayerMask _groundLayer)
        {
            if (Physics.Raycast(_position, _downDirection, out RaycastHit hit, _heightOffGround, _groundLayer))
                return false;
            else
                return true;
        }

        public static RaycastHit GetLeftWallHit(Vector3 _position, Vector3 _leftDirection, float _wallCheckDistance, LayerMask _wallRunnableLayers)
        {
            Physics.Raycast(_position, _leftDirection, out RaycastHit hit, _wallCheckDistance, _wallRunnableLayers);
            return hit;
        }

        public static RaycastHit GetRightWallHit(Vector3 _position, Vector3 _rightDirection, float _wallCheckDistance, LayerMask _wallRunnableLayers)
        {
            Physics.Raycast(_position, _rightDirection, out RaycastHit hit, _wallCheckDistance, _wallRunnableLayers);
            return hit;
        }
    }
}