using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{

    [SerializeField] private int id = 0;

    [Unity.Collections.ReadOnly()] public PlaceableObjectData data = new PlaceableObjectData();
    public bool Placed { get; private set; }
    public BoundsInt area;

    public void Initialize(GameObject _target)
    {
        data.assetName = _target.gameObject.name;
        data.Id = SaveData.GenerateId();
            }

    public void Initialize(PlaceableObjectData _objectData)
    {
        data = _objectData;
    }
    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.current.CanTakeArea(areaTemp))
        {
            return true;
        }
        return false;
    }

    public void Place()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        Placed = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
    }

    private void OnDisable()
    {
        data.position = transform.position;
        //todo add to datas;
    }
}
