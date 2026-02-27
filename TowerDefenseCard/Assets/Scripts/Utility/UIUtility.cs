using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }

    public static void PlaceNearAnchor(
       RectTransform panel,
       Vector3 anchorWorldPosition,
       Vector2 sourceWorldSize,
       Vector3 preferredOffset,
       Camera camera = null)
    {
        if (camera == null)
            camera = Camera.main;

        // Lock Z to the canvas plane — never move it.
        float canvasZ = panel.position.z;

        // Depth from camera to canvas plane, used by ViewportToWorldPoint.
        float depth = camera.orthographic
            ? camera.nearClipPlane + 0.1f
            : Mathf.Abs((camera.transform.InverseTransformPoint(new Vector3(0, 0, canvasZ))).z);

        // ── 1. Start at preferred offset ──────────────────────────────────────
        panel.position = new Vector3(
            anchorWorldPosition.x + preferredOffset.x,
            anchorWorldPosition.y + preferredOffset.y,
            canvasZ);

        Canvas.ForceUpdateCanvases();

        // ── 2. Measure panel world-space extents via corners ──────────────────
        Vector3[] corners = new Vector3[4];
        panel.GetWorldCorners(corners);
        // [0] bottom-left  [1] top-left  [2] top-right  [3] bottom-right
        float panelW = corners[3].x - corners[0].x;
        float panelH = corners[1].y - corners[0].y;

        // ── 3. Clamp to viewport ──────────────────────────────────────────────
        panel.position = ClampToViewport(panel.position, panelW, panelH, canvasZ, depth, camera);

        // ── 4. Push away from source card if overlapping ──────────────────────
        Rect srcRect = new Rect(
            anchorWorldPosition.x - sourceWorldSize.x * 0.5f,
            anchorWorldPosition.y - sourceWorldSize.y * 0.5f,
            sourceWorldSize.x,
            sourceWorldSize.y);

        panel.GetWorldCorners(corners);
        Rect panelRect = new Rect(corners[0].x, corners[0].y, panelW, panelH);

        if (panelRect.Overlaps(srcRect))
        {
            float spaceRight = 1f - camera.WorldToViewportPoint(new Vector3(srcRect.xMax, anchorWorldPosition.y, canvasZ)).x;
            float spaceLeft = camera.WorldToViewportPoint(new Vector3(srcRect.xMin, anchorWorldPosition.y, canvasZ)).x;
            float spaceTop = 1f - camera.WorldToViewportPoint(new Vector3(anchorWorldPosition.x, srcRect.yMax, canvasZ)).y;
            float spaceBottom = camera.WorldToViewportPoint(new Vector3(anchorWorldPosition.x, srcRect.yMin, canvasZ)).y;

            float bestSpace = Mathf.Max(spaceRight, spaceLeft, spaceTop, spaceBottom);

            Vector3 pos = panel.position;
            if (bestSpace == spaceRight)
                pos.x = srcRect.xMax + panelW * 0.5f;
            else if (bestSpace == spaceLeft)
                pos.x = srcRect.xMin - panelW * 0.5f;
            else if (bestSpace == spaceTop)
                pos.y = srcRect.yMax + panelH * 0.5f;
            else
                pos.y = srcRect.yMin - panelH * 0.5f;

            panel.position = pos;

            // Re-clamp after push
            panel.position = ClampToViewport(panel.position, panelW, panelH, canvasZ, depth, camera);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Vector3 ClampToViewport(
        Vector3 worldPos, float panelW, float panelH,
        float canvasZ, float depth, Camera camera)
    {
        const float margin = 0.01f;

        Vector3 centerVP = camera.WorldToViewportPoint(new Vector3(worldPos.x, worldPos.y, canvasZ));
        Vector3 rightEdgeVP = camera.WorldToViewportPoint(new Vector3(worldPos.x + panelW * .5f, worldPos.y, canvasZ));
        Vector3 topEdgeVP = camera.WorldToViewportPoint(new Vector3(worldPos.x, worldPos.y + panelH * .5f, canvasZ));

        float halfVpW = Mathf.Abs(rightEdgeVP.x - centerVP.x);
        float halfVpH = Mathf.Abs(topEdgeVP.y - centerVP.y);

        centerVP.x = Mathf.Clamp(centerVP.x, halfVpW + margin, 1f - halfVpW - margin);
        centerVP.y = Mathf.Clamp(centerVP.y, halfVpH + margin, 1f - halfVpH - margin);

        // ViewportToWorldPoint needs the distance from the camera, not the world Z.
        Vector3 result = camera.ViewportToWorldPoint(new Vector3(centerVP.x, centerVP.y, depth));
        return new Vector3(result.x, result.y, canvasZ);
    }
}
