using System;
using UnityEngine;

namespace SimpleInventory {

    public interface ISlotItem {

        InventoryTab Tab { get; set; } //아이템을 가진 탭 이름
        int Index { get; set; } //인벤토리 슬롯 순서
    }

    public interface IItemProperty {

        int MaxCount { get; set; } //아이템 최대 갯수
        int Count { get; set; } //아이템 현재 갯수

        string Id { get; set; } //사용자지정 아이디
        string Name { get; set; } //아이템 이름
        int Type { get; set; } //아이템 타입
        string Description { get; set; } //아이템 설명

        Sprite Icon { get; set; } //표시 아이콘

        void SetCount (int maxCount, int count); //최대 갯수 및 현재 갯수 설정 메서드
        void SetProperty (string id, string name, int type, string description); //아이템 속성 설정 함수
    }

    public interface ITradable {

        int BuyPrice { get; set; } //구매 가격
        int SellPrice { get; set; } //판매 가격

        void SetPrice (int buyPrice, int sellPrice); //구매 및 판매 가격 설정 메서드
    }

    public interface IUsable {
        bool Usable { get; set; }
        Action<SlotItem> UseEvent { get; set; }
    }

    public interface IEquipment : IUsable {
        InventorySlot TargetSlot { get; set; } //사용시 착용되는 대상 슬롯
    }

    public interface IConsumable : IUsable { }
}
