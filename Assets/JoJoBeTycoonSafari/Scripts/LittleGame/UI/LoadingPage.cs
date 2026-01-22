using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载界面
/// </summary>
public class LoadingPage : UIPage
{

    public LoadingPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "";
    }


}
