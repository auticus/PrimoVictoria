using System.Runtime.InteropServices.WindowsRuntime;
using PrimoVictoria.Models;
using UnityEngine;
using UnityEngine.AI;

namespace PrimoVictoria.Controllers
{
    /// <summary>
    /// Interface between the unit object on the master controller and each of the individual soldier meshes on the battlefielde
    /// This is attached to the character model prefab and requires an Animator and NavMeshAgent component on the prefab
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class UnitMiniatureController : MonoBehaviour
    {
        //https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
        //animator settings etc came from the above 
        private NavMeshAgent _agent;
        private Animator _animator;
        private Vector2 _smoothDeltaPosition = Vector2.zero;
        private Vector2 _velocity = Vector2.zero;

        //this is a bit of a circular reference
        public Stand Stand;
        
        public Unit ParentUnit => Stand?.ParentUnit;

        private bool _manualMoving;
        /// <summary>
        /// When TRUE - the gamepad or other input is moving this, not the nav agent
        /// </summary>
        public bool ManualMoving
        {
            get => _manualMoving;
            set
            {
                _manualMoving = value;
                _agent.isStopped = value || Destination == Vector3.zero;
            }
        }

        private Vector3 _destination;
        public Vector3 Destination
        {
            get => _destination;
            set
            {
                if (_destination != value)
                {
                    _destination = value;
                    _agent.destination = _destination;
                    _agent.isStopped = ManualMoving || value == Vector3.zero;
                }
            }
        }

        /// <summary>
        /// The nav agent speed
        /// </summary>
        public float Speed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        // Start is called before the first frame update
        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;  //don't update the position autmoatically we will do it in code
            _animator = GetComponent<Animator>();
        }

        protected void Update()
        {
            if (ManualMoving)
            {
                ManualMove();
            }
            else
            {
                UpdateAnimator();
            }
        }

        /// <summary>
        /// Callback for processing animation movements for modifying root motion
        /// This is invoked each frame after the state machines and the animations have been evaluated but before OnAnimatorIK
        /// </summary>
        private void OnAnimatorMove()
        {
            //update position based on animation movement using navigation surface height
            if (ManualMoving) return;
            transform.position = _agent.nextPosition;
        }

        private void ManualMove()
        {
            //this requires the manual movement of the mesh 

        }

        private void UpdateAnimator()
        {
            if (_animator == null) return;

            var worldDeltaPosition = _agent.nextPosition - transform.position;

            //map the worldDeltaPosition into local space
            //dot product is a float value equal to the magnitude (length) of the two vectors multiplied together and then multiplied by COS of the angle between them
            //for normalized vector, DOT returns 1 if they point in same direction, -1 if they point in opposite directions, and zero if they are perpendicular
            var dx = Vector3.Dot(transform.right, worldDeltaPosition);
            var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            var deltaPosition = new Vector2(dx, dy);

            var smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

            //update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                _velocity = _smoothDeltaPosition / Time.deltaTime; //not currently used, did not use the velocity tags as we don't have a blend tree to use

            var shouldMove = _velocity.magnitude > 0.5f && _agent.remainingDistance > 0;

            //update animation parameters
            _animator.SetBool("Move", shouldMove);
        }
    }
}
