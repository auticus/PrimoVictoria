using System.Collections.Generic;
using System;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Parameters;
using PrimoVictoria.Assets.Code.Models.Utilities;
using UnityEngine;
using PrimoVictoria.Controllers;
using PrimoVictoria.DataModels;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
        /// Based off of its current world position, the upper left point
        /// </summary>
        public Vector3 UpperLeftPoint => StandPosition.GetUpperPoint(this, StandPosition.StandPoint.UpperLeft);

        /// <summary>
        /// Based off of its current world position, the upper right point
        /// </summary>
        public Vector3 UpperRightPoint => StandPosition.GetUpperPoint(this, StandPosition.StandPoint.UpperRight);

        public EventHandler<StandLocationArgs> OnSendLocationData;

        /// <summary>
        /// The location that the stand needs to travel to or the direction it needs to take if manually moving
        /// </summary>
        public Vector3 Destination
        {
            get => _controller.Destination;
            set => _controller.Destination = value;
        }

        /// <summary>
        /// When not set to identity tells the stand to rotate in that direction its speed for that frame
        /// </summary>
        public Vector3 RotationDirection;

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

        private bool _visible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
        
        private UnitData _unitData; //data pertinent to the unit overall

        private UnitMiniatureController _controller;

        private void Update()
        {
            if (!DiagnosticsOn) return;

            var args = new StandLocationArgs(Transform.position);
            OnSendLocationData?.Invoke(this, args);
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
        
            UnitOffset = StandPosition.GetStandUnitOffset(pivotStand, parms.RankIndex, parms.FileIndex);
            var location = parms.Location + UnitOffset;

            AddMiniatureToStand(parms.Location, parms.Rotation);
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
           SelectModelMesh(projectors, isFriendly);
        }

        /// <summary>
        /// Moves the individual stand by setting its DestinationRotation and Destination properties (which will be captured by the StandController)
        /// </summary>
        /// <param name="rawDestinationPosition">the point on the table that the mouse was clicked and the pivot stand will move to.</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 rawDestinationPosition, bool isRunning)
        {
            //the destination position passed was the point on the table clicked
            //for the pivot mesh (the #1 stand) this should be where its front touches so will need offset from its middle to its front
            //for every other mesh - a calculation of the point will need done to calculate where they need to move
            ManualMoving = false;
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            RotationDirection = StandMovePosition.GetDirectionToLookAtPoint(rawDestinationPosition, this, RankIndex, FileIndex);
            Destination = StandMovePosition.GetStandMovePosition(rawDestinationPosition, this, RankIndex, FileIndex);
        }

        public void ManualMove(Vector3 direction, bool isRunning)
        {
            ManualMoving = true;
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            RotationDirection = Vector3.zero;
            Destination = direction;
        }

        public void StopManualMove()
        {
            ManualMoving = false;
            Destination = Vector3.zero;
            RotationDirection = Vector3.zero;
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
            RotationDirection = direction;

            //we're going to rotate around a point and see what that does.  like a boss. - pick a point on the stand and rotate AROUND that
            WheelPivotPoint = (direction == Vector3.right * -1) ? 
                ParentUnit.GetUnitWheelPoint(Unit.WheelPointIndices.Left_UpperLeft) : 
                ParentUnit.GetUnitWheelPoint(Unit.WheelPointIndices.Right_UpperRight);
            
            //todo: the inner part of the wheel doesnt move
            //the outer stands move the furthest
        }

        public void StopRotating()
        {
            RotationDirection = Vector3.zero;
            WheelPivotPoint = Vector3.zero;
        }

        /// <summary>
        /// Returns a new vector3 that is a copy of the current position
        /// </summary>
        /// <returns></returns>
        public Vector3 CopyPosition()
        {
            return new Vector3(Transform.position.x, Transform.position.y, Transform.position.z);
        }

        #region Private Methods

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectModelMesh(Projectors projectors, bool isFriendly)
        {
            //draw the projector prefab (the circle under the models) under the models
            var selectionObject = GetSelectionProjector(projectors, isFriendly);
            selectionObject.transform.SetParent(MiniatureMesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
        }

        private GameObject GetSelectionProjector(Projectors projectors, bool isFriendly)
        {
            var projectorPrefab = GetActiveProjectorPrefab(projectors, isFriendly: isFriendly);
            var selectionObject = Instantiate(projectorPrefab);
            var projector = selectionObject.GetComponentInChildren<Projector>();
            projector.orthographicSize = GetProjectorOrthoSize();

            return selectionObject;
        }

        /// <summary>
        /// Based on if the stand is visible and what the unit type is - return the size of the selection cursor
        /// </summary>
        /// <returns></returns>
        private float GetProjectorOrthoSize()
        {
            if (_visible && _unitData.UnitType == UnitTypes.Infantry)
                return (float)_unitData.SelectionInfantryStandOrthoSize;
            if (!_visible && _unitData.UnitType == UnitTypes.Infantry)
                return (float)_unitData.SelectionInfantryOrthoSize;

            Debug.LogError($"Stand::GetProjectorOrthoSize encountered a unit type that is not currently supported - {_unitData.UnitType}");
            throw new ArgumentException($"Encountered Unit Type is not supported - {_unitData.UnitType}");
        }

        private GameObject GetActiveProjectorPrefab(Projectors projector, bool isFriendly)
        {
            if (isFriendly && _visible) return projector.FriendlySquareSelection;
            if (isFriendly) return projector.FriendlyCircleSelection;
            if (!isFriendly && _visible) return projector.OtherSquareSelection;
            if (!isFriendly) return projector.OtherCircleSelection;

            throw new Exception($"Invalid data state in Stand::GetActiveProjector");
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
