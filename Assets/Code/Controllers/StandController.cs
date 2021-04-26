﻿using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using PrimoVictoria.Assets.Code.Models.Utilities;
using PrimoVictoria.Models;
using Unity.UNetWeaver;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Class that represents the high level of a stand used for moving the stand and accessing the parent unit and stand data
/// </summary>
public class StandController : MonoBehaviour
{
    public Unit ParentUnit;
    public Stand Stand;
    private bool _executingMovement;

    private void Update()
    {
        if ((Stand.ShouldMove || Stand.ShouldRotate) && !_executingMovement)
        {
            _executingMovement = true;
            StartCoroutine(MoveStand());
        }
    }

    private void LateUpdate()
    {
        AdjustStandMeshHeight();
    }

    private IEnumerator MoveStand()
    {
        while(Stand.ShouldRotate)
        {
            RotateStandMesh();
            yield return null;
        }

        while (Stand.ShouldMove)
        {
            MoveStandMesh();
            yield return null;
        }

        //yield return new WaitForSeconds(1); //just an example of WaitForSeconds
        _executingMovement = false;
    }

    /// <summary>
    /// Check the four corners of the stand and get the height of each, returning the highest
    /// </summary>
    private float GetHighestYCoordinate()
    {
        //get the X and the Z scale of the gameobject then find the corner and verify the positions
        //note that if the scale is "2" then that means 1 unit in any direction past center is the edge (divide by two)
        //0,0 is the center.  so some values will be negative and some positive around that

        var xScale = Stand.MeshScale.x;
        var zScale = Stand.MeshScale.z;
        var x = Stand.Position.x + (xScale / 2);
        var y = Stand.Position.y;
        var z = Stand.Position.z + (zScale / 2);

        var points = new[]
        {
            new Vector3(Stand.Position.x,Stand.Position.y, Stand.Position.z),
            new Vector3(x,y,z),
            new Vector3(-x, y, -z),
            new Vector3(-x, y, z),
            new Vector3(x, y, -z)
        };

        //for every point in the array, get the height and return back the highest point found (in this case by putting all the heights into a new array and then getting max)
        return points.Select(point => Terrain.activeTerrain.SampleHeight(point)).Concat(new[] {0.0f}).Max();
    }

    private void AdjustStandMeshHeight()
    {
        //sample the height where we are and bump us up so we aren't clipping through the terrain
        //removing this also makes the stand mesh do weird things when moving like ramp up and be dramatic when moving over bumps on the terrain
        var pos = Stand.Position;
        pos.y = GetHighestYCoordinate();
        Stand.Position = pos; //PROBLEM - this is still not raising the piece up - and the Y is listed as higher than the terrain but the terrain is still clipping through
    }

    /// <summary>
    /// Rotates the mesh if needed
    /// </summary>
    /// <returns>RETURNS TRUE if a rotation occurred</returns>
    private void RotateStandMesh()
    {
        //we're rotating around the Y axis so transform right or left into up or down
        var direction = Stand.RotationDirection == Vector3.right ? Vector3.up : Vector3.up * -1;
        Stand.Transform.Rotate(direction * (Stand.Speed * Time.deltaTime * Stand.WheelSpeed));

        if (Stand.IsFacingDestination) Stand.StopRotating();
    }

    private void MoveStandMesh()
    {
        var step = Stand.Speed * Time.deltaTime; //calculate the distance to move
        Stand.Position = Vector3.MoveTowards(Stand.Position, Stand.Destination, step);
        //Debug.Log($"Current Position={Stand.Position}::Destination = {Stand.Destination}::Registered Distance Left = {Stand.MoveDistance}::Step = {step}");
    }
}
