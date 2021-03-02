using System.Collections.Generic;
using System;
using UnityEngine;
using PrimoVictoria.Controllers;
using PrimoVictoria.DataModels;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Stands represent the trays on which a model stands.  They hold X number of models
    /// </summary>
    public class Stand : MonoBehaviour
    {
        public Unit ParentUnit;
        public Guid Id;
        public List<GameObject> ModelMeshes; //the warrior meshes
        public GameObject StandMesh; //the mesh that is the stand that the models are standing on
        public int StandCapacity;  //how many models fit on the stand at full capacity
        
        private bool _visible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
                                //note: i'd much rather use properties here and getters and setters but thats not currently the unity way

        private bool _modelsVisible;  //if true will draw the model meshes on the stand.  If false, will only draw the stand

        private GameObject _pivotMesh;  //the central figure in the first rank, the model that gets moved first and the others on the stand distance around
                                        //todo the pivot mesh should become the stand itself not a model on the stand
        private UnitData _unitData; //data pertinent to the unit overall

        public void InitializeStand(Unit parent, UnitData data, Vector3 location, Vector3 rotation, bool visible, bool modelsVisible)
        {
            Id = Guid.NewGuid();
            ParentUnit = parent;
            _unitData = data;
            _visible = visible;
            _modelsVisible = modelsVisible;

            if (ModelMeshes == null)
                ModelMeshes = new List<GameObject>();

            ModelMeshes.Clear();
            
            InitializeStandMesh(parent, rotation, location, visible);
            AddModelMeshesToStand(location, rotation);  
        }

        /// <summary>
        /// The rotation of the object in Euler angles
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRotation()
        {
            return _pivotMesh.transform.eulerAngles;
        }

        public void SetRotation(float rotation)
        {
            //setting rotation:  https://docs.unity3d.com/ScriptReference/Transform-eulerAngles.html or https://docs.unity3d.com/ScriptReference/Quaternion-operator_multiply.html
            //setting the Y in the euler, x & z should stay 0
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        public Vector3 GetLocation()
        {
            return _pivotMesh.transform.position;
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

        public void Select(Projectors projectors, bool isFriendly)
        {
            if (_visible) SelectStand(projectors, isFriendly); 
            else SelectModelMeshes(projectors, isFriendly);
        }

        /// <summary>
        /// Moves the individual stand
        /// </summary>
        /// <param name="pivotMeshPosition">Pivot Mesh Position is the point on the table that the mouse was clicked.
        /// It is the location that the pivot soldier will land on and the others around him will be spaced as they need</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 pivotMeshPosition, bool isRunning)
        {
            var pivotController = _pivotMesh.GetComponent<UnitMeshController>();

            if (pivotController == null)
            {
                Debug.LogError("Unit Mesh does not have a controller!");
                return;
            }

            pivotController.Speed = isRunning ? _unitData.RunSpeed : _unitData.WalkSpeed;
            pivotController.Destination = pivotMeshPosition;

            //todo: move the rest of the unit based on a grid
        }

        #region Private Methods

        /// <summary>
        /// Draws selection projector around the entire stand
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectStand(Projectors projectors, bool isFriendly)
        {
            var selectionObject = GetSelectionProjector(projectors, isFriendly);
            selectionObject.transform.SetParent(StandMesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the projector goes
        }

        /// <summary>
        /// Draws selection projector around each model mesh
        /// </summary>
        /// <param name="projectors"></param>
        /// <param name="isFriendly"></param>
        private void SelectModelMeshes(Projectors projectors, bool isFriendly)
        {
            //draw the projector prefab (the circle under the models) under the models
            foreach (var mesh in ModelMeshes)
            {
                var selectionObject = GetSelectionProjector(projectors, isFriendly);
                selectionObject.transform.SetParent(mesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
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
            Debug.Log($"Instantiating stand  location={location} rotation={rotation}");
            StandMesh = Instantiate(original: _unitData.StandMesh, position: location, rotation: Quaternion.Euler(rotation));

            StandMesh.transform.SetParent(parent.transform);
            StandMesh.SetActive(visible);
            var standController = StandMesh.AddComponent<StandController>();
            standController.ParentUnit = parent;
            standController.StandData = this;
        }

        private void AddModelMeshesToStand(Vector3 location, Vector3 rotation)
        {
            var modelCount = 1;
            for (int i = 0; i < modelCount; i++)
            {
                //todo: first guy in will likely be a standard bearer mesh of some kind so not everything will just use the UnitMesh object here
                //this will likely take the form of an enum or something explaining each element in the UnitMeshes list and what it really is
                var soldier = Instantiate(original: _unitData.UnitMeshes[0], position: location, rotation: Quaternion.Euler(rotation));

                if (soldier == null)
                {
                    Debug.Log("Stand::InitializeStand - Model Mesh is null after instantiation");
                    continue;
                }

                var controller = soldier.GetComponent<UnitMeshController>();

                if (controller == null)
                {
                    Debug.LogWarning("Stand::InitializeStand - mesh controller was not found on the mesh");
                    continue;
                }
                controller.UnitID = ParentUnit.ID;

                ModelMeshes.Add(soldier);
                if (i == 0) //first item created is the pivot guy
                {
                    _pivotMesh = soldier;
                }

                soldier.transform.SetParent(ParentUnit.transform);
                soldier.SetActive(_modelsVisible);
            }
        }

        #endregion Private Methods
    }
}
