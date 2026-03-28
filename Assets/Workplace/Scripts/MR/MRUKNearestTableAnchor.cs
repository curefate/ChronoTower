using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using static Meta.XR.MRUtilityKit.MRUKAnchor;   // MRUK Namespace

public class MRUKNearestTableAnchor : MonoBehaviour
{
    [Header("要锚定的物体")]
    public Transform targetTransform;

    [Header("放置偏移量")]
    [Tooltip("物体放置在桌面上的高度偏移")]
    public Vector3 placementOffset = new Vector3(0, 0.05f, 0);

    private Transform player;

    private void Start()
    {
        player = Camera.main.transform;
    }

    public void OnMRUKSceneLoaded()
    {
        Debug.Log("[MRUK] Scene Loaded. 开始查找桌子…");

        var anchors = MRUK.Instance.GetCurrentRoom().Anchors;

        MRUKAnchor nearestTable = FindNearestTable(anchors);

        if (nearestTable != null)
        {
            Debug.Log("[MRUK] 找到最近桌子：" + nearestTable.name);
            PlaceOnTable(nearestTable);
        }
        else
        {
            targetTransform.position = new Vector3(0, 0, 0) + placementOffset; // 默认位置
        }
    }

    // ----------------------------------------------------------
    // 查找最近的桌子（Table 或 Desk）
    // ----------------------------------------------------------
    private MRUKAnchor FindNearestTable(List<MRUKAnchor> anchors)
    {
        float minDist = float.MaxValue;
        MRUKAnchor nearest = null;

        foreach (var a in anchors)
        {
            bool isTable = a.Label == SceneLabels.TABLE;

            if (!isTable) continue;

            float dist = Vector3.Distance(player.position, a.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = a;
            }
        }

        return nearest;
    }

    // ----------------------------------------------------------
    // 将物体放到桌面中心
    // （使用桌子的第一个 plane）
    // ----------------------------------------------------------
    private void PlaceOnTable(MRUKAnchor table)
    {
        Vector3 placePos;

        // 优先使用 PlaneBoundary2D 数据获取桌面中心
        if (table.PlaneBoundary2D != null && table.PlaneBoundary2D.Count > 0)
        {
            // PlaneBoundary2D 的点在本地空间中（2D）
            // 对于水平平面（桌子），应映射为 (x, 0, y)，其中 y 是深度方向
            Vector3 localCenter = Vector3.zero;
            foreach (var point in table.PlaneBoundary2D)
            {
                // 正确的映射：point.x → X轴, 0 → Y轴(平面高度), point.y → Z轴(深度)
                localCenter += new Vector3(point.x, 0, point.y);
            }
            localCenter /= table.PlaneBoundary2D.Count;

            // 转换到世界空间
            placePos = table.transform.TransformPoint(localCenter);
            Debug.Log($"[MRUK] 使用桌子 PlaneBoundary2D 放置物体，本地中心: {localCenter}, 世界位置: {placePos}");
        }
        else
        {
            // 如果没有 plane 信息，直接使用 anchor transform
            placePos = table.transform.position;
            Debug.LogWarning("[MRUK] 桌子没有 plane 信息，直接使用 anchor transform");
        }

        // 应用偏移量
        targetTransform.position = placePos + placementOffset;
        Debug.Log($"[MRUK] 最终放置位置: {targetTransform.position} (偏移: {placementOffset})");
    }
}