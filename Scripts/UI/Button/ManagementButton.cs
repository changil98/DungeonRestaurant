using System.Collections;
using System.Collections.Generic;
using System.Management;
using UnityEngine;

public class ManagementButton : MonoBehaviour
{
    public GameObject Management;
    private GameObject managementInstance;

    public void OnClickManagementButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 30)
        {
            ToturialsManager.Instance.isClear[13] = true;
            ToturialsManager.Instance.OnNextPhase();
        }

        if (managementInstance == null)
        {
            managementInstance = Instantiate(Management);
        }
        else
        {
            if (managementInstance.activeSelf == false)
            {
                managementInstance.SetActive(true);
                FindObjectOfType<ManageSlotInstance>().Refresh();
            }
        }
    }
}
