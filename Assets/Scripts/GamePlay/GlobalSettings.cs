using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour, ISavable
{
    [SerializeField] UIMode screenMode;

    // TODO 글자 색, UI 전부 global세팅에 맞춰서
    // 게임 내에서 옵션 창 누르면 UI 바꿀 수 있도록 해야 함
    [Header("발악기")]
    [SerializeField] MoveBase strugglePhysical;
    [SerializeField] MoveBase struggleSpecial;
    [Header("글자 색")]
    [SerializeField] Color highLightedColor;
    [SerializeField] Color greenLightedColor;
    [SerializeField] Color unchosenColor;
    [Header("스프라이트")] // TODO sprite 여기서 관리하고 유닛에게 들어간 sprite는 전부 여기서 사용하고, 유닛에게 ID박을것, 아마도 하단에 UNITID가 UNITBASE로 갈듯
    [SerializeField] List<Sprite> HeroSprite;
    [SerializeField] List<Sprite> HinamiSprite;
    [SerializeField] List<Sprite> KrabeSprite;
    [SerializeField] List<Sprite> GhostSprite;
    [SerializeField] List<Sprite> MashiroSprite;
    [SerializeField] List<Sprite> ItSprite;

    [SerializeField] List<Sprite> KarlSprite;
    [SerializeField] List<Sprite> LordSprite;
    [SerializeField] List<Sprite> SalamandarSprite;
    [SerializeField] List<Sprite> VioletSprite;
    [SerializeField] List<Sprite> ControllerSprite;
    [SerializeField] List<Sprite> TrekaSprite;
    [SerializeField] List<Sprite> OctaSprite;

    [SerializeField] List<Sprite> ReversedLord;
    [SerializeField] List<Sprite> ReversedViolet;
    [SerializeField] List<Sprite> ReversedOcta;
    [Header("닉네임")]
    [SerializeField] Sprite HeroNicknameSprite;
    [SerializeField] Sprite HinamiNicknameSprite;
    [SerializeField] Sprite KrabeNicknameSprite;
    [SerializeField] Sprite GhostNicknameSprite;
    [SerializeField] Sprite MashiroNicknameSprite;
    [SerializeField] Sprite ItNicknameSprite;
    [SerializeField] Sprite KarlNicknameSprite;
    [SerializeField] Sprite LordNicknameSprite;
    [SerializeField] Sprite SalamandarNicknameSprite;
    [SerializeField] Sprite VioletNicknameSprite;
    [SerializeField] Sprite ControllerNicknameSprite;
    [SerializeField] Sprite TrekaNicknameSprite;
    [SerializeField] Sprite OctaNicknameSprite;
    [SerializeField] float nicknameSize = 48;
    [Header("공격 스프라이트")]
    [SerializeField] List<Sprite> Attack1Sprite;
    [SerializeField] List<Sprite> Attack2Sprite;
    [SerializeField] List<Sprite> Attack3Sprite;
    [SerializeField] List<Sprite> Attack4Sprite;
    [SerializeField] List<Sprite> Attack5Sprite;
    [SerializeField] List<Sprite> Attack6Sprite;
    [SerializeField] List<Sprite> Attack7Sprite;
    [SerializeField] List<Sprite> Attack8Sprite;
    [SerializeField] List<Sprite> Attack9Sprite;
    [SerializeField] List<Sprite> Attack10Sprite;
    [SerializeField] List<Sprite> Attack11Sprite;
    [SerializeField] List<Sprite> Attack12Sprite;
    [SerializeField] List<Sprite> Attack13Sprite;
    [SerializeField] List<Sprite> Blow1Sprite;
    [SerializeField] List<Sprite> Blow2Sprite;
    [SerializeField] List<Sprite> Blow3Sprite;
    [SerializeField] List<Sprite> Darkness1Sprite;
    [SerializeField] List<Sprite> Darkness2Sprite;
    [SerializeField] List<Sprite> Darkness3Sprite;
    [SerializeField] List<Sprite> Death1Sprite;
    [SerializeField] List<Sprite> Earch1Sprite;
    [SerializeField] List<Sprite> Earch2Sprite;
    [SerializeField] List<Sprite> Earch3Sprite;
    [SerializeField] List<Sprite> Fire1Sprite;
    [SerializeField] List<Sprite> Fire2Sprite;
    [SerializeField] List<Sprite> Fire3Sprite;
    [SerializeField] List<Sprite> Fire4Sprite;
    [SerializeField] List<Sprite> Gun1Sprite;
    [SerializeField] List<Sprite> Gun2Sprite;
    [SerializeField] List<Sprite> Heal1Sprite;
    [SerializeField] List<Sprite> Heal2Sprite;
    [SerializeField] List<Sprite> Heal3Sprite;
    [SerializeField] List<Sprite> Heal4Sprite;
    [SerializeField] List<Sprite> Heal5Sprite;
    [SerializeField] List<Sprite> Heal6Sprite;
    [SerializeField] List<Sprite> Ice1Sprite;
    [SerializeField] List<Sprite> Ice2Sprite;
    [SerializeField] List<Sprite> Ice3Sprite;
    [SerializeField] List<Sprite> Ice4Sprite;
    [SerializeField] List<Sprite> Ice5Sprite;
    [SerializeField] List<Sprite> Light1Sprite;
    [SerializeField] List<Sprite> Light2Sprite;
    [SerializeField] List<Sprite> Light3Sprite;
    [SerializeField] List<Sprite> Light4Sprite;
    [SerializeField] List<Sprite> Light5Sprite;
    [SerializeField] List<Sprite> Light6Sprite;
    [SerializeField] List<Sprite> Light7Sprite;
    [SerializeField] List<Sprite> MeteorSprite;
    [SerializeField] List<Sprite> Spear1Sprite;
    [SerializeField] List<Sprite> Spear2Sprite;
    [SerializeField] List<Sprite> Spear3Sprite;
    [SerializeField] List<Sprite> Special1Sprite;
    [SerializeField] List<Sprite> Special2Sprite;
    [SerializeField] List<Sprite> Special3Sprite;
    [SerializeField] List<Sprite> Special4Sprite;
    [SerializeField] List<Sprite> Special5Sprite;
    [SerializeField] List<Sprite> Special6Sprite;
    [SerializeField] List<Sprite> Special7Sprite;
    [SerializeField] List<Sprite> Special8Sprite;
    [SerializeField] List<Sprite> Special9Sprite;
    [SerializeField] List<Sprite> Special10Sprite;
    [SerializeField] List<Sprite> Special11Sprite;
    [SerializeField] List<Sprite> Special12Sprite;
    [SerializeField] List<Sprite> Special13Sprite;
    [SerializeField] List<Sprite> Special14Sprite;
    [SerializeField] List<Sprite> Special15Sprite;
    [SerializeField] List<Sprite> Special16Sprite;
    [SerializeField] List<Sprite> Special17Sprite;
    [SerializeField] List<Sprite> State1Sprite;
    [SerializeField] List<Sprite> State2Sprite;
    [SerializeField] List<Sprite> State3Sprite;
    [SerializeField] List<Sprite> State4Sprite;
    [SerializeField] List<Sprite> State5Sprite;
    [SerializeField] List<Sprite> State6Sprite;
    [SerializeField] List<Sprite> Sword1Sprite;
    [SerializeField] List<Sprite> Sword2Sprite;
    [SerializeField] List<Sprite> Sword3Sprite;
    [SerializeField] List<Sprite> Sword4Sprite;
    [SerializeField] List<Sprite> Sword5Sprite;
    [SerializeField] List<Sprite> Sword6Sprite;
    [SerializeField] List<Sprite> Sword7Sprite;
    [SerializeField] List<Sprite> Sword8Sprite;
    [SerializeField] List<Sprite> Sword9Sprite;
    [SerializeField] List<Sprite> Sword10Sprite;
    [SerializeField] List<Sprite> Thunder1Sprite;
    [SerializeField] List<Sprite> Thunder2Sprite;
    [SerializeField] List<Sprite> Thunder3Sprite;
    [SerializeField] List<Sprite> Thunder4Sprite;
    [SerializeField] List<Sprite> Water1Sprite;
    [SerializeField] List<Sprite> Water2Sprite;
    [SerializeField] List<Sprite> Water3Sprite;
    [SerializeField] List<Sprite> Wind1Sprite;
    [SerializeField] List<Sprite> Wind2Sprite;
    [SerializeField] List<Sprite> Wind3Sprite;
    [SerializeField] List<Sprite> StonePieceSprite;



    public UIMode ScreenMode => screenMode;
    public MoveBase StrugglePhysical => strugglePhysical;
    public MoveBase StruggleSpecial => struggleSpecial;
    public Color HighlightedColor => highLightedColor;
    public Color GreenlightedColor => greenLightedColor;
    public Color UnchosenColor => unchosenColor;
    public Color Transparent => new Color(1, 1, 1, 0);
    public static GlobalSettings i { get; private set; }
    public Dictionary<UnitID, List<Sprite>> UnitSprites { get; private set; }
    public Dictionary<UnitID, List<Sprite>> ReversedSprites { get; private set; }
    public bool IsClear = false;
    public Dictionary<UnitID, bool> CheckNaming { get; private set; }
    public Dictionary<UnitID, Sprite> UnitNicknameSprites { get; private set; }
    public Dictionary<AttackSpriteID, List<Sprite>> AttackSprites { get; private set; }
    public float NicknameSize => nicknameSize;
    public int TakedTurn = 0;
    public Difficulty GameLevel = Difficulty.Normal;
    public List<ClearData> ClearDatas;
    private void Awake()
    {
        i = this;
        UnitSprites = new Dictionary<UnitID, List<Sprite>>()
        {
            {
                UnitID.None,
                new List<Sprite>()
            },
            {
                UnitID.Hero,
                HeroSprite
            },
            {
                UnitID.Hinami,
                HinamiSprite
            },
            {
                UnitID.Krabe,
                KrabeSprite
            },
            {
                UnitID.Ghost,
                GhostSprite
            },
            {
                UnitID.Mashiro,
                MashiroSprite
            },
            {
                UnitID.It,
                ItSprite
            },
            {
                UnitID.Karl,
                KarlSprite
            },
            {
                UnitID.Lord,
                LordSprite
            },
            {
                UnitID.Salamandar,
                SalamandarSprite
            },
            {
                UnitID.Violet,
                VioletSprite
            },
            {
                UnitID.Controller,
                ControllerSprite
            },
            {
                UnitID.Treka,
                TrekaSprite
            },
            {
                UnitID.Octa,
                OctaSprite
            },
        };
        ReversedSprites = new Dictionary<UnitID, List<Sprite>>()
        {
            {
                UnitID.Lord,
                ReversedLord
            },
            {
                UnitID.Violet,
                ReversedViolet
            },
            {
                UnitID.Octa,
                ReversedOcta
            },
        };
        Array array = Enum.GetValues(typeof(UnitID));
        CheckNaming = new Dictionary<UnitID, bool>();
        foreach (object value in array)
        {
            //object型のvalueからnumに変換
            UnitID id = (UnitID)value;

            // Console.WriteLine(id);
            CheckNaming.Add(id, false);
        }
        UnitNicknameSprites = new Dictionary<UnitID, Sprite>()
        {
            {
                UnitID.None,
                HeroNicknameSprite
            },
            {
                UnitID.Hero,
                HeroNicknameSprite
            },
            {
                UnitID.Hinami,
                HinamiNicknameSprite
            },
            {
                UnitID.Krabe,
                KrabeNicknameSprite
            },
            {
                UnitID.Ghost,
                GhostNicknameSprite
            },
            {
                UnitID.Mashiro,
                MashiroNicknameSprite
            },
            {
                UnitID.It,
                ItNicknameSprite
            },
            {
                UnitID.Karl,
                KarlNicknameSprite
            },
            {
                UnitID.Lord,
                LordNicknameSprite
            },
            {
                UnitID.Salamandar,
                SalamandarNicknameSprite
            },
            {
                UnitID.Violet,
                VioletNicknameSprite
            },
            {
                UnitID.Controller,
                ControllerNicknameSprite
            },
            {
                UnitID.Treka,
                TrekaNicknameSprite
            },
            {
                UnitID.Octa,
                OctaNicknameSprite
            },
        };
        AttackSprites = new Dictionary<AttackSpriteID, List<Sprite>>()
        {
            {
                AttackSpriteID.Attack1,
                Attack1Sprite
            },
            {
                AttackSpriteID.Attack2,
                Attack2Sprite
            },
            {
                AttackSpriteID.Attack3,
                Attack3Sprite
            },
            {
                AttackSpriteID.Attack4,
                Attack4Sprite
            },
            {
                AttackSpriteID.Attack5,
                Attack5Sprite
            },
            {
                AttackSpriteID.Attack6,
                Attack6Sprite
            },
            {
                AttackSpriteID.Attack7,
                Attack7Sprite
            },
            {
                AttackSpriteID.Attack8,
                Attack8Sprite
            },
            {
                AttackSpriteID.Attack9,
                Attack9Sprite
            },
            {
                AttackSpriteID.Attack10,
                Attack10Sprite
            },
            {
                AttackSpriteID.Attack11,
                Attack11Sprite
            },
            {
                AttackSpriteID.Attack12,
                Attack12Sprite
            },
            {
                AttackSpriteID.Blow1,
                Blow1Sprite
            },
            {
                AttackSpriteID.Blow2,
                Blow2Sprite
            },
            {
                AttackSpriteID.Blow3,
                Blow3Sprite
            },
            {
                AttackSpriteID.Darkness1,
                Darkness1Sprite
            },
            {
                AttackSpriteID.Darkness2,
                Darkness2Sprite
            },
            {
                AttackSpriteID.Darkness3,
                Darkness3Sprite
            },
            {
                AttackSpriteID.Death1,
                Death1Sprite
            },
            {
                AttackSpriteID.Earch1,
                Earch1Sprite
            },
            {
                AttackSpriteID.Earch2,
                Earch2Sprite
            },
            {
                AttackSpriteID.Earch3,
                Earch3Sprite
            },
            {
                AttackSpriteID.Fire1,
                Fire1Sprite
            },
            {
                AttackSpriteID.Fire2,
                Fire2Sprite
            },
            {
                AttackSpriteID.Fire3,
                Fire3Sprite
            },
            {
                AttackSpriteID.Fire4,
                Fire4Sprite
            },
            {
                AttackSpriteID.Gun1,
                Gun1Sprite
            },
            {
                AttackSpriteID.Gun2,
                Gun2Sprite
            },
            {
                AttackSpriteID.Heal1,
                Heal1Sprite
            },
            {
                AttackSpriteID.Heal2,
                Heal2Sprite
            },
            {
                AttackSpriteID.Heal3,
                Heal3Sprite
            },
            {
                AttackSpriteID.Heal4,
                Heal4Sprite
            },
            {
                AttackSpriteID.Heal5,
                Heal5Sprite
            },
            {
                AttackSpriteID.Heal6,
                Heal6Sprite
            },
            {
                AttackSpriteID.Ice1,
                Ice1Sprite
            },
            {
                AttackSpriteID.Ice2,
                Ice2Sprite
            },
            {
                AttackSpriteID.Ice3,
                Ice3Sprite
            },
            {
                AttackSpriteID.Ice4,
                Ice4Sprite
            },
            {
                AttackSpriteID.Ice5,
                Ice5Sprite
            },
            {
                AttackSpriteID.Light1,
                Light1Sprite
            },
            {
                AttackSpriteID.Light2,
                Light2Sprite
            },
            {
                AttackSpriteID.Light3,
                Light3Sprite
            },
            {
                AttackSpriteID.Light4,
                Light4Sprite
            },
            {
                AttackSpriteID.Light5,
                Light5Sprite
            },
            {
                AttackSpriteID.Light6,
                Light6Sprite
            },
            {
                AttackSpriteID.Light7,
                Light7Sprite
            },
            {
                AttackSpriteID.Meteor,
                MeteorSprite
            },
            {
                AttackSpriteID.Spear1,
                Spear1Sprite
            },
            {
                AttackSpriteID.Spear2,
                Spear2Sprite
            },
            {
                AttackSpriteID.Spear3,
                Spear3Sprite
            },
            {
                AttackSpriteID.Special1,
                Special1Sprite
            },
            {
                AttackSpriteID.Special2,
                Special2Sprite
            },
            {
                AttackSpriteID.Special3,
                Special3Sprite
            },
            {
                AttackSpriteID.Special4,
                Special4Sprite
            },
            {
                AttackSpriteID.Special5,
                Special5Sprite
            },
            {
                AttackSpriteID.Special6,
                Special6Sprite
            },
            {
                AttackSpriteID.Special7,
                Special7Sprite
            },
            {
                AttackSpriteID.Special8,
                Special8Sprite
            },
            {
                AttackSpriteID.Special9,
                Special9Sprite
            },
            {
                AttackSpriteID.Special10,
                Special10Sprite
            },
            {
                AttackSpriteID.Special11,
                Special11Sprite
            },
            {
                AttackSpriteID.Special12,
                Special12Sprite
            },
            {
                AttackSpriteID.Special13,
                Special13Sprite
            },
            {
                AttackSpriteID.Special14,
                Special14Sprite
            },
            {
                AttackSpriteID.Special15,
                Special15Sprite
            },
            {
                AttackSpriteID.Special16,
                Special16Sprite
            },
            {
                AttackSpriteID.Special17,
                Special17Sprite
            },
            {
                AttackSpriteID.State1,
                State1Sprite
            },
            {
                AttackSpriteID.State2,
                State2Sprite
            },
            {
                AttackSpriteID.State3,
                State3Sprite
            },
            {
                AttackSpriteID.State4,
                State4Sprite
            },
            {
                AttackSpriteID.State5,
                State5Sprite
            },
            {
                AttackSpriteID.State6,
                State6Sprite
            },
            {
                AttackSpriteID.Sword1,
                Sword1Sprite
            },
            {
                AttackSpriteID.Sword2,
                Sword2Sprite
            },
            {
                AttackSpriteID.Sword3,
                Sword3Sprite
            },
            {
                AttackSpriteID.Sword4,
                Sword4Sprite
            },
            {
                AttackSpriteID.Sword5,
                Sword5Sprite
            },
            {
                AttackSpriteID.Sword6,
                Sword6Sprite
            },
            {
                AttackSpriteID.Sword7,
                Sword7Sprite
            },
            {
                AttackSpriteID.Sword8,
                Sword8Sprite
            },
            {
                AttackSpriteID.Sword9,
                Sword9Sprite
            },
            {
                AttackSpriteID.Sword10,
                Sword10Sprite
            },
            {
                AttackSpriteID.Thunder1,
                Thunder1Sprite
            },
            {
                AttackSpriteID.Thunder2,
                Thunder2Sprite
            },
            {
                AttackSpriteID.Thunder3,
                Thunder3Sprite
            },
            {
                AttackSpriteID.Thunder4,
                Thunder4Sprite
            },
            {
                AttackSpriteID.Water1,
                Water1Sprite
            },
            {
                AttackSpriteID.Water2,
                Water2Sprite
            },
            {
                AttackSpriteID.Water3,
                Water3Sprite
            },
            {
                AttackSpriteID.Wind1,
                Wind1Sprite
            },
            {
                AttackSpriteID.Wind2,
                Wind2Sprite
            },
            {
                AttackSpriteID.Wind3,
                Wind3Sprite
            },
            {
                AttackSpriteID.StonePiece,
                StonePieceSprite
            },
        };
    }
    public object CaptureState()
    {
        var saveData = new ClearSaveData();
        saveData.IsClear = this.IsClear;
        saveData.ClearDatas = this.ClearDatas;

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as ClearSaveData;
        if (saveData != null)
        {
            IsClear = saveData.IsClear;
            ClearDatas = saveData.ClearDatas;
        }
    }
}
public enum UIMode { Light, Dark }
public enum Difficulty { Normal, Hell }
[System.Serializable]
public class ClearSaveData
{
    public bool IsClear;
    public List<ClearData> ClearDatas;
}

