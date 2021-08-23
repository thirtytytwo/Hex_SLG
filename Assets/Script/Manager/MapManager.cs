using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : BaseManagerToMono<MapManager>
{
    public Dictionary<Vector3Int, HexInfo> allHexInfo = new Dictionary<Vector3Int, HexInfo>();//新建字典，用于储存生成瓦片的信息
    public List<HexInfo> allHexInfoByList = new List<HexInfo>();//同样存储了所有Hex的列表，用于其他脚本的调用

    //A*
    private List<HexInfo> firstPickList = new List<HexInfo>();//第一次筛选的列表
    public List<HexInfo> secondPickList = new List<HexInfo>();//第二次筛选的列表
    private Vector3Int[] aroundHex = { new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1) };

    #region 生成网格方法

    //全部生成
    public void GenerateMap(Vector2Int _mapSize, float _zOffset, GameObject _prefeb, float hexPercent)
    {
        for (int i = 0; i < _mapSize.y; i++)
        {//列
            for (int j = 0; j < _mapSize.x; j++)
            {//排
                Vector3 instantiatePos = new Vector3((i + 1) % 2 == 0 ? 2 * j : 2 * j - 1, 0, _zOffset * i);
                GameObject hex = Instantiate<GameObject>(_prefeb, instantiatePos, Quaternion.Euler(-90.0f, 0, 0));
                hex.AddComponent<HexInfo>().SetCoordInfo(ChangeCoordinateToHexCoordinate(1, new Vector2Int(j, -i)));
                hex.transform.localScale *= hexPercent;
                allHexInfo.Add(hex.GetComponent<HexInfo>().GetInfo(), hex.GetComponent<HexInfo>());
                allHexInfoByList.Add(hex.GetComponent<HexInfo>());
            }
        }
    }

    //随机地点生成
    public void GenerateRandomMap(Vector2Int _mapSize,GameObject prefeb, float percent)
    {
        Queue<HexInfo> randomCoord = new Queue<HexInfo>(RandomCoord(allHexInfoByList.ToArray()));
        for (int i = 0; i < (_mapSize.x * _mapSize.y * percent); i++)
        {
            HexInfo target = randomCoord.Dequeue();
            GameObject obstacle = Instantiate<GameObject>(prefeb, target.transform.position, Quaternion.Euler(-90.0f, 0, 0));
            obstacle.transform.localScale *= (percent + 0.001f);
            randomCoord.Enqueue(target);
        }
    }

    public HexInfo[] RandomCoord(HexInfo[] _array)
    {
        for (int i = 0; i < _array.Length; i++)
        {
            int randomIndex = Random.Range(i, _array.Length);
            HexInfo temp = _array[randomIndex];
            _array[randomIndex] = _array[i];
            _array[i] = temp;
        }
        return _array;
    }

    #endregion
    #region 转换坐标方法
    private Vector3Int ChangeCoordinateToHexCoordinate(int offset, Vector2 coord)
    {
        int x = (int)coord.x - (int)Mathf.Ceil((coord.y + offset * (coord.y % 2 == 0 ? 1.0f : 0.0f)) / 2);
        int z = (int)coord.y;
        int y = 0 - x - z;
        return new Vector3Int(x, y, z);
    }
    #endregion

    #region 获取坐标范围内网格方法
    public List<HexInfo> CheckHexToMovable(int invaliableRange, HexInfo current)
    {
        List<HexInfo> _range = new List<HexInfo>();

        foreach (var item in allHexInfo)
        {
            if (Mathf.Abs(item.Key.x - current.GetInfo().x) <= invaliableRange)
            {
                if (Mathf.Abs(item.Key.y - current.GetInfo().y) <= invaliableRange)
                {
                    if (Mathf.Abs(item.Key.z - current.GetInfo().z) <= invaliableRange)
                    {
                        _range.Add(item.Value);
                    }
                }
            }
        }
        return _range;
    }
    #endregion
    #region A*寻路
    public List<HexInfo> AStarFindWay(HexInfo _start, HexInfo end)
    {
        //清空上次使用痕迹
        firstPickList.Clear();
        secondPickList.Clear();
        //如果使用者可以不点到Hex上
        if (end == null)
        {
            return null;
        }
        //对开始点进行设置，并将开始点放入
         if(_start.father == null)
         {
 
         }
         else
         {
             _start.father = null;
         }
        _start.f = 0;
        _start.g = 0;
        _start.h = 0;
        secondPickList.Add(_start);
        //对周围进行检测
        while (true)
        {
            //左
            CheckAround(_start.GetInfo() - aroundHex[0], 1, _start, end);
            //左上
            CheckAround(_start.GetInfo() - aroundHex[1], 1, _start, end);
            //右上
            CheckAround(_start.GetInfo() - aroundHex[2], 1, _start, end);
            //右
            CheckAround(_start.GetInfo() - aroundHex[3], 1, _start, end);
            //右下
            CheckAround(_start.GetInfo() - aroundHex[4], 1, _start, end);
            //左下
            CheckAround(_start.GetInfo() - aroundHex[5], 1, _start, end);

            //找不到出路
            if (firstPickList.Count == 0)
            {
                return null;
            }
            //选出离目标点最近的点
            firstPickList.Sort(FirstPickSort);
            //找到这个点，并令他为start,存入closeList中,并在openList中移除
            secondPickList.Add(firstPickList[0]);
            _start = firstPickList[0];
            firstPickList.RemoveAt(0);
            //每次循环后判断时候已经找到目标点
            if (_start == end)
            {
                List<HexInfo> path = new List<HexInfo>();
                path.Add(end);
                while (true)
                {
                    if (end.father != null)
                    {
                        end = end.father;
                        path.Add(end);
                    }
                    else if(end.father == null)
                    {
                        path.Reverse();
                        return path;
                    }
                }
            }
        }
    }
    private int FirstPickSort(HexInfo a, HexInfo b)
    {
        if (a.f > b.f)
        {
            return 1;
        }
        else if (a.f == b.f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
    private void CheckAround(Vector3Int _coord, int g, HexInfo start, HexInfo end)
    {
        //边界检测
        if (allHexInfo.ContainsKey(_coord))
        {
            HexInfo node = allHexInfo[_coord];
            //判断是否是障碍物，是否已经被搜索过
            if (node.isObstacle || secondPickList.Contains(node) || firstPickList.Contains(node))
            {
                return;
            }
            else
            {
                //设置node父亲节点
                node.father = start;
                //计算F = G + H 值
                node.g = node.father.g + g;
                node.h = Mathf.Abs(end.GetInfo().x - node.GetInfo().x) + Mathf.Abs(end.GetInfo().y - node.GetInfo().y) + Mathf.Abs(end.GetInfo().z - node.GetInfo().z);
                node.f = node.g + node.h;

                //计算好之后存入OpenList
                firstPickList.Add(node);
            }
        }
        else
        {
            return;
        }
    }
    #endregion
}