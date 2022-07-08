using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "new tool class", menuName = "Item/Misc")]
public class Misc : Item
{
      
    public override Item GetItem() { return this; }
    public override Tool GetTool() { return null; }
    public override Misc GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }


}
