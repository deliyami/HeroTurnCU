using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] UIMode screenMode;

    // TODO 글자 색, UI 전부 global세팅에 맞춰서
    // 게임 내에서 옵션 창 누르면 UI 바꿀 수 있도록 해야 함
    [Header("발악기")]
    [SerializeField] MoveBase strugglePhysical;
    [SerializeField] MoveBase struggleSpecial;
    [Header("글자 색")]
    [SerializeField] Color highLightedColor;
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


    public UIMode ScreenMode => screenMode;
    public MoveBase StrugglePhysical => strugglePhysical;
    public MoveBase StruggleSpecial => struggleSpecial;
    public Color HighlightedColor => highLightedColor;
    public Color UnchosenColor => unchosenColor;
    public Color Transparent => new Color(1, 1, 1, 0);
    public static GlobalSettings i { get; private set; }
    public Dictionary<UnitID, List<Sprite>> UnitSprites { get; private set; }
    public Dictionary<UnitID, List<Sprite>> ReversedSprites { get; private set; }
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
    }
}
public enum UIMode { Light, Dark }