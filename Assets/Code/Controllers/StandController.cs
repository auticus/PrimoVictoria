using System.Security.Cryptography;
using System.Text;
using PrimoVictoria.Controllers;
using PrimoVictoria.Models;
using UnityEngine;

/// <summary>
/// Class that represents the high level of a stand used for moving the stand and accessing the parent unit and stand data
/// </summary>
public class StandController : MonoBehaviour
{
    public Unit ParentUnit;
    public Stand StandData;
    public Vector3 Destination;
    public float Speed;

    private void Update()
    {
        //code put here as an example of moving the stand without the help of a navmesh agent
        if (Vector3.Distance(transform.position, Destination) < 0.001f) return;

        var step = Speed * Time.deltaTime; //calculate the distance to move
        var direction = (Destination - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);
        transform.position = Vector3.MoveTowards(transform.position, Destination, step);
    }

    private void LateUpdate()
    {
        //sample the height where we are and bump us up so we aren't clipping through the terrain
        var pos = transform.position;
        pos.y = GetHighestYCoordinate();
        transform.position = pos; //PROBLEM - this is still not raising the piece up - and the Y is listed as higher than the terrain but the terrain is still clipping through
    }

    /// <summary>
    /// Check the four corners of the stand and get the height of each, returning the highest
    /// </summary>
    private float GetHighestYCoordinate()
    {
        //get the X and the Z scale of the gameobject then find the corner and verify the positions
        //note that if the scale is "2" then that means 1 unit in any direction past center is the edge (divide by two)
        //0,0 is the center.  so some values will be negative and some positive around that

        var xScale = transform.localScale.x;
        var zScale = transform.localScale.z;
        var x = transform.localPosition.x + (xScale / 2);
        var y = transform.localPosition.y;
        var z = transform.localPosition.z + (zScale / 2);

        var points = new Vector3[]
        {
            new Vector3(transform.localPosition.x,transform.localPosition.y, transform.localPosition.z),
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
