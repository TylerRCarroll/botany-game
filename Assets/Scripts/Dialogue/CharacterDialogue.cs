using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Lines", menuName = "Dialogue System/New Dialogue Lines", order = 1)]

public class CharacterDialogue : ScriptableObject
{   
    public List<NPC_Sentence> Sentences;
}