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
    public class UnitMeshController : MonoBehaviour
    {
        //https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
        //animator settings etc came from the above 
        public int UnitID => ParentStand.ParentUnit.ID;

        public StandSocket Socket; //socket on the stand that this should stand on
        public Stand ParentStand; //the overall stand that this miniature belongs to

        private NavMeshAgent Agent;
        private Animator Animator;
        private Vector2 SmoothDeltaPosition = Vector2.zero;
        private Vector2 Velocity = Vector2.zero;

        private readonly float _movementThreshold = 0.9f; //when distance remaining <= this, stop moving

        // Start is called before the first frame update
        protected void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.updatePosition = false;  //don't update the position autmoatically we will do it in code

            Animator = GetComponent<Animator>();
        }

        protected void Update()
        {
            UpdateAnimator();
        }
        private void LateUpdate()
        {
            //updating position here because the stand does a lateupdate to adjust its height and if you update the position in update,
            //the meshes may pop like popcorn up in the air while moving
            UpdatePosition();
        }

        /// <summary>
        /// Callback for processing animation movements for modifying root motion
        /// This is invoked each frame after the state machines and the animations have been evaluated but before OnAnimatorIK
        /// </summary>
        private void OnAnimatorMove()
        {
            //update position based on animation movement using navigation surface height
            //transform.position = Agent.nextPosition;
        }

        private void UpdateAnimator()
        {
            //todo: if not using navmesh agent this will need manual work to set the animations
            //todo: add mechanism in here to keep track of remaining distance (since not using agent) like a Destination vector you can calculate off of
            if (Animator == null) return;

            //var shouldMove = Velocity.magnitude > 0.5f && Agent.remainingDistance > 0;
            
            //update animation parameters
            Animator.SetBool("Move", ParentStand.ShouldMove);
        }

        private void UpdatePosition()
        {
            transform.rotation = ParentStand.Rotation;
            transform.position = ParentStand.Transform.position;
        }
    }
}
