using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCharacter : MonoBehaviour
{
    public QuestData quest;

    private Transform player;
    private Arena currArena;

    private float awarenessRadius = 10;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        QuestManager.inst.CharacterLoaded(quest, currArena);
    }

    // Update is called once per frame
    void Update()
    {
        if (currArena.ArenaActive)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < awarenessRadius)
            {
                if (!QuestManager.inst.PlayerNearby)
                {
                    QuestManager.inst.PlayerNearby = true;
                    QuestManager.inst.ShowDialogue();
                }
            }
            else
            {
                if (QuestManager.inst.PlayerNearby)
                {
                    QuestManager.inst.PlayerNearby = false;
                    QuestManager.inst.HideDialogue();
                }
            }
        }
    }

    public void SetArenaConnection(Arena a)
    {
        currArena = a;
    }
}
