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
        
        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            movementDirection = MovementDirection.Idle;

            if (!disableTilting)
            {
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y,
                    movementRollAngle * -horizontalInput);
            }

            if (horizontalInput == 0) 
                return;
        
            #if UNITY_EDITOR
            debugLineColor = Color.green;
            #endif
            
            movementDirection = (MovementDirection) Mathf.Sign(horizontalInput);
            if (Physics.Raycast(transform.position, Vector3.right * horizontalInput, blockDetectionDistance, LayerMask.GetMask("Blocker")))
            {
                #if UNITY_EDITOR
                debugLineColor = Color.red;
                #endif
                return;
            }
                
            transform.position += Vector3.right * (horizontalInput * movementSpeed * Time.deltaTime);
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