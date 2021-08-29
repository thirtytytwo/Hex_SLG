using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActorController : MonoBehaviour
{
    //ActorActionSetting
    HexInfo currentMouseStay;
    private bool acceptToClick;//允许点击进行移动
    //地面检测以及鼠标射线检测
    public List<HexInfo> path = new List<HexInfo>();
    private float distance = 30f;

    //直接在Base类中调用Update和Start，避免每个角色通用功能代码调用重复
    private void Start()
    {
        transform.position = MapManager.GetInstance().allHexInfo[new Vector3Int(8, 3, -11)].transform.position + new Vector3(0,1,0);
        acceptToClick = true;
    }

    private void FixedUpdate()
    {
    //Debug.Log(new Vector3(transform.position.x, -1, transform.position.z));
    #region 检测
    //鼠标停留在同一个方块
        if (currentMouseStay == MouseRaySenser() && currentMouseStay != null)
        {
            if (Input.GetMouseButtonDown(0) && acceptToClick)
            {
                acceptToClick = false;
                for (int i = 0; i < path.Count; ++i)
                {
                    transform.position = path[i].transform.position + new Vector3(0, 1, 0);
                    currentMouseStay = path[i];
                }
                acceptToClick = true;
            }
        }
        //鼠标停留发生了变化
        else if(currentMouseStay != MouseRaySenser() && MouseRaySenser() != null)
        {
            //先将上次记录删除
            for (int i = 0; i < path.Count; ++i)
            {
                path[i].isSelected = false;
            }
            path.Clear();
        //当路径为空菜去添加，避免重复添加导致的内存泄露
        if (path.Count == 0)
            {
                Debug.Log(ActorGroundSenser());
                path = MapManager.GetInstance().AStarFindWay(ActorGroundSenser(), MouseRaySenser());
                for (int i = 0; i < path.Count; ++i)
                {
                    path[i].isSelected = true;
                }
    
            }
            currentMouseStay = MouseRaySenser();
        }
        else
        {
        
        }
    #endregion
    }
    #region 射线检测类
    //地面检测，并且传回HexInfo类
    private HexInfo ActorGroundSenser()
    {
        Debug.Log("调用角色检测");
        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,10f))
        {
            return hit.collider.transform.gameObject.GetComponent<HexInfo>();
        }
        else
        {
            //Debug.LogWarning("没有检测到地面");
            return null;
        }
    }
    private HexInfo MouseRaySenser()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,1000f))
        {
            return hit.collider.transform.gameObject.GetComponent<HexInfo>();
        }
        else
        {
            //Debug.Log("鼠标没检测到物体");
            return null;
        }
    }
    #endregion

    #region 角色移动

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(transform.position, -Vector3.up));
        Gizmos.color = Color.red;
    }
}
