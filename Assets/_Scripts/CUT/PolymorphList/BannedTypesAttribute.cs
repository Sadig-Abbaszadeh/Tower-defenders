using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannedTypesAttribute : PropertyAttribute
{
    public readonly Type[] BannedTypes = new Type[0];

    public BannedTypesAttribute(params Type[] bannedTypes)
    {
        this.BannedTypes = bannedTypes;
    }
}