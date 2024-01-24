using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr inst;
    private void Awake()
    {
        inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // to be called from control manager when player presses certain key (probably 'e')
    public void OpenInventory()
    {
        // load inventory as an overlay to player view
        SceneManager.LoadScene("Inventory", LoadSceneMode.Additive);
    }
}
