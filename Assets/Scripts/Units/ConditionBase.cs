using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "Unit/Create new conditions")]
public class ConditionBase : ScriptableObject
{
    // none, psn, brn, slp, par, frz, dpsn
    [SerializeField] Sprite none;
    [SerializeField] Sprite psn;
    [SerializeField] Sprite brn;
    [SerializeField] Sprite slp;
    [SerializeField] Sprite par;
    [SerializeField] Sprite frz;
    [SerializeField] Sprite dpsn;
    
    public Sprite None {
        get { return none; }
    }
    public Sprite Psn {
        get { return psn; }
    }
    public Sprite Brn {
        get { return brn; }
    }
    public Sprite Slp {
        get { return slp; }
    }
    public Sprite Par {
        get { return par; }
    }
    public Sprite Frz {
        get { return frz; }
    }
    public Sprite Dpsn {
        get { return dpsn; }
    }
}