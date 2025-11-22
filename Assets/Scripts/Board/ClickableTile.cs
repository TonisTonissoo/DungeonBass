using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour, IPointerClickHandler
{
    public Waypoint myWaypoint;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HorseCarriageUI.Instance != null &&
            HorseCarriageUI.Instance.IsChoosingTile)
        {
            HorseCarriageUI.Instance.OnTileClicked(myWaypoint);
        }
    }
}