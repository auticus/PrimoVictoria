﻿using System.Collections.Generic;
using System;
using System.Linq;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Parameters;
using PrimoVictoria.Assets.Code.Models.Utilities;
using UnityEngine;
using PrimoVictoria.Controllers;
using PrimoVictoria.DataModels;
using Debug = UnityEngine.Debug;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Stands represent the trays on which a model stands.  They hold X number of models
    /// </summary>
    public class Stand : MonoBehaviour
    {
        public Unit ParentUnit;
        public Guid Id;
        public List<Miniature> Miniatures; //the warrior meshes and their controller
        public StandController StandController; //the stand's controller, part of the StandMesh but pulled out on initialization for performance reasons

        public int StandCapacity;  //how many models fit on the stand at full capacity

        /// <summary>
        /// The default threshold to come within
        /// </summary>
        public const float MOVE_THRESHOLD = 0.5f;

        /// <summary>
        /// The default angle rotation to come within on rotations
        /// </summary>
        public const float ROTATION_THRESHOLD = 12.0f;

        public const float WHEEL_DEGREES = 10.0f;

        
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

        public EventHandler<StandLocationArgs> OnLocationChanged;

        /// <summary>
        /// The size of the object's dimensions
        /// </summary>
        public Vector3 MeshScale => Transform.localScale;

        /// <summary>
        /// The distance between the Stand's current location and its final destination
        /// </summary>
        public float MoveDistance => Vector3.Distance(Destination, Transform.position);


        /// <summary>
        /// The location that the stand needs to travel to
        /// </summary>
        public Vector3 Destination;

        /// <summary>
        /// When not set to identity tells the stand to rotate in that direction its speed for that frame
        /// </summary>
        public Vector3 RotationDirection;

        /// <summary>
        /// When TRUE, is not manual movement - user clicked somewhere and unit is moving there
        /// </summary>
        public bool AutoMoving;

        /// <summary>
        /// The stand's current rotation
        /// </summary>
        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// The stand's current position
        /// </summary>
        public Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <summary>
        /// if TRUE means the stand is facing the Destination point (if one has been set)
        /// </summary>
        public bool IsFacingDestination => Destination != Vector3.zero && LookDirectionToDestinationAngle < ROTATION_THRESHOLD;
        private float LookDirectionToDestinationAngle => Vector3.Angle(Transform.forward, Destination - Transform.position);

        /// <summary>
        /// Flag that indicates if the Stand should perform a movement to reach its target destination
        /// </summary>
        /// <returns></returns>
        public bool ShouldMove => Destination != Vector3.zero && MoveDistance > MOVE_THRESHOLD;

        /// <summary>
        /// Returns TRUE if RotationDirection is not a zero vector
        /// </summary>
        public bool ShouldRotate => RotationDirection != Vector3.zero;
        
        /// <summary>
        /// The transform of the pivot mesh itself, (not the Stand::GameObject)
        /// </summary>
        public Transform Transform => _mesh.transform;

        /// <summary>
        /// How fast the stand moves
        /// </summary>
        public float Speed;

        public float WheelSpeed => _unitData.WheelSpeed;

        private GameObject _mesh; //the mesh that is the stand that the models are standing on
        private bool _visible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
        
        private bool _modelsVisible;  //if true will draw the model meshes on the stand.  If false, will only draw the stand
        private UnitData _unitData; //data pertinent to the unit overall
        private List<StandSocket> _modelSockets;

        public void InitializeStand(StandInitializationParameters parms)
        {
            Id = Guid.NewGuid();
            ParentUnit = parms.Parent;
            _unitData = parms.Data;
            _visible = parms.StandVisible;
            _modelsVisible = parms.ModelMeshesVisible;
            FileIndex = parms.FileIndex;
            RankIndex = parms.RankIndex;

            if (Miniatures == null) Miniatures = new List<Miniature>();
            Miniatures.Clear();

            var pivotStand = ParentUnit.GetPivotStand();
            if (pivotStand == null)
            {
                pivotStand = this; //the first time through there will be no pivot stand, which means THIS is the pivot stand
                
            }
        
            UnitOffset = StandPosition.GetStandUnitOffset(pivotStand, parms.RankIndex, parms.FileIndex);
            var location = parms.Location + UnitOffset;

            InitializeStandMesh(ParentUnit, parms.Rotation, location, _visible);
            CreateStandSockets();
            AddMiniaturesToStand(parms.Location, parms.Rotation);
        }

        public bool GetVisibility()
        {
            return _visible;
        }

        public void SetVisibility(bool value)
        {
            _visible = value;
        }

        public bool GetModelVisibility()
        {
            return _modelsVisible;
        }

        public void SetModelVisibility(bool value)
        {
            _modelsVisible = value;
        }

        public Vector3 TransformPoint(Vector3 standPosition)
        {
            return _mesh.transform.TransformPoint(standPosition); //weird offset of the model
        }

        public void Select(Projectors projectors, bool isFriendly)
        {
            if (_visible) SelectStand(projectors, isFriendly);
            else SelectModelMeshes(projectors, isFriendly);
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
            AutoMoving = true;
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            RotationDirection = StandMovePosition.GetDirectionToLookAtPoint(rawDestinationPosition, this, RankIndex, FileIndex);
            Destination = StandMovePosition.GetStandMovePosition(rawDestinationPosition, this, RankIndex, FileIndex);
        }

        /// <summary>
        /// Wheels a stand that is part of a unit
        /// </summary>
        /// <param name="direction">The direction to wheel the unit</param>
        /// <param name="isRunning"></param>
        public void Wheel(Vector3 direction, bool isRunning)
        {
            //rotation in a wheel - the stands all will have the same facing 
            AutoMoving = false;
            Destination = Vector3.zero; 
            Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            RotationDirection = direction;

            //todo: the inner part of the wheel doesnt move
            //the outer stands move the furthest
        }

        public void StopRotating()
        {
            RotationDirection = Vector3.zero;
        }

        #region Private Methods

        private void Update()
        {
            var args = new StandLocationArgs(_mesh.transform.position, _modelSockets.Select(socket => socket.StandPosition).ToArray());
            OnLocationChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Draws selection projector around the entire stand
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectStand(Projectors projectors, bool isFriendly)
        {
            var selectionObject = GetSelectionProjector(projectors, isFriendly);
            selectionObject.transform.SetParent(_mesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the projector goes
        }

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectModelMeshes(Projectors projectors, bool isFriendly)
        {
            //draw the projector prefab (the circle under the models) under the models
            foreach (var mini in Miniatures)
            {
                var selectionObject = GetSelectionProjector(projectors, isFriendly);
                selectionObject.transform.SetParent(mini.MiniatureMesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
            }
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

        private void InitializeStandMesh(Unit parent, Vector3 rotation, Vector3 location, bool visible)
        {
            _mesh = Instantiate(original: _unitData.StandMesh, position: location, rotation: Quaternion.Euler(rotation));
            
            _mesh.transform.SetParent(parent.transform);
            _mesh.GetComponent<MeshRenderer>().enabled = visible;

            StandController = _mesh.GetComponent<StandController>();
            StandController.ParentUnit = parent;
            StandController.Stand = this;
        }

        private void CreateStandSockets()
        {
            _modelSockets = StandSocketFactory.CreateStandSocketsForStand(_mesh, ParentUnit.Data);

            if (_modelSockets == null) Debug.LogWarning($"Stand for unit {_unitData.Name} failed to create modelSockets");
        }

        private void AddMiniaturesToStand(Vector3 location, Vector3 rotation)
        {
            for (var i = 0; i < ParentUnit.Data.ModelsPerStand; i++)
            {
                //todo: first guy in will likely be a standard bearer mesh of some kind so not everything will just use the UnitMesh object here
                //this will likely take the form of an enum or something explaining each element in the UnitMeshes list and what it really is
                var miniatureMesh = Instantiate(original: _unitData.UnitMeshes[0], position: location, rotation: Quaternion.Euler(rotation));
                SetMiniature(miniatureMesh);
            }
        }

        private void SetMiniature(GameObject miniatureMesh)
        {
            var miniController = miniatureMesh.GetComponent<UnitMeshController>();
            var mini = new Miniature(miniatureMesh, miniController, this);
            Miniatures.Add(mini);
            AssignEmptySocket(mini);

            if (mini.Socket == null)
            {
                Debug.LogWarning("Mini has no empty stand socket to be placed in!");
                return;
            }

            miniController.ParentStand = this;
            miniController.Socket = mini.Socket;
            miniatureMesh.transform.SetParent(ParentUnit.transform, worldPositionStays: false);
            miniatureMesh.SetActive(_modelsVisible);
        }

        private void AssignEmptySocket(Miniature mini)
        {
            var socket = _modelSockets.FirstOrDefault(p => p.Occupied == false);
            if (socket == null) return;
            mini.Socket = socket;
            socket.Occupied = true;
        }

        #endregion Private Methods
    }
}
