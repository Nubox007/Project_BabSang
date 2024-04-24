using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUIList : MonoBehaviour
{

    private Building[] sturctures = null;
    [SerializeField] private GameObject Button = null;

    private Dictionary<BtnUI, Building> InstanceBuilding = new Dictionary<BtnUI, Building>();



    private void Awake()
    {
        sturctures = Resources.LoadAll<Building>("Prefabs");
    }

    private void Start()
    {
        foreach (Building b in sturctures) 
        {
            GameObject go = Instantiate(Button, transform);
            BtnUI btn = go.GetComponentInChildren<BtnUI>();
            btn.SetSprite(b.GetComponentInChildren<SpriteRenderer>().sprite);
            Debug.Log("foreach btn: " + btn.name);
            btn.OnClickCallback = OnclickCallback;
            InstanceBuilding.Add(btn, b);
        }
    }

    public void OnclickCallback(BtnUI btn)
    {
        if (InstanceBuilding.ContainsKey(btn))
            GridBuildingSystem.current.InstanceBuilding(InstanceBuilding[btn].gameObject);
    }

}
