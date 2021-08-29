using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfo : MonoBehaviour
{
    //Self
    private Vector3Int coord;

    public bool isObstacle;//标记不可行走的
    
    public bool isSelected;//监听是否被寻路选中
    //A*
    public int f;//总距离

    public int g;//离起点的距离

    public int h;//离终点的距离

    public HexInfo father;
    
    private void Update()
    {
        //实时监听
        if (isSelected)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            StartCoroutine(SelectHex());
        }
    }

    public void SetCoordInfo(Vector3Int _coord)
    {
        coord = _coord;
    }
    public Vector3Int GetInfo()
    {
        return coord;
    }

    IEnumerator SelectHex()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

    }
}
