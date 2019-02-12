using UnityEngine;

public class UICursor : MonoBehaviour
{
    [SerializeField] Texture2D NoUnit_Default;
    [SerializeField] Texture2D NoUnit_Friendly;
    [SerializeField] Texture2D NoUnit_Enemy;
    [SerializeField] Texture2D Unknown;
    [SerializeField] CameraRaycaster Raycaster;

    void Start()
    {
        Raycaster.OnLayerChanged += AdjustCursor;
    }

    private void AdjustCursor (object sender, string layer)
    {
        //todo: need to add the ability to know if we have a friend or enemy clicked, this should be stored in a game manager of some type
        var selectedUnit = GameManager.instance.SelectedUnit;

        if (selectedUnit == null)
        {
            SetNoUnitSelectedIcon(layer);
        }
        //else if the unit is a friend then set friend
        //else if the unit is an enemy then set enemy
        //else what the fuck is going on this should never happen
    }

    private void SetNoUnitSelectedIcon(string layer)
    {
        switch (layer)
        {
            case CameraRaycaster.POST_PROCESSING:
                Cursor.SetCursor(Unknown, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CameraRaycaster.FRIENDLY:
                Cursor.SetCursor(NoUnit_Friendly, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CameraRaycaster.ENEMY:
                Cursor.SetCursor(NoUnit_Enemy, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CameraRaycaster.STRUCTURES:
                Cursor.SetCursor(Unknown, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CameraRaycaster.TERRAIN:
                Cursor.SetCursor(NoUnit_Default, new Vector2(16, 16), CursorMode.Auto);
                break;
        }
    }

    private void SetFriendSelectedIcon(string layer)
    {

    }

    private void SetEnemySelectedIcon(string layer)
    {

    }
}
