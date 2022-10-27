using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type
{
    public TypeBase Base { get; set; }

    public Type(TypeBase uBase)
    {
        Base = uBase;
    }
}