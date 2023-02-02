using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<UnitEncounterRecord> wildUnits;
    [HideInInspector]
    [SerializeField] List<UnitEncounterRecord> findZeroChance;
    [HideInInspector]
    [SerializeField] int totalChance = 0;

    private void OnValidate()
    {
        totalChance = 0;
        foreach (var record in wildUnits)    
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }


    private void Start()
    {
        findZeroChance = wildUnits;
    }

    public Unit GetRandomWildUnit()
    {
        int randVal = Random.Range(0, totalChance + 1);
        var unitRecord = wildUnits.First(u => randVal >= u.chanceLower && randVal <= u.chanceUpper);
        var levelRange = unitRecord.LevelRange;
        int level = levelRange.y < levelRange.x?levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildUnit = new Unit(unitRecord.unit, level, unitRecord.Tribe, unitRecord.Effort, unitRecord.Personality);
        wildUnit.Init();
        Debug.Log($"유닛 생성중 {wildUnit.Base.Name}/{wildUnit.Level}");
        return wildUnit;
    }
}

[System.Serializable]
public class UnitEncounterRecord
{
    public UnitBase unit;
    // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    [SerializeField] int[] tribe;
    // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    [SerializeField] int[] effort;
    // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    [SerializeField] int[] personality;
    public Vector2Int LevelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
    public int[] Tribe => tribe;
    public int[] Effort => effort;
    public int[] Personality => personality;
}