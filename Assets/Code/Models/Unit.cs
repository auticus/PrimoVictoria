using System;
using System.Collections.Generic;
using UnityEngine;
using PrimoVictoria.DataModels;
using PrimoVictoria.Controllers;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Unit class that represents the entirety of a unit and who belongs to it (all of the models that make up the unit)
    /// Added via the manager when creating a new unit
    /// </summary>
    public class Unit : MonoBehaviour
    {
        public UnitData Data;
        public int ID;
        private List<GameObject> SoldierMeshes;
        private GameObject PivotMesh;  //the central figure in the first rank

        private void Start()
        {
            
        }

        public void SetLocation(Vector3 location)
        {
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        public Vector3 GetLocation()
        {
            return PivotMesh.transform.position;
        }

        /// <summary>
        /// The rotation of the object in Euler angles
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRotation()
        {
            return PivotMesh.transform.eulerAngles;
        }

        public void SetRotation(float rotation)
        {
            //setting rotation:  https://docs.unity3d.com/ScriptReference/Transform-eulerAngles.html or https://docs.unity3d.com/ScriptReference/Quaternion-operator_multiply.html
            //setting the Y in the euler, x & z should stay 0
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        /// <summary>
        /// Game Controller method which will set the meshes within the unit
        /// </summary>
        /// <param name="initialSize"></param>
        /// <param name="unitLocation"></param>
        /// <param name="rotation">Euler angles that are then turned into a quaternion</param>
        public void InitializeUnit(GameObject parent, int unitID, UnitSize initialSize, Vector3 unitLocation, Vector3 rotation)
        {
            ID = unitID;

            if (SoldierMeshes == null)
                SoldierMeshes = new List<GameObject>();

            SoldierMeshes.Clear();

            //todo: implement actually creating the size, right now this just does one guy for lolz
            //should be using initialSize
            var modelCount = 1;
            
            for (int i = 0; i < modelCount; i++)
            {
                //todo: first guy in will likely be a standard bearer mesh of some kind so not everything will just use the UnitMesh object here
                var soldier = Instantiate(original: Data.UnitMesh, position: unitLocation, rotation: Quaternion.Euler(rotation));

                if (soldier == null)
                    Debug.Log("Soldier is null");

                var controller = soldier.GetComponent<UnitMeshController>();

                if (controller == null)
                    Debug.Log("Controller is null");

                controller.UnitID = unitID;

                SoldierMeshes.Add(soldier);
                if (i == 0) //first item created is the pivot guy
                {
                    PivotMesh = soldier;
                }

                soldier.transform.SetParent(parent.transform);
            }
        }

        public void Select(GameObject projectorPrefab)
        {
            var selectionObject = Instantiate(projectorPrefab);
            var projector = selectionObject.GetComponentInChildren<Projector>();
            projector.orthographicSize = (float)Data.SelectionOrthoSize;

            foreach(var mesh in SoldierMeshes)
            {
                selectionObject.transform.SetParent(mesh.transform, worldPositionStays: false); //worldPositionStays = false otherwise who knows where the circle goes
            }
        }

        public void Unselect()
        {
            //this may not be the best way to go about this but doing research, the performance shouldn't be bad since Unity keeps a list of all actual tagged objects
            //if performance is an issue, try keeping the projectors in an arraylist and just hit that
            var projectors = GameObject.FindGameObjectsWithTag(GameManager.MESH_DECORATOR_TAG);
            foreach (var projector in projectors)
            {
                Destroy(projector);
            }
        }

        public void MoveUnit(Vector3 pivotMeshPosition, bool isRunning)
        {
            var pivotController = PivotMesh.GetComponent<UnitMeshController>();

            if (pivotController == null)
            {
                Debug.LogError("Unit Mesh does not have a controller!");
                return;
            }

            pivotController.Speed = isRunning ? Data.RunSpeed : Data.WalkSpeed;
            pivotController.Destination = pivotMeshPosition;

            //todo: move the rest of the unit based on a grid
        }
    }
}
