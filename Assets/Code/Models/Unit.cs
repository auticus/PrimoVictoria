﻿using System;
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
        private List<Stand> _stands;
        private readonly GameObject _pivotMesh;  //the central figure in the first rank
        private GameObject _parent; //the owning game object

        private const string STANDS_GAMEOBJECT = "Stands";

        private void Start()
        {
            
        }

        public void SetLocation(Vector3 location)
        {
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        public Vector3 GetLocation()
        {
            return _pivotMesh.transform.position;
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

        /// <summary>
        /// Game Controller method which will set the meshes within the unit
        /// </summary>
        /// <param name="initialSize"></param>
        /// <param name="unitLocation"></param>
        /// <param name="rotation">Euler angles that are then turned into a quaternion</param>
        public void InitializeUnit(GameObject parent, int unitID, int standCount, Vector3 unitLocation, Vector3 rotation)
        {
            ID = unitID;
            _parent = parent;
            _stands = new List<Stand>();

            var stands = GameObject.Find(STANDS_GAMEOBJECT);

            //add my stands collection if it doesn't already exist
            if (stands == null)
            {
                stands = new GameObject(STANDS_GAMEOBJECT);
            }

            //initialize the stands in the unit
            //todo: we shouldn't be hardcoding positions and rotation this is just for demo sake
            for (int i = 0; i < standCount; i++)
            {
                var stand = stands.AddComponent<Stand>();
                var location = new Vector3(104.81f, 0.02f, 80.736f);
                var demoRotation = new Vector3(0, 178, 0);

                stand.StandCapacity = 4;
                stand.StandVisible = false;
                stand.InitializeStand(this, Data,  location, demoRotation); 

                _stands.Add(stand);
            }
        }

        /// <summary>
        /// Selects the Unit overall, which should highlight all of the stands within it
        /// </summary>
        /// <param name="projectorPrefab"></param>
        public void Select(GameObject projectorPrefab)
        {
            foreach (var stand in _stands)
            {
                stand.Select(projectorPrefab);
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

        /// <summary>
        /// Moves the unit
        /// </summary>
        /// <param name="pivotMeshPosition">Pivot Mesh Position is the point on the table that the mouse was clicked.
        /// It is the location that the pivot soldier will land on and the others around him will be spaced as they need</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 pivotMeshPosition, bool isRunning)
        {
            foreach (var stand in _stands)
            {
                stand.Move(pivotMeshPosition, isRunning);
            }

            //todo: move the rest of the unit based on a grid
        }
    }
}
