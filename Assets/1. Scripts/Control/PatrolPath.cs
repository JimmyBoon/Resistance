using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    
    public class PatrolPath : MonoBehaviour
    {
        
        [SerializeField] Vector3 waypointSize = new Vector3(1,1,1);
        [SerializeField] Color waypointColor = Color.red;
        
        
        private void OnDrawGizmos() 
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.color = waypointColor;

                Gizmos.DrawCube(GetWaypointPosition(i), waypointSize);
                Gizmos.DrawLine(GetWaypointPosition(i), GetWaypointPosition(j));
            }

        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWaypointPosition(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}