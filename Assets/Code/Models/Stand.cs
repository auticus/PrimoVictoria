using System.Collections.Generic;
using System;
using System.Linq;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Utilities;
using UnityEngine;
using PrimoVictoria.Controllers;
using PrimoVictoria.DataModels;
using Unity.UNetWeaver;

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

        public EventHandler<StandLocationArgs> OnLocationChanged;

        private GameObject _mesh; //the mesh that is the stand that the models are standing on
        private bool _visible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
                                //note: i'd much rather use properties here and getters and setters but thats not currently the unity way

        private bool _modelsVisible;  //if true will draw the model meshes on the stand.  If false, will only draw the stand
        private UnitData _unitData; //data pertinent to the unit overall
        private List<StandSocket> _modelSockets;

        public void InitializeStand(Unit parent, UnitData data, Vector3 location, Vector3 rotation, bool visible, bool modelsVisible)
        {
            Id = Guid.NewGuid();
            ParentUnit = parent;
            _unitData = data;
            _visible = visible;
            _modelsVisible = modelsVisible;

            if (Miniatures == null)
                Miniatures = new List<Miniature>();

            Miniatures.Clear();

            InitializeStandMesh(parent, rotation, location, visible);
            CreateStandSockets();
            AddMiniaturesToStand(location, rotation);
        }

        /// <summary>
        /// The transform of the actual mesh that represents the stand
        /// </summary>
        public Transform MeshTransform => _mesh.transform;

        /// <summary>
        /// The distance between the Stand's current location and its final destination
        /// </summary>
        public float MoveDistance => Vector3.Distance(StandController.Destination, _mesh.transform.position);

        public Vector3 Destination => StandController.Destination;

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

        public void Select(Projectors projectors, bool isFriendly)
        {
            if (_visible) SelectStand(projectors, isFriendly);
            else SelectModelMeshes(projectors, isFriendly);
        }

        /// <summary>
        /// Moves the individual stand
        /// </summary>
        /// <param name="destinationPosition">the point on the table that the mouse was clicked.</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 destinationPosition, bool isRunning)
        {
            MoveStand(destinationPosition, isRunning);
        }

        #region Private Methods

        private void Update()
        {
            var args = new StandLocationArgs(_mesh.transform.position, _modelSockets.Select(socket => socket.StandPosition).ToArray());
            OnLocationChanged?.Invoke(this, args);
        }

        private void MoveStand(Vector3 destinationPosition, bool isRunning)
        {
            //will be moving the stand, and then the models on the stand.  the models on the stand will have an offset value of the location 
            StandController.Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            StandController.Destination = destinationPosition;
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
