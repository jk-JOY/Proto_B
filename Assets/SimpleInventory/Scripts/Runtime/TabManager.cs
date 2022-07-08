using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleInventory {

    public class TabManager : MonoBehaviour {

        [Serializable]
        public class TabProperty {
            public string tabName; //탭 이름
            public int capacity; //탭의 아이템 수용량
        }

        [Tooltip ("탭 리스트 (최소 하나 이상의 탭 필요)")]
        public List<TabProperty> tabList = null;

        private static List<string> tabNameList = new List<string> (); //탭 이름 리스트
        private static Dictionary<string, InventoryTab> TabDictionary { get; } = new Dictionary<string, InventoryTab> (); //탭 딕셔너리

        private void Awake () {

            //탭 이름 리스트, 딕셔너리에 탭 추가
            for (int i = 0; i < tabList.Count; i++) {
                tabNameList.Add (tabList[i].tabName);
                TabDictionary.Add (tabList[i].tabName, new InventoryTab (tabList[i].tabName, tabList[i].capacity));
            }
        }

        //탭 이름으로 탭 접근
        public static InventoryTab GetTab (string tabName) {
            if (tabNameList.Contains (tabName))
                return TabDictionary[tabName];
            else {
                Debug.LogError ("인벤토리 탭 : 정의되지 않은 인벤토리 탭입니다.");
                return null;
            }
        }

        //인덱스로 탭 접근
        public static InventoryTab GetTab (int index) {
            if ((index >= 0) && (index < tabNameList.Count))
                return TabDictionary[tabNameList[index]];
            else {
                Debug.LogError ("인벤토리 탭 : 인덱스 범위를 초과했습니다.");
                return null;
            }
        }
    }

    public class InventoryTab {

        public string TabName { get; } //탭 이름
        public int Capacity { get; private set; } //최대 수용량

        public List<SlotItem> ItemTable { get; private set; } //아이템 리스트

        //아이템 인덱스로 아이템 접근 및 설정 (인덱서)
        public SlotItem this[int itemIndex] {
            get { return ItemTable[itemIndex]; }
            set { ItemTable[itemIndex] = value; }
        }

        //아이템 ID로 아이템 접근 (인덱서)
        public SlotItem this[string id] {
            get {
                foreach (var item in ItemTable) {
                    if (item != null && item.Id == id) return item;
                }
                return null;
            }
        }

        //아이템 이름으로 아이템 반환
        public SlotItem[] GetItemsByName (string itemName) {
            List<SlotItem> items = new List<SlotItem> ();
            foreach (var item in ItemTable) {
                if (item != null && item.Name == itemName) items.Add (item);
            }
            return items.ToArray ();
        }

        //모든 아이템 반환 (저장 용도)
        public SlotItem[] GetItemsAll () {
            List<SlotItem> items = new List<SlotItem> ();
            foreach (var item in ItemTable) {
                if (item != null) items.Add (item);
            }
            return items.ToArray ();
        }

        public int Count { //현재 아이템 개수
            get {
                int count = 0;
                foreach (var item in ItemTable) {
                    if (item != null) count += 1;
                }
                return count;
            }
        }

        public bool IsEmpty { //비어있는지 여부 반환
            get { return Count == 0; }
        }

        public bool IsFull { //가득찼는지 여부 반환
            get { return Count == Capacity; }
        }

        public int GetNextIndex () { //순서대로 비어있는 슬롯 탐색
            int idx;
            for (idx = 0; idx < Capacity; idx++)
                if (ItemTable[idx] == null) break;
            return idx;
        }

        public InventoryTab (string tabName, int capacity) {
            TabName = tabName; //이름 설정
            Capacity = capacity; //최대 수용량 설정

            //아이템 테이블 초기화
            ItemTable = new List<SlotItem> (Capacity);
            for (int i = 0; i < Capacity; i++)
                ItemTable.Add (null);
        }

        //리스트에 아이템 추가 (자동 병합)
        public void Add (SlotItem item, bool autoMerge = true, Action addFailEvent = null) {

            //자동 병합
            if (autoMerge && item.MaxCount > 1) {

                foreach (var target in ItemTable) {

                    if (target == null || item == null || item.Name != target.Name) {
                        continue;
                    }

                    if (target.MaxCount > target.Count) {
                        int valid = target.MaxCount - target.Count;

                        if (item.Count <= valid) {
                            target.Count += item.Count;
                            item.Count = 0;

                        } else {
                            target.Count += valid;
                            item.Count -= valid;
                        }
                    }

                    if (item.Count <= 0) {
                        return;
                    }    
                }
            }

            //아이템 추가
            if (!IsFull) {
                item.Tab = this;
                int idx = GetNextIndex ();
                ItemTable[idx] = item;
                item.Index = idx;
            } else addFailEvent?.Invoke ();
        }

        //리스트에 아이템 추가 (인덱스 직접 설정, 저장소 불러오기 용도)
        public void Add (SlotItem item, int index) {
            item.Tab = this;
            ItemTable[index] = item;
            item.Index = index;
        }

        //리스트에서 아이템 제거
        public void Remove (SlotItem item) {
            if (ItemTable.Contains (item)) 
                ItemTable[item.Index] = null;
        }

        //리스트 확장
        public void Extend (int capacity) {
            ItemTable.Capacity += capacity;
            for (int i = 0; i < capacity; i++) {
                ItemTable.Add (null);
            }
            Capacity += capacity;
        }
    }
}
