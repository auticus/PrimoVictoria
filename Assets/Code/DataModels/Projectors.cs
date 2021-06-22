using UnityEngine;

/// <summary>
/// Projectors is a container class that contains various UI projectors that are used to draw elements on the screen
/// </summary>
[CreateAssetMenu(fileName = "New Projector Collection", menuName = "New Projector Collection", order = 4)]
public class Projectors : ScriptableObject
{
    public GameObject FriendlySelection;
    public GameObject EnemySelection;

    [Tooltip("The value of the transparency on a ghost select (1.0 is full opacity)")]
    public float GhostSelectionTransparency;

    [Tooltip("The value of the transparency on a full select (1.0 is full opacity)")]
    public float FullSelectionTransparency;
}
