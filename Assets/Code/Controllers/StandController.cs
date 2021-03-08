using System.Security.Cryptography;
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
        var oldpos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
        transform.position = pos;
    }
    
}
