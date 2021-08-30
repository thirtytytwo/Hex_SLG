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
    //移动参数
    protected float speed = 2f;

    [Header("信号及开关")]
    public bool isThisActorRound;//当是这个角色的回合
    public bool isMoving;//角色移动中

    //直接在Base类中调用Update和Start，避免每个角色通用功能代码调用重复
    private void Start()
    {
        transform.position = MapManager.GetInstance().allHexInfo[new Vector3Int(8, 3, -11)].transform.position + new Vector3(0,1,0);
        acceptToClick = true;
    }

    private void FixedUpdate()
    {
        //当是你的回合是才去检测，假设都选择移动
        if (isThisActorRound)
        {
             //鼠标停留在同一个方块
            if (currentMouseStay == MouseRaySenser() && currentMouseStay != null)
            {
                if (Input.GetMouseButtonDown(0) && acceptToClick)
                {
                    ActorMove();
                }
            }
            //鼠标停留发生了变化
            else if(currentMouseStay != MouseRaySenser() && MouseRaySenser() != null && !isMoving)
            {
                try
                {
                    //先将上次记录删除
                    for (int i = 0; i < path.Count; ++i)
                    {
                        path[i].isSelected = false;
                    }
                    path.Clear();
                }
                catch (System.Exception)
                {
                    path.Clear();
                }
            //当路径为空菜去添加，避免重复添加导致的内存泄露
            if (path.Count == 0)
                {
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
        }
    }
    #region 射线检测类
    //地面检测，并且传回HexInfo类
    private HexInfo ActorGroundSenser()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,10f))
        {
            return hit.collider.transform.gameObject.GetComponent<HexInfo>();
        }
        else
        {
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
            return null;
        }
    }
    #endregion

    #region 角色移动
    protected void ActorMove()
    {
        StartCoroutine(Move(path.ToArray(), 0));
    }
    IEnumerator Move(HexInfo[] _path, int index)
    {
        while(transform.position != _path[_path.Length - 1].transform.position)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, _path[index].transform.position + new Vector3(0, 1, 0), speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, _path[index].transform.position + new Vector3(0, 1, 0)) <= 0.2f)
            {
                index++;
            }
            yield return null;
        }
        isMoving = false;
        yield return null;

    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(transform.position, -Vector3.up));
        Gizmos.color = Color.red;
    }
}
