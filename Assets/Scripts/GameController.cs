using System;
using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public enum GameState { FreeRoam, Busy, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution, Shop, Dex }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] DexUI dexUI;
    [SerializeField] DexDescriptionUI dexDescriptionUI;
    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;

    public StateMachine<GameController> StateMachine { get; private set; }
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        Instance = this;
        // menuController = GetComponentInParent<MenuController>();
        // menuController = transform.Find("UI Canvas").GetComponent<MenuController>();

        UnitDB.Init();
        MoveDB.Init();
        ConditionDB.Init();
        ItemDB.Init();
        QuestDB.Init();
        TypeDB.Init();
    }

    private void Start()
    {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        EvolutionManager.i.OnStartEvolution += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.i.OnCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = GameState.FreeRoam;

            AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };
        ShopController.i.OnStart += () =>
        {
            state = GameState.Shop;
        };
        ShopController.i.OnFinish += () => { state = GameState.FreeRoam; };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }
    public void StartCutsceneState()
    {
        Debug.Log("gamecontroller cutscene push");
        state = GameState.Cutscene;
        StateMachine.Push(CutsceneState.i);
        // cutscene.OnPlayerTriggered(PlayerController.i);
    }
    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }
    public void StartBattle(BattleTrigger trigger)
    {
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer, int unitCount = 1)
    {
        BattleState.i.trainer = trainer;
        BattleState.i.unitCount = unitCount;
        StateMachine.Push(BattleState.i);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<UnitParty>();
        // bool hasEvolutions = playerParty.CheckForEvolution();
        // if (hasEvolutions)
        //     StartCoroutine(playerParty.RunEvolutions());
        // else
        //     AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        AudioManager.i.PlayMusic(CurrentScene.SceneMusic, loop: false, fade: true);
    }
    private void Update()
    {
        StateMachine.Execute();
        if (state == GameState.Cutscene)
        {
            playerController.Character.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // 유닛
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            // 아이템
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            // 저장
            SavingSystem.i.Save("yonggi");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            // 로드
            SavingSystem.i.Load("yonggi");
            state = GameState.FreeRoam;
        }

    }
    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut = false)
    {
        yield return Fader.i.FadeIn(0.5f);
        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);
        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);
        else
            StartCoroutine(Fader.i.FadeOut(0.5f));
    }
    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 60;
        GUILayout.Label("STATE STACK", style);
        foreach (var stat in StateMachine.StateStack)
        {
            GUILayout.Label(stat.GetType().ToString(), style);
        }
    }
    public GameState State => state;
    public PlayerController PlayerController => playerController;
    public Camera WorldCamera => worldCamera;
}
