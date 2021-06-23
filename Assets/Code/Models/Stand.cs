using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using PrimoVictoria.Controllers;
using PrimoVictoria.Core;
using PrimoVictoria.Core.Events;
using PrimoVictoria.DataModels;
using PrimoVictoria.Models.Parameters;
using PrimoVictoria.Utilities;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Stands represent the trays on which a model stands.  They hold X number of models
    /// </summary>
    public class Stand : MonoBehaviour
    {
        public Unit ParentUnit;
        public Guid Id;

        /// <summary>
        /// The default angle rotation to come within on rotations
        /// </summary>
        public const float ROTATION_THRESHOLD = 12.0f;

        /// <summary>
        /// The mesh of the miniature itself
        /// </summary>
        public GameObject MiniatureMesh { get; set; }

        /// <summary>
        /// The transform of the pivot mesh itself, (not the Stand::GameObject)
        /// </summary>
        public Transform Transform => MiniatureMesh.transform;

        /// <summary>
        /// What position in the unit footprint does this occupy (refer to unit documentation)
        /// </summary>
        public int FileIndex;

        /// <summary>
        /// What row in the unit does this occupy (refer to unit documentation)
        /// </summary>
        public int RankIndex;

        /// <summary>
        /// Based off of its location in the unit, the offset from the location passed that the stand actually occupies
        /// </summary>
        public Vector3 UnitOffset;

        /// <summary>
        /// Based off of its current world position, the upper left point of the base
        /// </summary>
        public Vector3 UpperLeftPoint => RankAndFilePosition.GetUpperPoint(this, RankAndFilePosition.StandPoint.UpperLeft);

        /// <summary>
        /// Based off of its current world position, the upper right point of the base
        /// </summary>
        public Vector3 UpperRightPoint => RankAndFilePosition.GetUpperPoint(this, RankAndFilePosition.StandPoint.UpperRight);

        

        /// <summary>
        /// The location that the stand needs to travel to or the direction it needs to take if manually moving
        /// </summary>
        public Vector3 Destination
        {
            get => _controller.Destination;
            set => _controller.Destination = value;
        }
        
        /// <summary>
        /// When FALSE, is not manual movement - user clicked somewhere and unit is moving there, otherwise Manually Moving from input
        /// </summary>
        public bool ManualMoving;

        /// <summary>
        /// The stand's current rotation
        /// </summary>
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// if TRUE means the stand is facing the Destination point (if one has been set)
        /// </summary>
        public bool IsFacingDestination => Destination != Vector3.zero && LookDirectionToDestinationAngle < ROTATION_THRESHOLD;
        private float LookDirectionToDestinationAngle => Vector3.Angle(Transform.forward, Destination - Transform.position);

        /// <summary>
        /// How fast the stand moves
        /// </summary>
        public float Speed;

        public float WheelSpeed => _unitData.WheelSpeed;
        public Vector3 WheelPivotPoint;

        public bool DiagnosticsOn;

        private bool _visible; //whether or not the miniature is visible or not  
        
        private UnitData _unitData; //data pertinent to the unit overall

        private UnitMiniatureController _controller;

        private void Update()
        {
            if (!DiagnosticsOn) return;

            var args = new StandLocationArgs(Transform.position);
            EventManager.Publish(PrimoEvents.SelectedUnitLocationChanged, args);
        }

        public void InitializeStand(StandInitializationParameters parms)
        {
            Id = Guid.NewGuid();
            ParentUnit = parms.Parent;
            _unitData = parms.Data;
            _visible = parms.StandVisible;
            FileIndex = parms.FileIndex;
            RankIndex = parms.RankIndex;

            var pivotStand = ParentUnit.GetPivotStand();
            if (pivotStand == null)
            {
                pivotStand = this; //the first time through there will be no pivot stand, which means THIS is the pivot stand
            }
        
            UnitOffset = RankAndFilePosition.GetPosition(pivotStand, parms.RankIndex, parms.FileIndex, parms.Spacing);
            var location = parms.Location + UnitOffset;

            AddMiniatureToStand(location, parms.Rotation);
        }

        public bool GetVisibility()
        {
            return _visible;
        }

        public void SetVisibility(bool value)
        {
            _visible = value;
        }

        public void Select(Projectors projectors, bool isFriendly)
        {
           SelectModelMesh(projectors, isFriendly, ghost: false);
        }

        public void GhostSelect(Projectors projectors, bool isFriendly)
        {
            SelectModelMesh(projectors, isFriendly, ghost: true);
        }

        /// <summary>
        /// Moves the individual stand by setting its DestinationRotation and Destination properties (which will be captured by the StandController)
        /// </summary>
        /// <param name="rawDestinationPosition">the point on the table that the mouse was clicked and the pivot stand will move to.</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 rawDestinationPosition, bool isRunning, UnitFormation formation)
        {
            //the destination position passed was the point on the table clicked
            //for the pivot mesh (the #1 stand) this should be where its front touches so will need offset from its middle to its front
            //for every other mesh - a calculation of the point will need done to calculate where they need to move
            ManualMoving = false;
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            Destination = RankAndFilePosition.CalculateMovePosition(rawDestinationPosition, this, RankIndex, FileIndex, formation.Spacing);
        }

        public void ManualMove(Vector3 direction, bool isRunning)
        {
            ManualMoving = true;
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            Destination = direction;
        }

        public void StopManualMove()
        {
            ManualMoving = false;
            Destination = Vector3.zero;
        }

        /// <summary>
        /// Wheels a stand that is part of a unit
        /// </summary>
        /// <param name="direction">The direction to wheel the unit</param>
        /// <param name="isRunning"></param>
        public void Wheel(Vector3 direction, bool isRunning)
        {
            //rotation in a wheel - the stands all will have the same facing 
            ManualMoving = true;
            Destination = Vector3.zero; 
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;

            //we're going to rotate around a point and see what that does.  like a boss. - pick a point on the stand and rotate AROUND that
            WheelPivotPoint = (direction == Vector3.right * -1) ? 
                ParentUnit.GetUnitWheelPoint(Unit.WheelPointIndices.Left_UpperLeft) : 
                ParentUnit.GetUnitWheelPoint(Unit.WheelPointIndices.Right_UpperRight);
            
            //todo: the inner part of the wheel doesnt move
            //the outer stands move the furthest
        }

        public void StopRotating()
        {
            WheelPivotPoint = Vector3.zero;
        }

        #region Private Methods

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectModelMesh(Projectors projectors, bool isFriendly, bool ghost)
        {
            //draw the projector prefab (the circle under the models) under the models
            var selectionObject = GetSelectionProjector(projectors, isFriendly, ghost);
            if (selectionObject == null) return;

            selectionObject.transform.SetParent(MiniatureMesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
        }

        private GameObject GetSelectionProjector(Projectors projectors, bool isFriendly, bool ghost)
        {
            var projectorPrefab = GetActiveProjectorPrefab(projectors, isFriendly: isFriendly);

            if (projectorPrefab == null) return null;

            var selectionObject = Instantiate(projectorPrefab);
            var projector = selectionObject.GetComponentInChildren<Projector>();
            projector.orthographicSize = GetProjectorOrthoSize();

            //set the transparency depending on if we're ghosting or not
            var falloff = ghost ? projectors.GhostSelectionTransparency : projectors.FullSelectionTransparency; //transparency value
            var color = projector.material.color;
            projector.material.color = new Color(color.r, color.g, color.b, falloff);

            return selectionObject;
        }

        /// <summary>
        /// Based on if the stand is visible and what the unit type is - return the size of the selection cursor
        /// </summary>
        /// <returns></returns>
        private float GetProjectorOrthoSize()
        {
            if (_visible)
                return _controller.AgentSize;

            return 0;
        }

        private GameObject GetActiveProjectorPrefab(Projectors projector, bool isFriendly)
        {
            if (_visible == false) return null;
            if (isFriendly) return projector.FriendlySelection;

            return projector.EnemySelection;
        }
        
        private void AddMiniatureToStand(Vector3 location, Vector3 rotation)
        {
            var miniatureMesh = Instantiate(original: _unitData.UnitMeshes[0], position: location, rotation: Quaternion.Euler(rotation));
            
            SetMiniature(miniatureMesh);
        }

        private void SetMiniature(GameObject miniatureMesh)
        {
            _controller = miniatureMesh.GetComponent<UnitMiniatureController>();
            _controller.Stand = this;
            miniatureMesh.transform.SetParent(ParentUnit.transform, worldPositionStays: false);
            MiniatureMesh = miniatureMesh;
        }

        #endregion Private Methods
    }
}
