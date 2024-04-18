using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuController : MonoBehaviour
{
    public UserInterfaceManager userInterfaceMgr;
    public EscapeMenuController inst;

    public KeyCode escapeKey = KeyCode.Escape; // This is hardcode to the escape key. We don't want the player to be able to change this
    public KeyCode altEscapeKey = KeyCode.P; // Alternate escape key cause escape doesn't work in the editor

    public bool isEscapeMenuActive = false;

    void Start()
    {
        inst = this;
    }

    void Update()
    {
        if ((Input.GetKeyDown(escapeKey) || Input.GetKeyDown(altEscapeKey)) && !isEscapeMenuActive)
        {
            userInterfaceMgr.EscapeMenu();
            isEscapeMenuActive = true;
        }
        else if (isEscapeMenuActive) // ! Need to check if we're still in the escape menu
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                userInterfaceMgr.ResumeGame();
                isEscapeMenuActive = false;
            }
        }
    }

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }
}