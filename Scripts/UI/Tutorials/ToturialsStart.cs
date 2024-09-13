using System;
using System.Collections.Generic;
using UnityEngine;


public class ToturialsStart : MonoBehaviour
{
   public void StartToturials()
    {
        ToturialsManager.Instance.ToturialsStart();
        Destroy(gameObject);
    }

    public void SkipToturials()
    {
        DataManager.Instance.userInfo.isUserTutorials = true;
        ToturialsManager.Instance.ToturialsStart();
        Destroy(gameObject);
    }
}

