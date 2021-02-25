using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PrimoVictoria.Models;
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
        public List<GameObject> ModelMeshes; //the living meshes
        public int StandCapacity;  //how many models fit on the stand at full capacity
        public bool StandVisible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
                                   //todo: add a stand object that they can stand on that can be turned off and on

        private GameObject _pivotMesh;  //the central figure in the first rank, the model that gets moved first and the others on the stand distance around
                                        //todo the pivot mesh should become the stand itself not a model on the stand
        private UnitData _data; //data pertinent to the unit overall

        public void InitializeStand(Unit parent, UnitData data, Vector3 location, Vector3 rotation)
        {
            Id = Guid.NewGuid();
            ParentUnit = parent;
            _data = data;

            if (ModelMeshes == null)
                ModelMeshes = new List<GameObject>();

            ModelMeshes.Clear();

            //todo: implement actually creating the size, right now this just does one guy for lolz
            //should be using stand capacity
            var modelCount = 1;

            for (int i = 0; i < modelCount; i++)
            {
                //todo: first guy in will likely be a standard bearer mesh of some kind so not everything will just use the UnitMesh object here
                var soldier = Instantiate(original: _data.UnitMesh, position: location, rotation: Quaternion.Euler(rotation));

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

                controller.UnitID = parent.ID;

                ModelMeshes.Add(soldier);
                if (i == 0) //first item created is the pivot guy
                {
                    _pivotMesh = soldier;
                }

                soldier.transform.SetParent(parent.transform);
            }
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

        public void Select(GameObject projectorPrefab)
        {
            var selectionObject = Instantiate(projectorPrefab);
            var projector = selectionObject.GetComponentInChildren<Projector>();
            projector.orthographicSize = (float)_data.SelectionOrthoSize;

            //draw the projector prefab (the circle under the models) under the models
            foreach (var mesh in ModelMeshes)
            {
                selectionObject.transform.SetParent(mesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
            }
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

            pivotController.Speed = isRunning ? _data.RunSpeed : _data.WalkSpeed;
            pivotController.Destination = pivotMeshPosition;

            //todo: move the rest of the unit based on a grid
        }
    }
}
