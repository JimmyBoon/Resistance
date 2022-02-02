using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Control
{
    public class DoorOpener : MonoBehaviour, IRaycastable
    {
        
        [SerializeField] bool doorOpen = false;
        [SerializeField] float doorOpenAngle = 100f;
        [SerializeField] float doorMovementSpeed = 1f;
        CursorType cursorType = CursorType.Door;
        float closedDoorEurlerAngle;


        // Start is called before the first frame update
        void Start()
        {
            closedDoorEurlerAngle = transform.localEulerAngles.y;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ActivateDoor()
        {
            if(!doorOpen)
            {
                doorOpen = true;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + doorOpenAngle, transform.localEulerAngles.z);
            }
            else
            {
                doorOpen = false;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - doorOpenAngle, transform.localEulerAngles.z);
            }

        }

        public CursorType GetCursorType()
        {
            return cursorType;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                ActivateDoor();
            }
            
            return true;
        }

        public object CaptureState()
        {
            throw new System.NotImplementedException();
        }

        public void RestoreState(object state)
        {
            throw new System.NotImplementedException();
        }
    }
}
