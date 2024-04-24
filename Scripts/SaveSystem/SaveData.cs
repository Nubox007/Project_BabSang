using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.AI;


[Serializable]
public class SaveData : MonoBehaviour
{
    public static int idCount = 0;
    public Dictionary<string,PlaceableObjectData> placeObjectDatas = new Dictionary<string, PlaceableObjectData>();

    public static string GeneratieId()
    {
            ++idCount;
            return idCount.ToString();
    }

    public void AddData(Data _data)
    {
        if(_data is PlaceableObjectData plObjData)
        {
            if(placeObjectDatas.ContainsKey(plObjData.Id))
            {
                placeObjectDatas[plObjData.Id] = plObjData;
            }
            else
            {
                placeObjectDatas.Add(plObjData.Id, plObjData);
            }
        }
    }

    public void RemoveData(Data _data)
    {        
        if(_data is PlaceableObjectData plObjData)
        {
            if(placeObjectDatas.ContainsKey(plObjData.Id))
            {
                placeObjectDatas.Remove(plObjData.Id);
            }
        }
    }

    [OnDeserialized] 
    internal void OnDeserialized(StreamingContext _context)
    {
        placeObjectDatas ??= new Dictionary<string, PlaceableObjectData>();
    }
}
