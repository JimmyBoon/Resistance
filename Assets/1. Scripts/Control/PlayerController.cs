using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Combat;
using RPG.Attributes;
using RPG.Inventories;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Mover mover;
        Fighter fighter;
        Health health;
        Animator animator;
        ActionStore actionStore;


        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        
        [SerializeField] float maxSpeed = 5.6f;
        [SerializeField] float injuredSpeed = 1.3f;

        float currentSpeed;

        bool isDraggingUI = false;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
            actionStore = GetComponent<ActionStore>();
        }


        void Update()
        {
            CheckInjuredStatus();

            if (InteractWithUI())
            {
                return;
            }

            if (!health.IsAlive())
            {
                SetCursor(CursorType.none);
                return;
            }

            CheckActionItemKey();

            if (InteractWithComponent())
            {
                return;
            }

            if (InteractWithMovement()) { return; }
            SetCursor(CursorType.none);

        }

        private void CheckActionItemKey()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(1,this.gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                actionStore.Use(2, this.gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                actionStore.Use(3, this.gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                actionStore.Use(4, this.gameObject);
            }
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        private bool InteractWithUI()
        {
            
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject()) //this returns true if it is over UI.
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.ui);
                return true;
            }

            if(isDraggingUI)
            {
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllHitsSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllHitsSorted()
        {

            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }


            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (!hasHit) { return false; }
            if (Input.GetMouseButton(0))
            {
                mover.StartMoveAction(target, currentSpeed);
            }
            SetCursor(CursorType.movement);

            return true;
        }

        private void CheckInjuredStatus()
        {
            if(health.IsInjured())
            {
                currentSpeed = injuredSpeed;
                animator.SetBool("injured", true);
            }
            else
            {
                currentSpeed = maxSpeed;
                animator.SetBool("injured", false);
            }
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit) { return false; }

            NavMeshHit navMeshHit;
            bool rayCastToNavMeshSample = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!rayCastToNavMeshSample)
            {

                return false;
            }
            target = navMeshHit.position;

            if(!mover.CanMoveTo(target))
            { return false; }

            return true;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping cursorMapping in cursorMappings)
            {
                if (cursorMapping.type == type)
                {
                    return cursorMapping;
                }
            }
            return cursorMappings[0];
        }

        //Needed for animation
        public void FootR()
        {

        }
        public void FootL()
        {
            
        }
        public void Land()
        {
        }
        public void WeaponSwitch()
        {
        }
    }
}
