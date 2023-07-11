using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSecondAbility : AbilityBase
{
    public override bool isFocusSash()
    {
        if (isActivatableAbiility)
        {
            isActivatableAbiility = false;
            return true;
        }
        return false;
    }
}
