using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfo : MonoBehaviour
{
    //Self
    [SerializeField]private Vector3Int coord;

    public bool isObstacle;//标记不可行走的
    //A*
    public int f;//总距离

    public int g;//离起点的距离

    public int h;//离终点的距离

    public HexInfo father;

    public void SetCoordInfo(Vector3Int _coord)
    {
        coord = _coord;
    }
    public Vector3Int GetInfo()
    {
        return coord;
    }
}
