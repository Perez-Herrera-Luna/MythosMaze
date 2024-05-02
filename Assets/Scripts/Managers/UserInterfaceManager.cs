using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UserInterfaceManager : MonoBehaviour
{
    public static UserInterfaceManager inst;
    // public SceneManager sceneMgr;
    // public GameManager gameMgr;
    public MainMenuController mainMenuController;
    public OptionsMenuController optionsMenuController;
    public EscapeMenuController escapeMenuController;

    public GameObject canvas;
    public GameObject background;
    public GameObject escapeMenuBackground;
    public GameObject loadingScreen;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject escapeMenu;
    public GameObject keyRebindingMenu;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
    public GameObject playerGameUI;
    public GameObject playerDamageScreen;
    public PlayerHealthBar healthBar;
    public UnityEngine.UI.Slider progressBar;
    public UnityEngine.UI.Slider attackIndicatorSlider;
    public UnityEngine.UI.Slider dashIndicatorSlider;
    public GameObject questDialogue;
    public TMP_Text questDialogueText;
    public GameObject questPanel;

    private IEnumerator Attack_Indicator_Coroutine;

    void Awake()
    {
        // inst = this;
        if(inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        background = GameObject.Find("Background");
        escapeMenuBackground = GameObject.Find("EscapeMenuBackground");
        mainMenu = GameObject.Find("MainMenu");
        escapeMenu = GameObject.Find("EscapeMenu");
        keyRebindingMenu = GameObject.Find("KeyRebindingMenu");
        optionsMenu = GameObject.Find("OptionsMenu");
        loadingScreen = GameObject.Find("LoadingScreen");
        gameOverMenu = GameObject.Find("GameOverMenu");
        gameWonMenu = GameObject.Find("GameWinMenu");
        playerGameUI = GameObject.Find("PlayerGameUI");
        playerDamageScreen = GameObject.Find("PlayerDamageScreen");
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();
        healthBar = GameObject.Find("Health bar").GetComponent<PlayerHealthBar>();
        attackIndicatorSlider = GameObject.Find("AttackIndicatorSlider").GetComponent<Slider>();
        dashIndicatorSlider = GameObject.Find("DashIndicatorSlider").GetComponent<Slider>();

        questDialogue = GameObject.Find("QuestDialogue");
        questDialogueText = GameObject.Find("DialogueText").GetComponent<TMP_Text>();
        questPanel = GameObject.Find("DialoguePanel");
        questPanel.SetActive(false);
        questDialogueText.text = null;

        mainMenuController = mainMenu.GetComponent<MainMenuController>();
        optionsMenuController = optionsMenu.GetComponent<OptionsMenuController>();
        escapeMenuController = inst.GetComponent<EscapeMenuController>();
        mainMenuController.setUserInterfaceManager(inst);
        optionsMenuController.setUserInterfaceManager(inst);
        escapeMenuController.setUserInterfaceManager(inst);

        EnableMenuElement(mainMenu);

        // sceneMgr = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        // gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        SceneManager.inst.setUserInterfaceManager(inst);
        GameManager.inst.setUserInterfaceManager(inst);
        QuestManager.inst.setUserInterfaceManager(inst);
    }

    public void MainMenu()
    {
        EnableMenuElement(mainMenu);
    }

    public void OptionsMenu()
    {
        EnableMenuElement(optionsMenu);
        // optionsMenuController.UpdateOptions();
    }

    public void OptionsMenuPaused()
    {
        EnableMenuElement(optionsMenu, false);
        // optionsMenuController.UpdateOptions();
        escapeMenuController.LeftEscapeMenu();
    }

    public void OptionsMenuBackPaused()
    {
        EnableMenuElement(escapeMenu, false);
        escapeMenuController.ReturnedToEscapeMenu();
    }

    public void GameLoading()
    {
        EnableMenuElement(loadingScreen);
    }

    public void GameWon()
    {
        EnableMenuElement(gameWonMenu);
    }

    public void GameOver()
    {
        EnableMenuElement(gameOverMenu);
    }

    public void BackToMainMenu()
    {
        EnableMenuElement(mainMenu);
        
        questPanel.SetActive(false);
        questDialogueText.text = null;

        GameManager.inst.BackToMainMenu();
    }

    public void KeyRebinding()
    {
        EnableMenuElement(keyRebindingMenu);
    }

    public void KeyRebindingBackButton()
    {
        // EnableMenuElement(optionsMenu);
        if (GameManager.inst.isGamePaused)
        {
            EnableMenuElement(optionsMenu, false);
        }
        else
        {
            EnableMenuElement(optionsMenu);
        }
    }

    public void EscapeMenu()
    {
        GameManager.inst.PauseGame();
        EnableMenuElement(escapeMenu, false);
    }

    public void ResumeGame()
    {
        canvas.SetActive(true);
        background.SetActive(false);
        escapeMenuBackground.SetActive(false);
        escapeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);
        keyRebindingMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameWonMenu.SetActive(false);
        playerDamageScreen.SetActive(false);
        playerGameUI.SetActive(true);

        GameManager.inst.ResumeGame();
    }

    public void GameStart()
    {
        loadingScreen.SetActive(false);
        background.SetActive(false);
        playerGameUI.SetActive(true);
    }

    public void UpdateQuestText(string newText)
    {
        // string displayText = newText + "\nPress 'Attack' to continue";
        // questDialogueText.text = displayText;
        questDialogueText.text = newText;
    }

    public void ShowQuestDialogue()
    {
        if (questDialogueText.text != null)
            questDialogue.SetActive(true);
            questPanel.SetActive(true); // Shows the panel behind the dialogue text
    }

    public void HideQuestDialogue()
    {
        questDialogueText.text = null;
        questDialogue.SetActive(false);
        questPanel.SetActive(false); // Hides the panel behind the dialogue text
    }

    public void DisplayAttackIndicator(float attackCooldown)
    {
        if (attackCooldown < 0)
        {
            attackCooldown = 0;
        }

        if (Attack_Indicator_Coroutine != null)
        {
            StopCoroutine(Attack_Indicator_Coroutine);
        }

        Attack_Indicator_Coroutine = AnimateAttackIndicator(attackCooldown);
        StartCoroutine(Attack_Indicator_Coroutine);
    }

    IEnumerator AnimateAttackIndicator(float attackCooldown)
    {
        if (attackCooldown <= 0)
        {
            attackIndicatorSlider.value = 1;
            yield break;
        }

        attackIndicatorSlider.value = 0;
        float startCooldown = attackCooldown;
        float timer = 0;

        while (timer < attackCooldown)
        {
            attackIndicatorSlider.value = Mathf.Lerp(0, 1, timer / attackCooldown);
            timer += Time.deltaTime;
            yield return null;
        }

        attackIndicatorSlider.value = 1;
    }

    public void DisplayDashIndicator(float dashCooldownMax, float dashCooldown)
    {
        if (dashCooldown < 0)
        {
            dashCooldown = 0;
        }

        float dashCooldownPercentage = 1 - (dashCooldown / dashCooldownMax);

        dashIndicatorSlider.value = dashCooldownPercentage;
    }

    public void DisplayDamage(float health)
    {
        // canvas.SetActive(true);
        Debug.Log("Damage!");
        healthBar.SetHealth(health);
    }

    public void HideDamage()
    {
        // canvas.SetActive(false);
    }

    public void DisplayWeaponBuff(float amount)
    {
        // TODO : implement visual display of weapon speed/cooldown powerup
        Debug.Log("UI Mgr Weapon Buff");
    }

    public void DisplayWeaponDamageBuff(float amount)
    {
        // TODO : implement visual display of weapon damage powerup
        Debug.Log("UI Mgr Weapon Damage Buff");
    }

    public void DisplayMaxHealthBuff(float maxHealth)
    {
        // TODO : implement visual display of max health increase
    }

    public void DisplayHealing(float health)
    {
        healthBar.SetHealth(health);
    }

    public void DisplayPlayerDefenseBuff(float amount)
    {
        // TODO : implement visual display of absolute defense powerup
    }

    public void DisplayPlayerSpeedBuff(float amount)
    {
        // TODO : implement visual display of move speed powerup
    }

    public void DisplayPlayerDashBuff(float amount)
    {
        // TODO : implement visual display of dash cooldown powerup
    }

    public void DisplayPlayerJumpBuff(float amount)
    {
        // TODO : implement visual display of jump force powerup
    }

    public void updateProgressBar(float progress)
    {
        progressBar.value = progress;
    }

    public void LoadFirstLevel()
    {
        SceneManager.inst.LoadSceneByName(GameManager.inst.firstLevelName);
    }

    public void EnableMenuElement(GameObject element, bool enableFullBackground = true)
    {
        canvas.SetActive(true);
        background.SetActive(enableFullBackground);
        escapeMenuBackground.SetActive(!enableFullBackground);


        mainMenu.SetActive(false);
        escapeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        loadingScreen.SetActive(false);
        keyRebindingMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameWonMenu.SetActive(false);
        playerDamageScreen.SetActive(false);
        playerGameUI.SetActive(false);

        element.SetActive(true);
    }

    public void QuitGame()
    {
        // Quit the game. This will only work in the built game, not in the editor
        Application.Quit();
    }

}
