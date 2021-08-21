using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerater : MonoBehaviour
{
    [Header("AbouHex")]
    public Vector2Int mapSize;

    public GameObject hexPrefeb;

    public float offset = 1.73f;

    [Range(1f, 1.5f)] public float hexScalePercent;

    [Header("AboutObstacle")]

    public GameObject obstaclePrefeb;

    [Range(0f, 0.2f)] public float obstaclePercent;//障碍占比

    private void Awake()
    {
        MapManager.GetInstance().GenerateMap(mapSize, offset, hexPrefeb, hexScalePercent);
    }

}
