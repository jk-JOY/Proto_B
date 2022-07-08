using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SimpleInventory.Demo
{
    public class MenuViewer : ContentViewer
    {
        [Space(8f)]
        public InventoryManager inventory;
        public Text useText;
        public Text removeText;
        public Button removeButton;
        public Button sellButton;

        private InventorySlot slot;

        protected override void EventCall()
        {
            foreach (var handler in ItemHandler.HandlerList)
            {
                handler.OnSlotClick += OnDisplay;
            }
        }

        protected override void OnDisplay(PointerEventData eventData, InventorySlot slot)
        {

            if (slot.Item == null || eventData.button == PointerEventData.InputButton.Left ||
                eventData.button == PointerEventData.InputButton.Middle)
            {
                Cancel();
            }

            if (slot.Item != null && eventData.button == PointerEventData.InputButton.Right)
            {
                ViewerEnable(slot);
            }
        }

        protected override void OnDisappear(PointerEventData eventData, InventorySlot slot)
        {

        }

        protected override void DrawContent(InventorySlot slot)
        {
            this.slot = slot; //슬롯 등록
            SlotItem item = slot.Item; //아이템 가져오기

            //슬롯의 아이템 탭 이름에 따라 정렬 기준 및 텍스트 변경
            if (slot.Item.Tab.TabName == "ShopTab")
            {
                anchor = ViewerAnchor.BottomRight;
                removeText.text = "버리기";
                removeButton.interactable = false;
                sellButton.interactable = false;

            }
            else if (slot.Item.Tab.TabName == "QuickSlotTab")
            {
                anchor = ViewerAnchor.TopRight;
                removeText.text = "착용 해제";
                removeButton.interactable = true;
                sellButton.interactable = false;

            }
            else
            {
                anchor = ViewerAnchor.BottomRight;
                removeText.text = "버리기";
                removeButton.interactable = true;
                sellButton.interactable = true;
            }

            //상점 아이템일 경우
            if (slot.Item.Tab.TabName == "ShopTab")
            {
                useText.text = "구매";

            }
            else
            {
                //착용 장비에 따라 사용 텍스트 변경
                if (item is IEquipment)
                    useText.text = "착용";
                else
                    useText.text = "사용";
            }
        }

        public void Use()
        {
            if (slot != null)
            {
                slot.itemHandler.Use(slot.Item);
                SlotManager.RefreshAll();
                Cancel();
            }
        }

        public void Remove()
        {
            if (slot != null)
            {
                slot.Item.Tab.Remove(slot.Item);
                if (slot.Item.Tab.TabName == "QuickSlotTab")
                {
                    inventory.slotManager.LastRefreshedTab.Add(slot.Item, false);
                } //퀵슬롯일 경우 착용 해제 (제거 후 재추가)
                SlotManager.RefreshAll();
                Cancel();
            }
        }

        public void Sell()
        {
            if (slot != null)
            {
                ShopHelper.Sell(ref inventory.money, slot.Item);
                inventory.moneyText.text = inventory.money.ToString();
                SlotManager.RefreshAll();
                Cancel();
            }
        }

        public void Cancel()
        {
            ViewerDisable();
            slot = null;
        }
    }

}