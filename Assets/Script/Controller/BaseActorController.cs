using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActorController : MonoBehaviour
{
    //ActorActionSetting
    //地面检测以及鼠标射线检测
    List<HexInfo> path = new List<HexInfo>();
    private float distance = 30f;
    HexInfo currentMouseStay;

    //直接在Base类中调用Update和Start，避免每个角色通用功能代码调用重复
    private void Start()
    {
        transform.position = MapManager.GetInstance().allHexInfo[new Vector3Int(8, 3, -11)].transform.position + new Vector3(0,2,0);
    }

    private void FixedUpdate()
    {
        Debug.Log(Input.mousePosition);
        #region 检测
         //鼠标停留发生了变化
            if(currentMouseStay == MouseRaySenser() && currentMouseStay != null)
            {
                
            }
            if(currentMouseStay != MouseRaySenser() && MouseRaySenser() != null)
            {
                path = MapManager.GetInstance().AStarFindWay(ActorGroundSenser(), MouseRaySenser());
                currentMouseStay = MouseRaySenser();
            }
            else
            {
                Debug.Log("没有检测到物体");
            }
        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count; ++i)
            {
                path[i].transform.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
        #endregion
    }
    #region 射线检测类
    //地面检测，并且传回HexInfo类
    private HexInfo ActorGroundSenser()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,-Vector3.up,out hit, distance) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return hit.collider.transform.gameObject.GetComponent<HexInfo>();
        }
        else
        {
            Debug.LogWarning("没有检测到地面");
            return null;
        }
    }
    private HexInfo MouseRaySenser()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return hit.collider.transform.gameObject.GetComponent<HexInfo>();
        }
        else
        {
            Debug.LogWarning("鼠标没有检测到地面");
            return null;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition));
        Gizmos.color = Color.red;
    }
}
