using System.Globalization;
using UnityEngine;

public class z : MonoBehaviour
{
    void Start()
    {
        CenterParentAroundChildren();
    }

    void CenterParentAroundChildren()
    {
        GameObject parent = this.gameObject;
        var renderers = parent.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found.");
            return;
        }

        // Step 1: Calculate bounding box in world space
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var r in renderers)
        {
            Bounds b = r.bounds;
            minX = Mathf.Min(minX, b.min.x);
            maxX = Mathf.Max(maxX, b.max.x);
            minY = Mathf.Min(minY, b.min.y);
            maxY = Mathf.Max(maxY, b.max.y);
        }

        Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
        Vector3 newParentPos = new Vector3(center.x, center.y, parent.transform.position.z);

        Debug.Log($"🔵 Set the parent GameObject's position to: {newParentPos}");

        // Step 2: Store and calculate adjustments for child positions
        Transform[] children = new Transform[parent.transform.childCount];

        for (int i = 0; i < children.Length; i++)
        {
            children[i] = parent.transform.GetChild(i);
            Vector3 worldPos = children[i].position;

            // What the child's localPosition would need to be if parent moved
            Vector3 adjustedLocalPos = worldPos - newParentPos;

            Debug.Log($"🟢 Child \"{children[i].name}\" should have localPosition: {adjustedLocalPos}");
        }
    }
}