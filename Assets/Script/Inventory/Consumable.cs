using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "Item/Consumable")]
public class Consumable : Item 
{
    [Header("Consumable")] //
    public float healthAdded;

    public override Item GetItem() { return this; }
    public override Tool GetTool() { return null; }
    public override Misc GetMisc() { return null; }
    public override Consumable GetConsumable() { return this; }
}
