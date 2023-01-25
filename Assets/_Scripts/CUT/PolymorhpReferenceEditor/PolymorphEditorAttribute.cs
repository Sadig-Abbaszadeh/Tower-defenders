using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolymorphEditorAttribute : PropertyAttribute
{
    public readonly Type[] BannedTypes = new Type[0];

    public PolymorphEditorAttribute(params Type[] banned)
    {
        BannedTypes = banned;
    }
}