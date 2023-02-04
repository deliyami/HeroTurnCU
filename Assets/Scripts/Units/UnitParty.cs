using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitParty : MonoBehaviour
{
    [SerializeField] List<Unit> units;
    public event Action OnUpdated;

    public List<Unit> Units{
        get { return units; }
        set { 
            units = value; 
            OnUpdated?.Invoke();
        }
    }
    private void Awake()
    {
        foreach (var unit in units)
        {
            unit.Init();
        }
    }

    private void Start() {
        
    }

    public Unit GetHealtyhUnit(List<Unit> dontInclude = null)
    {
        var healthyUnits = units.Where(x => x.HP > 0);
        if (dontInclude != null)
            healthyUnits = healthyUnits.Where(u => !dontInclude.Contains(u));
        // TODO: 카를인 경우엔 적 타입보고 사용할 것
        return healthyUnits.FirstOrDefault();
    }

    public List<Unit> GetHealtyhUnits(int unitCount)
    {
        return units.Where(x => x.HP > 0).Take(unitCount).ToList();
    }


    public void AddUnit(Unit newUnit)
    {
        if (units.Count < 6)
        {
            units.Add(newUnit);
            OnUpdated?.Invoke();
        }
        else
        {
            // TODO: 외부로 보낼 것
        }
    }
    public bool CheckForEvolution()
    {
        return units.Any(u => u.CheckForEvolution() != null);
    }
    public IEnumerator RunEvolutions()
    {
        foreach (var unit in units)
        {
            var evolution = unit.CheckForEvolution();
            if (evolution != null)
            {
                yield return EvolutionManager.i.Evolve(unit, evolution);
            }
        }
    }
    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }
    
    public static UnitParty GetPlayerParty()
    {
        var unitParty = FindObjectOfType<PlayerController>().GetComponent<UnitParty>();
        // unitParty.Start();
        return unitParty;
    }
}
