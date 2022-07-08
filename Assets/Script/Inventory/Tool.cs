using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName ="Item/Tool")]
public class Tool : Item
{
    [Header("Tool")]
    public ToolType toolType;

    public enum ToolType
    {
        Item,
        Weapon,
    }

    public override Item GetItem() { return this; }
    public override Tool GetTool() { return this; }
    public override Misc GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }
}
