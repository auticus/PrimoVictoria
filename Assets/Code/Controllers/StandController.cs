using PrimoVictoria.Models;
using UnityEngine;

/// <summary>
/// Class that represents the high level of a stand used for moving the stand and accessing the parent unit and stand data
/// </summary>
public class StandController : MonoBehaviour
{
    public Unit ParentUnit;
    public Stand Stand;
    public Vector3 Destination;
    public float Speed;

    private void Start()
    {
        
    }

    private void Update()
    {
        var currentPos = Stand.MeshTransform.position;
        var currentRot = Stand.MeshTransform.rotation;
        if (Vector3.Distance(currentPos, Destination) < 0.001f) return;

        var step = Speed * Time.deltaTime; //calculate the distance to move
        var direction = (Destination - currentPos).normalized;
        var lookRotation = Quaternion.LookRotation(direction);

        Stand.MeshTransform.rotation = Quaternion.Slerp(currentRot, lookRotation, Time.deltaTime * Speed);
        Stand.MeshTransform.position = Vector3.MoveTowards(currentPos, Destination, step);
    }

    private void LateUpdate()
    {
        //sample the height where we are and bump us up so we aren't clipping through the terrain
        //removing this also makes the stand mesh do weird things when moving like ramp up and be dramatic when moving over bumps on the terrain
        var pos = Stand.MeshTransform.position;
        pos.y = GetHighestYCoordinate();
        Stand.MeshTransform.position = pos; //PROBLEM - this is still not raising the piece up - and the Y is listed as higher than the terrain but the terrain is still clipping through
    }

    /// <summary>
    /// Check the four corners of the stand and get the height of each, returning the highest
    /// </summary>
    private float GetHighestYCoordinate()
    {
        //get the X and the Z scale of the gameobject then find the corner and verify the positions
        //note that if the scale is "2" then that means 1 unit in any direction past center is the edge (divide by two)
        //0,0 is the center.  so some values will be negative and some positive around that

        var xScale = Stand.MeshTransform.transform.localScale.x;
        var zScale = Stand.MeshTransform.transform.localScale.z;
        var x = Stand.MeshTransform.transform.position.x + (xScale / 2);
        var y = Stand.MeshTransform.transform.position.y;
        var z = Stand.MeshTransform.transform.position.z + (zScale / 2);

        var points = new Vector3[]
        {
            new Vector3(Stand.MeshTransform.transform.position.x,Stand.MeshTransform.transform.position.y, Stand.MeshTransform.transform.position.z),
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
}
