using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent (typeof (RectTransform))]
public class RaycastCollider : Graphic {

    private static readonly Vector3[] RECT_WORLD_CORNERS = new Vector3[4];

    private RectTransform rect = null;

    public override void SetMaterialDirty () { return; }
    public override void SetVerticesDirty () { return; }

    protected override void OnPopulateMesh (VertexHelper vh) {
        vh.Clear ();
        return;
    }

    private void OnDrawGizmosSelected () {
        if (rect == null) {
            rect = GetComponent<RectTransform>();
        }

        rect.GetWorldCorners(RECT_WORLD_CORNERS);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(RECT_WORLD_CORNERS[0], RECT_WORLD_CORNERS[1]);
        Gizmos.DrawLine(RECT_WORLD_CORNERS[1], RECT_WORLD_CORNERS[2]);
        Gizmos.DrawLine(RECT_WORLD_CORNERS[2], RECT_WORLD_CORNERS[3]);
        Gizmos.DrawLine(RECT_WORLD_CORNERS[3], RECT_WORLD_CORNERS[0]);
    }
}
