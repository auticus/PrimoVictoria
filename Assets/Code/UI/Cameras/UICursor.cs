using UnityEngine;

public class UICursor : MonoBehaviour
{
    [SerializeField] Texture2D NoUnit_Default = null;
    [SerializeField] Texture2D NoUnit_Friendly = null;
    [SerializeField] Texture2D NoUnit_Enemy = null;
    [SerializeField] Texture2D Unknown = null;
    [SerializeField] CameraRaycaster Raycaster = null;

    void Start()
    {
        Raycaster.OnLayerChanged += AdjustCursor;
    }

    private void AdjustCursor (object sender, Layer layer)
    {
        //todo: need to add the ability to know if we have a friend or enemy clicked, this should be stored in a game manager of some type
        var selectedUnit = GameManager.instance.SelectedUnit;

        if (selectedUnit == null)
        {
            SetNoUnitSelectedIcon(layer.Name);
        }
        //else if the unit is a friend then set friend
        //else if the unit is an enemy then set enemy
        //else what the fuck is going on this should never happen
    }

    private void SetNoUnitSelectedIcon(string layer)
    {
        switch (layer)
        {
            case Layer.POST_PROCESSING:
            case Layer.DEFAULT:
                Cursor.SetCursor(Unknown, new Vector2(16, 16), CursorMode.Auto);
                break;
            case Layer.FRIENDLY:
                Cursor.SetCursor(NoUnit_Friendly, new Vector2(16, 16), CursorMode.Auto);
                break;
            case Layer.ENEMY:
                Cursor.SetCursor(NoUnit_Enemy, new Vector2(16, 16), CursorMode.Auto);
                break;
            case Layer.STRUCTURES:
                Cursor.SetCursor(Unknown, new Vector2(16, 16), CursorMode.Auto);
                break;
            case Layer.TERRAIN:
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
