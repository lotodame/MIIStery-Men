using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleActionType { Move, SwitchPokemon, UseItem, Run }

public class BattleAction
{
    public BattleActionType Type { get; set; }
    public BattleUnit User { get; set; }
    public BattleUnit Target { get; set; }

    public Move SelectedMove { get; set; }          // For perfoming moves
    public Pokemon SelectedPokemon { get; set; }    // For switching pokemon
    public ItemBase SelectedItem { get; set; }      // For using items

    public bool IsInvalid { get; set; }

    public int Priority => (Type == BattleActionType.Move) ? SelectedMove.Base.Priority : 99;
}