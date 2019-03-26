using System;
using System.Collections.Generic;
using UnityEngine;
using PrimoVictoria.DataModels;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Unit class that represents the entirety of a unit and who belongs to it
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
        public void InitializeUnit(int unitID, UnitSize initialSize, Vector3 unitLocation, Vector3 rotation)
        {
            ID = unitID;

            if (SoldierMeshes == null)
                SoldierMeshes = new List<GameObject>();

            SoldierMeshes.Clear();

            //todo: implement actually creating the size, right now this just does one guy for lolz
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
            }
        }
    }
}
