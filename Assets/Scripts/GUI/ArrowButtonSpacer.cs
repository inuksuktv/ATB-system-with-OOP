using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButtonSpacer : MonoBehaviour
{
    [SerializeField] RectTransform markerRtra;
    [SerializeField] RectTransform canvasRect;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        OrientSpacer();
    }

    private void OrientSpacer()
    {
        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

        // Set
        markerRtra.localPosition = canvasPos;
    }
}
