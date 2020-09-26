using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Solarmax;






public class GuideTipCell : MonoBehaviour
{
    public GameObject   build;
    public UILabel      desc;
    public GameObject   bg;
    public GameObject   line1;
    public GameObject   line2;

    
    public void SetLevelCell( string sprite, string msg, bool right )
    {
        string url          = "SpritesIcon/" + sprite;
        desc.text           = msg;
        LoadSprite(url);
        if ( right )
        {
            bg.transform.localPosition       = new Vector3(405, -241, 0);
            bg.transform.localEulerAngles    = new Vector3(0, 0, 0);
            line1.transform.localPosition    = new Vector3(74, -81, 0 );
            line1.transform.localEulerAngles = new Vector3(0, 0, 135);
            line2.transform.localPosition    = new Vector3(145, -114, 0);
            line2.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            bg.transform.localPosition       = new Vector3(-437, -227, 0);
            bg.transform.localEulerAngles    = new Vector3(0, 0, 0);
            line1.transform.localPosition    = new Vector3(-83.3f, -65.8f, 0 );
            line1.transform.localEulerAngles = new Vector3(0, 0, 45);
            line2.transform.localPosition    = new Vector3(-154, -98, 0);
            line2.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    private void LoadSprite( string url )
    {
        build.GetComponent<UITexture>().mainTexture = ResourcesUtil.GetUITexture(url);
    }
}

