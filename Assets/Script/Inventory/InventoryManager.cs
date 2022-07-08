using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item itemToAdd;
    public Item itemToRemove;
    public List<Item> items = new List<Item>();

    private void Start()
    {
        Add(itemToAdd);    
        Remove(itemToRemove);
    }

    //애드 차원에서는, 버튼을 눌러서 
    public void Add(Item item)
    {
        items.Add(item);
    }

    //리부므 차원에서는, 드래그 앤 드롭으로. 
    public void Remove(Item item)
    {
        items.Remove(item);
    }

}
