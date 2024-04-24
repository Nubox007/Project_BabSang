using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public enum TileType
    { Empty, White, Green, Red }
    
    [SerializeField] internal GridLayout gridLayout;
    [SerializeField] private Tilemap TempTileMap;           // 배치가 되는 타일맵
    [SerializeField] private Tilemap MainTileMap;           // 배치를 확인하는 타일맵 
    [SerializeField] private List<TileBase> tiles = null;   //배치상태를 구분할 타일들
    //[SerializeField] private BuildUIList list = null;

    public static GridBuildingSystem current;
    //building 스크립트와 싱글턴 연결용
    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();
    //타일의 상태를 구분할 수 있는 Dictionary 형태의 데이터
    private Building temp = null; //Instantiate로 복사한 물체의 Building 스크립트를 받아옴
    //private Vector3 prevPos = Vector3.zero; 
    private BoundsInt prevArea ; // 그리드의 크기를 지정
    //public SaveData saveData; 

  
    private void Awake()
    {
        current = this;

        //SaveSystem.Initialize();
    }
    /// <summary>
    /// 인스펙터에서 지정한 타일들로 상태를 지정
    /// Dictionary에 지정된 타일을 넣어줌
    /// </summary>
    private void Start()
    {        
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.Green, tiles[0]);
        tileBases.Add(TileType.Red, tiles[1]);
        tileBases.Add(TileType.White, tiles[2]);
    }
    private void Update()
    {
        
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int _cellPos = gridLayout.LocalToCell(touchPos); 


        // 마우스 위치에 따라 건물의 위치를 그리드 셀에 맞춰서 업데이트 시켜준다.
        if(temp) 
        {
            temp.transform.localPosition = gridLayout.CellToLocalInterpolated(_cellPos + new Vector3(0.5f, 0.5f, 0f));
            FollowBuilding();
        }
        if (Input.GetMouseButtonDown(0))
        {
            //마우스 포인터가 UI위에 있는지 판별
            //if (EventSystem.current.IsPointerOverGameObject(0)) return;           

            //현재 소환한 건물이 없으면 건물을 새로 복사한다.
                        

            // if (!temp.Placed)
            // {
                // if (prevPos != _cellPos)
                // {
                //     temp.transform.localPosition = gridLayout.CellToLocalInterpolated(_cellPos + new Vector3(0.5f, 0.5f, 0f));
                //     prevPos = _cellPos;
                //     FollowBuilding();
                // }
            // }
        }
        ///소환한 건물을 temptilemap에 설치
        else if (Input.GetMouseButtonDown(1))
        {
            if(!temp) return;
            if (temp.CanBePlaced()) 
            {
                temp.Place();
                temp = null;
            }
           
        }
        // 건물에 지정된 그리드 크기를 가져와서 XY 값을 서로 바꿔준다.(그리드 회전 용)
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if(!temp) return;
            int tmp = 0;
            Vector3Int newBounds = temp.area.size; // 새로운 크기를 설정하여 새로운 BoundsInt를 생성합니다.
            tmp =newBounds.x;
            newBounds.x = newBounds.y;
            newBounds.y =tmp;
            temp.area.size = newBounds;          
           
        }
        
  
    }  

    public void InstanceBuilding(GameObject go)
    {
        if (!temp) temp = Instantiate(go, Input.mousePosition, Quaternion.identity).GetComponent<Building>();
        if (temp)
        {
            Destroy(temp.gameObject);
            temp = Instantiate(go, Input.mousePosition, Quaternion.identity).GetComponent<Building>();
        }    
    }


    /// <summary>
    /// 해당 구역에 대한 초기화
    /// TempTileMap에 대해서 초기화를 진행한다.
    /// </summary>
    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTileMap.SetTilesBlock(prevArea, toClear);

    }

    /// <summary>
    /// temp에 생성된 gameobject의 위치에 따라 그리드레이아웃의 셀 위치로 지정된다.
    /// 건물의 크기를 받아 MainTileMap에 대한 정보를 받아와 건물이 설치할 수 있는지 없는지 
    /// TempTileMap에 색깔타일을 생성하여 표시한다.
    /// </summary>
    private void FollowBuilding()
    {
        ClearArea();

        temp.area.position = gridLayout.WorldToCell(temp.gameObject.transform.position);
        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTileBlock(buildingArea, MainTileMap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; ++i)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                break;
           }
        }

        TempTileMap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
        
    }

    

    /// <summary>
    /// 메인 타일맵을 기준으로 해당 그리드 위치에 배치할 수 있는지 판별
    /// </summary>
    /// <param name="area">해당 물체의 그리드 크기 BoundsInt.size</param>
    /// <returns>참 거짓</returns>
    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTileBlock(area, MainTileMap);
        foreach (TileBase tile in baseArray) 
        {
            if (tile != tileBases[TileType.White])
            {
                Debug.Log("Cannot place here");
                return false;
            }
        }

        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTileBlock(area, TileType.White, TempTileMap);
        SetTileBlock(area, TileType.Green, MainTileMap);
    }

    private static TileBase[] GetTileBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (Vector3Int v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void SetTileBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }


    /// <summary>
    /// 입력받은 타일의 배열과 지정된 형태를 받으면 해당 형태로 채워준다.
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="type"></param>
    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; ++i)
        {
            arr[i] = tileBases[type];
        }
    }
}
