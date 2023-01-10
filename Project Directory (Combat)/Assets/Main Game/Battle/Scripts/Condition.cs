using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<Character> OnStart { get; set; }

    public Func<Character, bool> OnBeforeMove { get; set; }
    public Action<Character> OnAfterTurn { get; set; }
}
