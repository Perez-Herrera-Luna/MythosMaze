using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public UserInterfaceManager userInterfaceMgr;
    public OptionsMenuController inst;

    void Start()
    {
        inst = this;
    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    public void BackButton()
    {
        userInterfaceMgr.MainMenu();
    }
}
