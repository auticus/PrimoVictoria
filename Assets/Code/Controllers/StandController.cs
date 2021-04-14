using System.Collections;
using System.Numerics;
using PrimoVictoria.Assets.Code.Models.Utilities;
using PrimoVictoria.Models;
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

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Stand.ShouldMove() && !_executingMovement)
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
        while (!IsFacing())
        {
            RotateStandMesh();
            yield return null;
        }

        while (Stand.ShouldMove())
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

        var points = new Vector3[]
        {
            new Vector3(Stand.Position.x,Stand.Position.y, Stand.Position.z),
            new Vector3(x,y,z),
            new Vector3(-x, y, -z),
            new Vector3(-x, y, z),
            new Vector3(x, y, -z)
        };

        var highestY = 0.0f;
        foreach (var point in points)
        {
            var currentY = Terrain.activeTerrain.SampleHeight(point);
            if (currentY > highestY) highestY = currentY;
        }

        return highestY;
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
    /// Rotates the mesh if needed.  
    /// </summary>
    /// <returns>RETURNS TRUE if a rotation occurred</returns>
    private void RotateStandMesh()
    {
        //Debug.Log("Rotating");
        var direction = (Stand.Destination - Stand.Position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);

        Stand.Rotation = Quaternion.Slerp(Stand.Rotation, lookRotation, Time.deltaTime * Stand.Speed);
    }

    private void MoveStandMesh()
    {
        var step = Stand.Speed * Time.deltaTime; //calculate the distance to move
        var direction = (Stand.Destination - Stand.Position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);
        Stand.Position = Vector3.MoveTowards(Stand.Position, Stand.Destination, step);
        //Debug.Log($"Current Position={Stand.Position}::Destination = {Stand.Destination}::Registered Distance Left = {Stand.MoveDistance}::Step = {step}");
    }

    private bool IsFacing()
    {
        var angle = Vector3.Angle(Stand.Transform.forward, Stand.Destination - Stand.Transform.position);
        //Debug.Log($"Checking if facing::Angle={angle}");
        return angle < 10;
    }
}
