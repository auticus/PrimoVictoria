﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PrimoVictoria.DataModels;
using PrimoVictoria.Core.Events;
using PrimoVictoria.Models.Parameters;
using PrimoVictoria.UI;
using PrimoVictoria.Utilities;

namespace PrimoVictoria.Models
{
    /// <summary>
    /// Unit class that represents the entirety of a unit and who belongs to it (all of the models that make up the unit)
    /// Added via the manager when creating a new unit
    /// </summary>
    public class Unit : MonoBehaviour
    {
        public enum WheelPointIndices
        {
            Left_UpperLeft = 0,
            Left_UpperRight,
            Right_UpperLeft,
            Right_UpperRight
        }

        public UnitData Data;
        public int ID;
        public string Name;
        public UnitFormation Formation { get; set; }

        private List<Stand> _stands;
        private Stand _pivotStand;  //the central stand in the first rank

        /// <summary>
        /// Left.UpperLeft, Left.UpperRight, Right.UpperLeft, Right.UpperRight position points of the unit
        /// </summary>
        private List<Vector3> WheelPoints;

        void Awake()
        {
            Formation = new UnitFormation { type = UnitFormation.FormationType.RankAndFile, FullRank = 5, Spacing = 1 };
        }

        /// <summary>
        /// Gets the pivot stand, the stand that is considered the center of the front rank
        /// </summary>
        /// <returns></returns>
        public Stand GetPivotStand() => _pivotStand;
        
        /// <summary>
        /// Game Controller method which will set the meshes within the unit
        /// </summary>
        public void InitializeUnit(UnitInitializationParameters parameters)
        {
            ID = parameters.UnitID;
            Name = parameters.Name;
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

                var parms = new StandInitializationParameters(index: i+1, this, Data, parameters.UnitLocation, parameters.Rotation,
                    fileIndex: file, rankIndex: row, standVisible: parameters.StandVisible, spacing: Formation.Spacing);
                var standModel = UnitFactory.BuildStand(parms);
                _stands.Add(standModel);

                if (i == 0)
                {
                    _pivotStand = standModel;
                }
            }
        }

        /// <summary>
        /// Selects the Unit overall, which should highlight all of the stands within it
        /// </summary>
        /// <param name="projectorPrefab"></param>
        public void SelectFriendly(Projectors projectors)
        {
            foreach (var stand in _stands)
            {
                stand.SelectFriendly(projectors);
            }
        }

        public void GhostSelectFriendly(Projectors projectors)
        {
            foreach (var stand in _stands)
            {
                stand.SelectGhostedFriendly(projectors);
            }
        }

        /// <summary>
        /// Selects the Unit overall, which should highlight all of the stands within it
        /// </summary>
        /// <param name="projectorPrefab"></param>
        public void SelectEnemy(Projectors projectors)
        {
            foreach (var stand in _stands)
            {
                stand.SelectEnemy(projectors);
            }
        }

        public void GhostSelectEnemy(Projectors projectors)
        {
            foreach (var stand in _stands)
            {
                stand.SelectGhostedEnemy(projectors);
            }
        }

        public void Unselect()
        {
            PrimoProjector.RemoveProjectors(StaticResources.MESH_DECORATOR_TAG);
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
                stand.Move(pivotMeshRawPosition, isRunning, Formation);
                destinations.Add(stand.Destination);
            }

