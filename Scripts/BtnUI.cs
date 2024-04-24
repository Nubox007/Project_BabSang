using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnUI : MonoBehaviour
{
    public delegate void OnclickDelegate(BtnUI go);
    private OnclickDelegate onClickCallback = null;
    public OnclickDelegate OnClickCallback
    {
        set { onClickCallback = value; }
    }
    

    private Button Btn = null;
    private Image image = null;

    private void Awake()
    {
        image = this.GetComponent<Image>();
        Btn = GetComponent<Button>();

    }

    private void Start()
    {
        Debug.Log(image.sprite.name);
 
    }
    public void SetSprite(Sprite _name)
    {
        image.sprite = _name;
    }

    public void OnClickEvent()
    {
        onClickCallback?.Invoke(this);
    }

}
