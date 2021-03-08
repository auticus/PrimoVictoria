using UnityEngine;
using UnityEngine.AI;

namespace PrimoVictoria.Controllers
{
    /// <summary>
    /// Interface between the unit object on the master controller and each of the individual soldier meshes on the battlefielde
    /// This is attached to the character model prefab and requires an Animator and NavMeshAgent component on the prefab
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMeshController : MonoBehaviour
    {
        //https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
        //animator settings etc came from the above 
        public int UnitID;
        public float DefaultAcceleration = 8f;
        public float Deceleration = 60f;
        public float DecelerationDistance = 4f;  //the distance from target to cut deceleration

        protected NavMeshAgent Agent;
        protected Animator Animator;
        protected Vector2 SmoothDeltaPosition = Vector2.zero;
        protected Vector2 Velocity = Vector2.zero;

        protected Vector3 _destination;
        public Vector3 Destination
        {
            get { return _destination; }
            set
            {
                if (_destination != value)
                {
                    _destination = value;
                    Agent.destination = _destination;
                }
            }
        }

        /// <summary>
        /// The nav agent speed
        /// </summary>
        public float Speed
        {
            get
            {
                return Agent.speed;
            }
            set
            {
                Agent.speed = value;
            }
        }

        // Start is called before the first frame update
        protected void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.updatePosition = false;  //don't update the position autmoatically we will do it in code

            Animator = GetComponent<Animator>();
        }

        protected void Update()
        {
            var worldDeltaPosition = Agent.nextPosition - transform.position;

            //map the worldDeltaPosition into local space
            //dot product is a float value equal to the magnitude (length) of the two vectors multiplied together and then multiplied by COS of the angle between them
            //for normalized vector, DOT returns 1 if they point in same direction, -1 if they point in opposite directions, and zero if they are perpendicular
            var dx = Vector3.Dot(transform.right, worldDeltaPosition);
            var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            var deltaPosition = new Vector2(dx, dy);

            var smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

            //update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                Velocity = SmoothDeltaPosition / Time.deltaTime; //not currently used, did not use the velocity tags as we don't have a blend tree to use

            UpdateAnimator();
        }

        /// <summary>
        /// Callback for processing animation movements for modifying root motion
        /// This is invoked each frame after the state machines and the animations have been evaluated but before OnAnimatorIK
        /// </summary>
        private void OnAnimatorMove()
        {
            //update position based on animation movement using navigation surface height
            transform.position = Agent.nextPosition;
        }

        private void UpdateAnimator()
        {
            if (Animator == null) return;

            var shouldMove = Velocity.magnitude > 0.5f && Agent.remainingDistance > 0;

            //update animation parameters
            Animator.SetBool("Move", shouldMove);
        }
    }
}
