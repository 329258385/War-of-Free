using UnityEngine;
using System.Collections;
using Solarmax;
using UnityEngine;

public class ResourcesUtil
{

    public static Texture GetUITexture(string resource)
    {
        Texture tex = Resources.Load<Texture>(resource);
        return tex ;
    }
}

