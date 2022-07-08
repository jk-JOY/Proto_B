using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInventory {

    /// <summary>
    ///  - 아이템 이동, 위치 교환, 병합 : 드래그 앤 드롭
    ///  - 아이템 사용 : 더블 클릭 (PC), 한번 탭 (모바일)
    /// </summary>

    [RequireComponent(typeof(SlotManager))]
    public class ItemHandler : MonoBehaviour {

        //아이템 핸들러 리스트
        public static List<ItemHandler> HandlerList { get; private set; } = new List<ItemHandler> ();

        public static SlotItem SelectedItem { get; internal set; } //선택된 아이템
        public static SlotItem TargetItem { get; internal set; } //타겟 아이템

        internal static bool RequestItemResetStop { get; set; }

        [Header ("Item Management Option")]
        [Tooltip ("아이템 이동 가능 여부")]
        public bool movable = true;

        [Tooltip ("다른 탭으로 이동 가능 여부")]
        public bool moveToOtherSlot = true; //이동이 가능할 때 사용 가능

        [Tooltip ("아이템간 위치 교환 가능 여부")]
        public bool switching = true; //이동이 가능할 때 사용 가능

        [Tooltip ("아이템 병합 가능 여부")]
        public bool merging = true; //이동이 가능할 때 사용 가능

        [Tooltip ("아이템 사용 가능 여부")]
        public bool usable = true;

        [Header ("Pointer Event And Exit Settings")]
        [Tooltip ("포인터 엔터 및 익스트 이벤트 활성화")]
        public bool enablePointerEnterAndExitEvent = true;

        [Tooltip ("포인터 엔터 갱신 대기 시간")]
        public float pointerUpdateInterval = 0.1f;

        public Action<InventorySlot, SlotItem> OnItemSelected { get; set; } //아이템 선택 이벤트 (선택 슬롯, 선택 아이템)
        public Action OnEventEnded { get; set; } //이벤트 종료 알림 이벤트

        public Action<SlotItem> OnItemMoved { get; set; } //아이템 이동 이벤트 (선택 아이템)
        public Action<SlotItem, SlotItem> OnItemSwitched { get; set; } //아이템 스위칭 이벤트 (선택 아이템, 대상 아이템)
        public Action<SlotItem> OnItemMerged { get; set; } //아이템 병합 이벤트 (대상 아이템)
        public Action<SlotItem> OnItemUsed { get; set; } //아이템 사용 이벤트 (선택 아이템)

        public Action<SlotItem> DragOutEvent { get; set; } //아이템을 인벤토리 밖으로 드래그했을 경우 이벤트
        public Action<SlotItem, InventorySlot> TypeNotMatchEvent { get; set; } //타입이 맞지 않을 경우 발생하는 이벤트
        public Action<SlotItem> SlotMoveFailEvent { get; set; } //슬롯간 이동 불능시 발생 이벤트

        public Action<PointerEventData, InventorySlot> OnSlotDown { get; set; } //포인터 다운 이벤트
        public Action<PointerEventData, InventorySlot> OnSlotUp { get; set; } //포인터 업 이벤트
        public Action<PointerEventData, InventorySlot> OnSlotClick { get; set; } //포인터 클릭 이벤트
        public Action<PointerEventData, InventorySlot> OnSlotEnter { get; set; } //포인터 엔터 이벤트
        public Action<PointerEventData, InventorySlot> OnSlotExit { get; set; } //포인터 익스트 이벤트

        private void Awake () {

            //핸들러 리스트에 현재 핸들러 추가
            if (!HandlerList.Contains (this)) HandlerList.Add (this);

            //아이템 이동 불가시 탭 이동, 스위칭, 머징 끄기
            if (!movable) {
                moveToOtherSlot = false;
                switching = false;
                merging = false;
            }
        }

        private void OnDestroy()
        {
            //핸들러 리스트에 현재 핸들러 제거
            if (HandlerList.Contains(this)) HandlerList.Remove(this);
        }

        //외부 업데이트 함수에서 호출 필요
        public static void RequestItemHandle () {
            //이벤트 종료 호출 (모바일)
#if (UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR)
            Touch touch = Input.GetTouch (0);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                CallEventEnd ();
                return;
            }
#endif

            //이벤트 종료 호출 (PC)
            if (Input.GetMouseButtonUp (0)) {
                CallEventEnd ();
            }
        }

        //선택, 대상 아이템 초기화 및 드래그 아웃 이벤트 호출
        internal static void CallEventEnd () {
            if (!RequestItemResetStop && SelectedItem != null) {
                InventorySlot slot = SlotManager.SelectedSlot;

                if (!EventSystem.current.IsPointerOverGameObject ()) //드래그 아웃 이벤트 호출
                    slot.itemHandler.DragOutEvent?.Invoke (slot.Item);

                slot.itemHandler.ResetItems (); //아이템 리셋
            }
        }

        //아이템 이동 (아이템 -> 빈 슬롯)
        public void Move (SlotItem selectedItem, InventorySlot targetSlot) {
            int originIdx = selectedItem.Index;
            targetSlot.slotManager.LastRefreshedTab.ItemTable[targetSlot.Index] = selectedItem;
            selectedItem.Tab[originIdx] = null;

            selectedItem.Index = targetSlot.Index;
            selectedItem.Tab = targetSlot.slotManager.LastRefreshedTab;

            OnItemMoved?.Invoke (selectedItem);
        }

        //아이템 위치 교환 (아이템 <-> 아이템)
        public void Switch (SlotItem selectedItem, SlotItem targetItem) {
            selectedItem.Tab[selectedItem.Index] = targetItem;
            targetItem.Tab[targetItem.Index] = selectedItem;

            int targetIdx = targetItem.Index;
            targetItem.Index = selectedItem.Index;
            selectedItem.Index = targetIdx;

            InventoryTab targetTab = targetItem.Tab;
            targetItem.Tab = selectedItem.Tab;
            selectedItem.Tab = targetTab;

            OnItemSwitched?.Invoke (selectedItem, targetItem);
        }

        //아이템 병합 (아이템 -> 아이템)
        public void Merge (SlotItem selectedItem, SlotItem targetItem) {
            if (targetItem.MaxCount > targetItem.Count) {
                int valid = targetItem.MaxCount - targetItem.Count;

                if (selectedItem.Count <= valid) {
                    targetItem.Count += selectedItem.Count;
                    selectedItem.Tab.Remove (selectedItem);

                } else {
                    targetItem.Count += valid;
                    selectedItem.Count -= valid;
                }

                OnItemMerged?.Invoke (targetItem);
            } else Switch (selectedItem, targetItem);
        }

        //아이템 사용
        public void Use (SlotItem usedItem) {
            //사용 이벤트가 있을 경우 실행
            if (usedItem is IUsable) {
                IUsable usableItem = usedItem as IUsable;

                if (usableItem.Usable) {
                    usableItem.UseEvent?.Invoke (usedItem); //아이템 사용
                    OnItemUsed?.Invoke (usedItem);
                }
            }

            //장비일 경우 추가 처리
            if (usedItem is IEquipment) {
                IEquipment equipment = usedItem as IEquipment;

                if (equipment.Usable) {

                    //현재 슬롯과 대상 슬롯이 다를 때 이벤트 실행
                    if (equipment.TargetSlot.Item != usedItem) {
                        if (equipment.TargetSlot.Item == null) 
                            Move (usedItem, equipment.TargetSlot);
                        else 
                            Switch (usedItem, equipment.TargetSlot.Item);

                        equipment.UseEvent?.Invoke (usedItem);
                        equipment.TargetSlot.slotManager.Refresh (usedItem.Tab);
                        OnItemUsed?.Invoke (usedItem);
                    }
                }

            //소비용품일 경우 추가 처리
            } else if (usedItem is IConsumable) {
                IConsumable consumableItem = usedItem as IConsumable;

                if (consumableItem.Usable) {
                    consumableItem.UseEvent?.Invoke (usedItem);
                    usedItem.Count -= 1;

                    if (usedItem.Count <= 0) 
                        usedItem.Tab.Remove (usedItem);

                    OnItemUsed?.Invoke (usedItem);
                }
            }
        }

        //선택 및 대상 아이템 초기화
        internal void ResetItems () {
            SelectedItem = null;
            TargetItem = null;
            OnEventEnded?.Invoke ();
        }
    }
}
