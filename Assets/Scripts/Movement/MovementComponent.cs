using Core;
using Subsystems.Persistent;
using UnityEngine;

namespace Movement
{
    public class MovementComponent : MonoBehaviour
    {
        private enum MovementDirection
        {
            Left = -1,
            Idle = 0,
            Right = 1,
        }
        
        [SerializeField] 
        private bool drawDebugHandles;

        [SerializeField] 
        private bool disableTilting;

        [SerializeField] 
        private float movementRollAngle = 15;
        
        [SerializeField] 
        private float movementSpeed = 5f;
        
        [SerializeField] 
        private float blockDetectionDistance = 1f;

        #if UNITY_EDITOR
        private Color debugLineColor = Color.green;
        #endif
        
        private MovementDirection movementDirection = MovementDirection.Idle;
        private InputSubsystem inputSubsystem;

        private void Start()
        {
            inputSubsystem = GamePersistent.GetActiveWorld().GetSubsystem<InputSubsystem>();
        }

        void Update()
        {
            movementDirection = MovementDirection.Idle;
            if (!disableTilting)
            {
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y,
                    movementRollAngle * -inputSubsystem.InputContext.movementValue);
            }

            if (inputSubsystem.InputContext.movementValue == 0) 
                return;
        
            #if UNITY_EDITOR
            debugLineColor = Color.green;
            #endif
            
            movementDirection = (MovementDirection) Mathf.Sign(inputSubsystem.InputContext.movementValue);
            if (Physics.Raycast(transform.position, Vector3.right * inputSubsystem.InputContext.movementValue, blockDetectionDistance, LayerMask.GetMask("Blocker")))
            {
                #if UNITY_EDITOR
                debugLineColor = Color.red;
                #endif
                return;
            }
                
            transform.position += Vector3.right * (inputSubsystem.InputContext.movementValue * movementSpeed * Time.deltaTime);
        }

        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!drawDebugHandles)
                return;
            
            Gizmos.color = debugLineColor;
            if (movementDirection == MovementDirection.Left)
            {
                Gizmos.DrawLine(this.transform.position, this.transform.position - Vector3.right * blockDetectionDistance);
            }
            else if (movementDirection == MovementDirection.Right)
            {
                Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.right * blockDetectionDistance);
            }
            
            Gizmos.color = Color.white;
        }
        #endif
    }
}