using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DexDescription : MonoBehaviour
{
    UnitBase unit;
    public UnitBase Unit => unit;
    private void Awake()
    {
        unit = DexState.i.CurrentUnit;
    }
    public static List<string> DexDescriptionCategories { get; set; } = new List<string>(){
        "캐릭터 설명", "기술", "지닌 물건"
    };
    public MoveBase GetItem(int itemIndex)
    {
        // TODO: 순서가 맞지 않을수도 있으니 나중에 system.linq의 find써볼 것
        return unit.LearnableMoves[itemIndex].Base;
    }
    public static DexDescription GetDexDescription()
    {
        return FindObjectOfType<GameController>().GetComponent<DexDescription>();
    }
}