            EventManager.Publish(PrimoEvents.UserInterfaceChange, new UserInterfaceArgs(this, UserInterfaceArgs.UserInterfaceCommand.DrawUnitDestination, destinations));
        }

        /// <summary>
        /// Given a position, will draw the footprint of the unit in that location
        /// </summary>
        /// <param name="pivotMeshRawPosition"></param>
        public IEnumerable<Vector3> GetProjectedMove(Vector3 pivotMeshRawPosition)
        {
            var destinations = new List<Vector3>();
            foreach (var stand in _stands)
            {
                destinations.Add(stand.ProjectedMove(pivotMeshRawPosition, Formation));
            }

            return destinations;
        }

        /// <summary>
        /// Moves the unit through manual controller input as opposed to clicking a location on the table
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="isRunning"></param>
        public void ManualMove(Vector3 direction, bool isRunning)
        {
            EventManager.Publish(PrimoEvents.UserInterfaceChange, new UserInterfaceArgs(this, UserInterfaceArgs.UserInterfaceCommand.DrawUnitDestination, new List<Vector3>()));

            foreach (var stand in _stands)
            {
                stand.ManualMove(direction, isRunning);
            }
        }

        /// <summary>
        /// Will halt the manual move
        /// </summary>
        public void StopManualMove()
        {
            foreach (var stand in _stands)
            {
                stand.StopManualMove();
            }
        }

        /// <summary>
        /// Will wheel the unit in the direction passed
        /// </summary>
        /// <param name="direction">a vector3 that is either right or left</param>
        /// <param name="isRunning">should the models appear to be running</param>
        public void Wheel(Vector3 direction, bool isRunning)
        {
            if (direction != Vector3.right && direction != Vector3.left)
                throw new ArgumentOutOfRangeException(nameof(direction), "direction must be vector right or left");

            WheelPoints = GetUnitWheelingPivotPoints();
            foreach (var stand in _stands)
            {
                stand.Wheel(direction, isRunning);
            }
        }

        /// <summary>
        /// Stops the wheeling action
        /// </summary>
        public void StopWheel()
        {
            foreach (var stand in _stands)
            {
                stand.StopRotating();
            }
        }

        /// <summary>
        /// Returns the vector3 at the index passed, unless not instantiated, in which case returns a Vector3.zero
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetUnitWheelPoint(WheelPointIndices index)
        {
            if (WheelPoints == null || WheelPoints.Count == 0) return Vector3.zero;

            return WheelPoints[(int) index];
        }

        public void ToggleDiagnostic(bool value)
        {
            //turns the diagnostics on for the pivot stand
            _stands[0].DiagnosticsOn = value;
        }

        /// <summary>
        /// Returns an array of two points, one for the left, and one for the right upper corners where the unit will wheel around
        /// </summary>
        /// <returns></returns>
        private List<Vector3> GetUnitWheelingPivotPoints()
        {
            //vector 1 = top left corner of the unit
            //vector 2 = top right corner of the unit
            var vectors = new List<Vector3>();

            //1 - how wide is the front rank
            var unitWidth = _stands.Count(stand => stand.RankIndex == 1);
            
            //2 - which is the left most stand and right most stand
            //even file indexes are left, odd file indexes are right, and if you only have one then its both left & right
            var leftMost = GetEndStand(unitWidth, Vector3.right * -1);
            var rightMost = GetEndStand(unitWidth, Vector3.right);
            
            //3 - return the points we care about
            vectors.Add(leftMost.UpperLeftPoint);
            vectors.Add(leftMost.UpperRightPoint);
            vectors.Add(rightMost.UpperLeftPoint);
            vectors.Add(rightMost.UpperRightPoint);

            return vectors;
        }

        /// <summary>
        /// Gets the last stand in the first row either right or left
        /// </summary>
        /// <param name="unitWidth"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Stand GetEndStand(int unitWidth, Vector3 direction)
        {
            if (direction != Vector3.right && direction != Vector3.right * -1)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "Direction passed must be left or right");
            }

            //even indexes are left, odd are right
            if (unitWidth == 1)
            {
                return _stands.First(stand => stand.RankIndex == 1);
            }

            var maxStand = _stands.Where(stand => stand.RankIndex == 1).OrderByDescending(stand => stand.FileIndex).First();
            var isEven = maxStand.FileIndex % 2 == 0;

            if (direction == Vector3.right * -1 && isEven) return maxStand;
            if (direction == Vector3.right && !isEven) return maxStand;

            //need the one before it at this point - so get me the last 2 in descending order and return the "1" element (the last element)
            var stands = _stands.Where(stand => stand.RankIndex == 1).OrderByDescending(stand => stand.FileIndex).Take(2).ToArray();
            return stands[1];
        }
    }
}
