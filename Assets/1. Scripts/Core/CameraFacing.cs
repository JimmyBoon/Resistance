using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {

        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}