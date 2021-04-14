using System;
using System.Collections.Generic;
using PrimoVictoria.Assets.Code.Models;
using PrimoVictoria.Assets.Code.Models.Parameters;
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
        public EventHandler<StandLocationArgs> OnLocationChanged;

        private List<Stand> _stands;
        private Stand _pivotStand;  //the central stand in the first rank
        private Vector3 _pivotStandMeshScale; //how big the stand is
        private GameObject _unit; //the owning game object
        private bool _standsVisible;
        private GameManager _gameManager;

        public void SetLocation(Vector3 location)
        {
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        public Stand GetPivotStand() => _pivotStand;
        /// <summary>
        /// The location of the unit based on the position of its Pivot Stand
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocation() => _pivotStand.Position;

        /// <summary>
        /// The rotation of the unit based on its Pivot Stand
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation() => _pivotStand.Rotation;

        public void SetRotation(float rotation)
        {
            //setting rotation:  https://docs.unity3d.com/ScriptReference/Transform-eulerAngles.html or https://docs.unity3d.com/ScriptReference/Quaternion-operator_multiply.html
            //setting the Y in the euler, x & z should stay 0
            throw new NotImplementedException("This feature has not been implemented yet Tokies");
        }

        /// <summary>
        /// Game Controller method which will set the meshes within the unit
        /// </summary>
        public void InitializeUnit(UnitInitializationParameters parameters)
        {
            ID = parameters.UnitID;
            _unit = parameters.ContainingGameObject;
            _stands = new List<Stand>();

            //initialize the stands in the unit
            var file = 0; //a  file in this sense is the column count of the model in the regiment... ie rank and FILE
            var row = 1;

            //the first stand coming in is located at UnitLocation.  The stands around it will fan out around it
            for (var i = 0; i < parameters.StandCount; i++)
            {
                file++;
                if (file > parameters.HorizontalStandCount)
                {
                    file = 1;
                    row++;
                }

                var stand = new GameObject($"Stand_{Data.Name}_{i + 1}");
                var standModel = stand.AddComponent<Stand>();

                stand.transform.SetParent(this.transform);
                standModel.StandCapacity = 4; //todo: this is locked into conquest and needs to not be hardcoded magic number

                standModel.InitializeStand(new StandInitializationParameters(this, Data, parameters.UnitLocation, parameters.Rotation,
                    fileIndex: file, rankIndex: row, parameters.StandVisible, parameters.ModelMeshesVisible));

                _stands.Add(standModel);

                if (i == 0)
                {
                    RegisterEvents(standModel); //first stand set its event up to track its location
                    _pivotStand = standModel;
                    _pivotStandMeshScale = standModel.MeshScale;
                }
            }
        }

        /// <summary>
        /// Selects the Unit overall, which should highlight all of the stands within it
        /// </summary>
        /// <param name="projectorPrefab"></param>
        public void Select(Projectors projectors, bool isFriendly)
        {
            foreach (var stand in _stands)
            {
                stand.Select(projectors, isFriendly);
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
        /// <param name="pivotMeshRawPosition">Pivot Mesh Position is the point on the table that the mouse was clicked.
        /// It is the location that the pivot soldier will land on and the others around him will be spaced as they need</param>
        /// <param name="isRunning"></param>
        public void Move(Vector3 pivotMeshRawPosition, bool isRunning)
        {
            var destinations = new List<Vector3>();
            foreach (var stand in _stands)
            {
                stand.Move(pivotMeshRawPosition, isRunning);
                destinations.Add(stand.Destination);
            }

            _gameManager.SelectedUnitDestinations = destinations;
        }

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void RegisterEvents(Stand stand)
        {
            stand.OnLocationChanged += (sender, args) =>
            {
                OnLocationChanged?.Invoke(this, args);
            };
        }
    }
}
