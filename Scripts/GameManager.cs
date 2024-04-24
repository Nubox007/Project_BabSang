using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    public SaveData saveData;
    public static GameManager current;
    [SerializeField] private GameObject go = null;
    
    private void Awake()
    {
        SaveSystem.Initalize();
        current = this;
    }


    private void Start()
    {

        saveData = SaveSystem.Load();
        LoadGame();
    }
    private void LoadGame()
    {
        LoadPlacealbeObjects();
    }

    private void LoadPlacealbeObjects()
    {

        foreach(var plObjData in saveData.placeObjectDatas.Values)
        {
            try
            {
                Building building = go.GetComponent<Building>();
                building.Initialize(plObjData);
                building.Load();
            }catch(Exception e)
            {

            }

        }
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save(saveData);
    }

}
