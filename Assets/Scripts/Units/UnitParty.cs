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
    private void Start()
    {
        foreach (var unit in units)
        {
            unit.Init();
        }
    }

    public Unit GetHealtyhUnit()
    {
        return units.Where(x => x.HP > 0).FirstOrDefault();
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
    
    public static UnitParty GetPlayerParty()
    {
        var unitParty = FindObjectOfType<PlayerController>().GetComponent<UnitParty>();
        // unitParty.Start();
        return unitParty;
    }
}
