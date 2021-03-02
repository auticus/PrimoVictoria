using UnityEngine;

/// <summary>
/// Projectors is a container class that contains various UI projectors that are used to draw elements on the screen
/// </summary>
[CreateAssetMenu(fileName = "New Projector Collection", menuName = "New Projector Collection", order = 4)]
public class Projectors : ScriptableObject
{
    public GameObject FriendlyCircleSelection;
    public GameObject OtherCircleSelection;
    public GameObject FriendlySquareSelection;
    public GameObject OtherSquareSelection;
}
