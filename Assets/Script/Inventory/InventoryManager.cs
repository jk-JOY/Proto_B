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

    //�ֵ� ����������, ��ư�� ������ 
    public void Add(Item item)
    {
        items.Add(item);
    }

    //���ι� ����������, �巡�� �� �������. 
    public void Remove(Item item)
    {
        items.Remove(item);
    }

}