public class ClearData
{
    public int TakeTurn { get; private set; }
    public List<Unit> Party { get; private set; }
    public List<ItemSlot> UsedItems { get; private set; }
}
public enum AttackSpriteID
{
    Attack1, Attack2, Attack3, Attack4, Attack5, Attack6, Attack7, Attack8, Attack9, Attack10, Attack11, Attack12,
    Blow1, Blow2, Blow3,
    Darkness1, Darkness2, Darkness3,
    Death1,
    Earch1, Earch2, Earch3,
    Fire1, Fire2, Fire3, Fire4,
    Gun1, Gun2,
    Heal1, Heal2, Heal3, Heal4, Heal5, Heal6,
    Ice1, Ice2, Ice3, Ice4, Ice5,
    Light1, Light2, Light3, Light4, Light5, Light6, Light7,
    Meteor,
    Spear1, Spear2, Spear3,
    Special1, Special2, Special3, Special4, Special5, Special6, Special7, Special8, Special9, Special10, Special11, Special12, Special13, Special14, Special15, Special16, Special17,
    State1, State2, State3, State4, State5, State6,
    Sword1, Sword2, Sword3, Sword4, Sword5, Sword6, Sword7, Sword8, Sword9, Sword10,
    Thunder1, Thunder2, Thunder3, Thunder4,
    Water1, Water2, Water3,
    Wind1, Wind2, Wind3,
    StonePiece,
}