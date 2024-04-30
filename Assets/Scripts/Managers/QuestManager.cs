using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager inst;
    private void Awake(){
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private UserInterfaceManager userInterfaceMgr;

    public void setUserInterfaceManager(UserInterfaceManager uim)
    {
        userInterfaceMgr = uim;
    }

    private QuestData currQuest;
    private bool questActive = false;
    private bool questComplete = false;

    private bool playerNearby = false;
    private bool playerFoundItem = false;
    private string itemName = null;
    private int playerInteractions = 0;

    public bool PlayerNearby { get; set; }

    public void CharacterLoaded(QuestData quest)
    {
        currQuest = quest;
        questActive = false;
        questComplete = false;
        playerFoundItem = false;
        itemName = null;
        playerInteractions = 0;
    }

    public void ShowDialogue()
    {
        PlayerNearby = true;

        if(!questActive && !questComplete)
        {
            switch (playerInteractions)
            {
                case 0:
                    userInterfaceMgr.UpdateQuestText(currQuest.greeting);
                    break;

                case 1:
                    userInterfaceMgr.UpdateQuestText(currQuest.charIntroduction);
                    break;

                case 2:
                    userInterfaceMgr.UpdateQuestText(currQuest.questIntroduction);
                    questActive = true;
                    playerInteractions = 0;
                    break;

                default:
                    userInterfaceMgr.UpdateQuestText(null);
                    break;
            }
        }else if (questActive)
        {
            if (!playerFoundItem)
            {
                userInterfaceMgr.UpdateQuestText(currQuest.activeQuestNoItem);
            }else if (itemName == currQuest.itemNeeded)
            {
                switch (playerInteractions)
                {
                    case 0:
                        userInterfaceMgr.UpdateQuestText(currQuest.itemRetrieved);
                        break;

                    case 1:
                        userInterfaceMgr.UpdateQuestText(currQuest.weaponIntroduction);
                        // TODO: add call to function to grant character access to currQuest.rewardWeapon
                        break;

                    case 2:
                        userInterfaceMgr.UpdateQuestText(currQuest.questComplete);
                        questActive = false;
                        questComplete = true;
                        playerInteractions = 0;
                        break;

                    default:
                        userInterfaceMgr.UpdateQuestText(null);
                        break;
                }
            }
        }
        else
        {
            userInterfaceMgr.UpdateQuestText(currQuest.questComplete);
        }

        userInterfaceMgr.ShowQuestDialogue();
    }

    public void HideDialogue()
    {
        PlayerNearby = false;
        userInterfaceMgr.HideQuestDialogue();
    }

    public void PlayerInteraction()
    {
        if (!questActive && !questComplete)
        {
            playerInteractions++;
            ShowDialogue();
        }else if(questActive && playerFoundItem)
        {
            playerInteractions++;
            ShowDialogue();
        }
    }

    public void ItemPickedUp(string name)
    {
        playerFoundItem = true;
        itemName = name;
    }
}
