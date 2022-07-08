using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleInventory.Demo
{
    public class InventoryManager : MonoBehaviour
    {
        public SlotManager slotManager;
        public SlotManager quickSlotManager;
        public SlotManager shopSlotManager;

        public List<GameObject> tabList = new List<GameObject>(); //탭 리스트

        
        public List<GameObject> extraSlotList = new List<GameObject>();

        private Dictionary<GameObject, Image> tabImgDctn = new Dictionary<GameObject, Image>(); //탭 이미지 딕셔너리
        private Dictionary<GameObject, Button> tabBtnDctn = new Dictionary<GameObject, Button>(); //탭 버튼 딕셔너리

        public List<Sprite> spriteList = new List<Sprite>();

        public InventorySlot shieldSlot;
        public MenuViewer itemMenuViewer;

        public int money = 1000;
        public Text moneyText;

        private void Awake()
        {
            //탭 이미지 및 버튼 딕셔너리에 추가
            foreach (var tab in tabList)
            {
                tabImgDctn.Add(tab, tab.GetComponent<Image>());
                tabBtnDctn.Add(tab, tab.GetComponent<Button>());
            }
        }

        private void Start()
        {
            //CreateSwordTab();
            CreateShieldTab();
            //CreatePotionTab();
            //CreateShopTab();

            //슬롯 매니저 각 탭으로 새로고침 (초기 새로고침 필수)
            slotManager.Refresh(TabManager.GetTab("SwordTab"));
            quickSlotManager.Refresh(TabManager.GetTab("QuickSlotTab"));
            shopSlotManager.Refresh(TabManager.GetTab("ShopTab"));

            //아이템 핸들러 이벤트 설정
            foreach (var handler in ItemHandler.HandlerList)
            {
                handler.DragOutEvent = (item) => Debug.Log("Drag Out: " + item.Name);
                handler.SlotMoveFailEvent = (item) => Debug.Log("Slot Move Fail: " + item.Name);
                handler.TypeNotMatchEvent = (item, slot) => Debug.Log("Type doesn't match: " + item.Type);
            }

            //슬롯 동적 추가
            foreach (var slot in extraSlotList)
            {
                slot.SetActive(true);
            }

            //탭 확장
            TabManager.GetTab("SwordTab").Extend(5);
            TabManager.GetTab("ShieldTab").Extend(5);
            TabManager.GetTab("PotionTab").Extend(5);

            //슬롯 리스트 재설정 (탭 확장시 필수)
            slotManager.SlotSort();
        }

        private void Update()
        {
            ItemHandler.RequestItemHandle(); //이벤트 종료 감지 함수 호출
        }

        //private void CreateSwordTab()
        //{
        //    //아이템 생성 및 탭에 등록 (Item 객체)
        //    InventoryTab swordTab = TabManager.GetTab("SwordTab");

        //    Item longSword = new Item("", "Long Sword", (int)ItemType.Sword, "누군가의 한이 맺힌 겁나 긴 검.", spriteList[0]);
        //    longSword.UseEvent += (item) => Debug.Log("Item: " + item.Name);
        //    longSword.SetPrice(200, 200);

        //    Item dagger = new Item("", "Dagger", (int)ItemType.Sword, "단도, 투척용이다.", spriteList[1]);
        //    dagger.UseEvent += (item) => Debug.Log("Item: " + item.Name);
        //    dagger.SetPrice(150, 150);

        //    Item shuriken = new Item("", "Shuriken", (int)ItemType.Sword, "이건 던지는 용도가 아니다.", spriteList[2]);
        //    shuriken.UseEvent += (item) => Debug.Log("Item: " + item.Name);
        //    shuriken.SetPrice(125, 125);

        //    swordTab.Add(longSword);
        //    swordTab.Add(dagger);
        //    swordTab.Add(shuriken);
        //}

        private void CreateShieldTab()
        {
            //장비 생성 및 탭에 등록 (Equipment 객체)
            InventoryTab shieldTab = TabManager.GetTab("ShieldTab");

            Equipment normalShield = new Equipment("", "Normal Shield", (int)ItemType.Shield, "매우 평범한 쉴드", spriteList[3]);
            normalShield.UseEvent += (item) => Debug.Log("Equipment: " + item.Name);
            normalShield.TargetSlot = shieldSlot;
            normalShield.SetPrice(180, 180);

            Equipment magicShield = new Equipment("", "Magic Shield", (int)ItemType.Shield, "저주가 걸린 쉴드, 보호 받은 만큼 피해를 받는다.", spriteList[4]);
            magicShield.UseEvent += (item) => Debug.Log("Equipment: " + item.Name);
            magicShield.TargetSlot = shieldSlot;
            magicShield.SetPrice(220, 220);

            Equipment bigShield = new Equipment("", "Big Shield", (int)ItemType.Shield, "이건 방어구가 아니다.", spriteList[5]);
            bigShield.UseEvent += (item) => Debug.Log("Equipment: " + item.Name);
            bigShield.TargetSlot = shieldSlot;
            bigShield.SetPrice(280, 280);

            shieldTab.Add(normalShield, 1);
            shieldTab.Add(magicShield, 3);
            shieldTab.Add(bigShield, 4);
        }

        //private void CreatePotionTab()
        //{
        //    //포션 생성 및 탭에 등록 (Consumable 객체)
        //    InventoryTab potionTab = TabManager.GetTab("PotionTab");

        //    Consumable yellowPotion1 = new Consumable(99, 30, "", "Yellow Potion", (int)ItemType.Potion, "경험치 포션.", spriteList[6]);
        //    yellowPotion1.UseEvent += (item) => Debug.Log("Consumable: " + item.Name);
        //    yellowPotion1.SetPrice(35, 35);

        //    Consumable redPotion1 = new Consumable(99, 45, "", "Red Potion", (int)ItemType.Potion, "먹으면 죽는다.", spriteList[7]);
        //    redPotion1.UseEvent += (item) => Debug.Log("Consumable: " + item.Name);
        //    redPotion1.SetPrice(20, 20);

        //    Consumable bluewPotion = new Consumable(99, 30, "", "Blue Potion", (int)ItemType.Potion, "파워에이드.", spriteList[8]);
        //    bluewPotion.UseEvent += (item) => Debug.Log("Consumable: " + item.Name);
        //    bluewPotion.SetPrice(30, 30);

        //    Consumable yellowPotion2 = new Consumable(99, 30, "", "Yellow Potion", (int)ItemType.Potion, "경험치 포션.", spriteList[6]);
        //    yellowPotion2.UseEvent += (item) => Debug.Log("Consumable: " + item.Name);
        //    yellowPotion2.SetPrice(35, 35);

        //    Consumable redPotion2 = new Consumable(99, 85, "", "Red Potion", (int)ItemType.Potion, "먹으면 죽는다.", spriteList[7]);
        //    redPotion2.UseEvent += (item) => Debug.Log("Consumable: " + item.Name);
        //    redPotion2.SetPrice(20, 20);

        //    potionTab.Add(yellowPotion1, 3);
        //    potionTab.Add(redPotion1, 2);
        //    potionTab.Add(bluewPotion);
        //    potionTab.Add(yellowPotion2, 1);
        //    potionTab.Add(redPotion2);
        //}

        //private void CreateShopTab()
        //{
        //    //상점 아이템 생성 및 상점 탭에 등록
        //    //사용 이벤트에서 새로운 아이템을 생성할 때 마찬가지로 가격을 지정해주어야 합니다.

        //    InventoryTab shopTab = TabManager.GetTab("ShopTab");

        //    Item shop_longSword = new Item("", "Long Sword", (int)ItemType.Sword, "누군가의 한이 맺힌 겁나 긴 검.", spriteList[0]);
        //    shop_longSword.UseEvent += (item) => {
        //        Item buyItem = new Item("", "Long Sword", (int)ItemType.Sword, "누군가의 한이 맺힌 겁나 긴 검.", spriteList[0]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Item: " + slotItem.Name);
        //        buyItem.SetPrice(200, 200);
        //        BuyEvent(buyItem);
        //    };
        //    shop_longSword.SetPrice(200, 200);

        //    Item shop_dagger = new Item("", "Dagger", (int)ItemType.Sword, "단도, 투척용이다.", spriteList[1]);
        //    shop_dagger.UseEvent += (item) => {
        //        Item buyItem = new Item("", "Dagger", (int)ItemType.Sword, "단도, 투척용이다.", spriteList[1]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Item: " + slotItem.Name);
        //        buyItem.SetPrice(150, 150);
        //        BuyEvent(buyItem);
        //    };
        //    shop_dagger.SetPrice(150, 150);

        //    Item shop_shuriken = new Item("", "Shuriken", (int)ItemType.Sword, "이건 던지는 용도가 아니다.", spriteList[2]);
        //    shop_shuriken.UseEvent += (item) => {
        //        Item buyItem = new Item("", "Shuriken", (int)ItemType.Sword, "이건 던지는 용도가 아니다.", spriteList[2]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Item: " + slotItem.Name);
        //        buyItem.SetPrice(125, 125);
        //        BuyEvent(buyItem);
        //    };
        //    shop_shuriken.SetPrice(125, 125);

        //    Item shop_normalShield = new Item("", "Normal Shield", (int)ItemType.Shield, "매우 평범한 쉴드", spriteList[3]);
        //    shop_normalShield.UseEvent += (item) => {
        //        Equipment buyItem = new Equipment("", "Normal Shield", (int)ItemType.Shield, "매우 평범한 쉴드", spriteList[3]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Equipment: " + slotItem.Name);
        //        buyItem.TargetSlot = shieldSlot;
        //        buyItem.SetPrice(180, 180);
        //        BuyEvent(buyItem);
        //    };
        //    shop_normalShield.SetPrice(180, 180);

        //    Item shop_magicShield = new Item("", "Magic Shield", (int)ItemType.Shield, "저주가 걸린 쉴드, 보호 받은 만큼 피해를 받는다.", spriteList[4]);
        //    shop_magicShield.UseEvent += (item) => {
        //        Equipment buyItem = new Equipment("", "Magic Shield", (int)ItemType.Shield, "저주가 걸린 쉴드, 보호 받은 만큼 피해를 받는다.", spriteList[4]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Equipment: " + slotItem.Name);
        //        buyItem.TargetSlot = shieldSlot;
        //        buyItem.SetPrice(220, 220);
        //        BuyEvent(buyItem);
        //    };
        //    shop_magicShield.SetPrice(220, 220);

        //    Item shop_bigShield = new Item("", "Big Shield", (int)ItemType.Shield, "이건 방어구가 아니다.", spriteList[5]);
        //    shop_bigShield.UseEvent += (item) => {
        //        Equipment buyItem = new Equipment("", "Big Shield", (int)ItemType.Shield, "이건 방어구가 아니다.", spriteList[5]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Equipment: " + slotItem.Name);
        //        buyItem.TargetSlot = shieldSlot;
        //        buyItem.SetPrice(280, 280);
        //        BuyEvent(buyItem);
        //    };
        //    shop_bigShield.SetPrice(280, 280);

        //    Item shop_yellowPotion = new Item("", "Yellow Potion", (int)ItemType.Potion, "경험치 포션.", spriteList[6]);
        //    shop_yellowPotion.UseEvent += (item) => {
        //        Consumable buyItem = new Consumable(99, 1, "", "Yellow Potion", (int)ItemType.Potion, "경험치 포션.", spriteList[6]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Consumable: " + slotItem.Name);
        //        buyItem.SetPrice(35, 35);
        //        BuyEvent(buyItem);
        //    };
        //    shop_yellowPotion.SetPrice(35, 35);

        //    Item shop_redPotion = new Item("", "Re[d Potion", (int)ItemType.Potion, "먹으면 죽는다.", spriteList[7]);
        //    shop_redPotion.UseEvent += (item) => {
        //        Consumable buyItem = new Consumable(99, 1, "", "Red Potion", (int)ItemType.Potion, "먹으면 죽는다.", spriteList[7]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Consumable: " + slotItem.Name);
        //        buyItem.SetPrice(20, 20);
        //        BuyEvent(buyItem);
        //    };
        //    shop_redPotion.SetPrice(20, 20);

        //    Item shop_bluewPotion = new Item("", "Blue Potion", (int)ItemType.Potion, "파워에이드.", spriteList[8]);
        //    shop_bluewPotion.UseEvent += (item) => {
        //        Consumable buyItem = new Consumable(99, 1, "", "Blue Potion", (int)ItemType.Potion, "파워에이드.", spriteList[8]);
        //        buyItem.UseEvent += (slotItem) => Debug.Log("Consumable: " + slotItem.Name);
        //        buyItem.SetPrice(30, 30);
        //        BuyEvent(buyItem);
        //    };
        //    shop_bluewPotion.SetPrice(30, 30);

        //    shopTab.Add(shop_longSword);
        //    shopTab.Add(shop_dagger);
        //    shopTab.Add(shop_shuriken);
        //    shopTab.Add(shop_normalShield);
        //    shopTab.Add(shop_magicShield);
        //    shopTab.Add(shop_bigShield);
        //    shopTab.Add(shop_yellowPotion);
        //    shopTab.Add(shop_redPotion);
        //    shopTab.Add(shop_bluewPotion);
        //}

        //아이템 구매 이벤트
        private void BuyEvent(SlotItem buyItem)
        {
            InventoryTab tab = slotManager.LastRefreshedTab;
            ShopHelper.Buy(ref money, buyItem, tab, () => Debug.Log("돈 부족"));
            moneyText.text = money.ToString();
            SlotManager.RefreshAll();
        }

        //시각적 탭 변환
        public void TabConvert(GameObject tabItem)
        {
            foreach (var tab in tabList)
            {
                if (tabItem != tab.gameObject)
                {
                    tabBtnDctn[tab].interactable = true;
                    tabImgDctn[tab].color = new Color32(240, 240, 240, 255);
                }
                else
                {
                    tabBtnDctn[tab].interactable = false;
                    tabImgDctn[tab].color = new Color32(255, 255, 255, 255);
                }
            }
            slotManager.Refresh(TabManager.GetTab(tabItem.name));
        }

        //아이템이 선택되어있을 경우 클릭 없이 탭 변환
        public void TabConvertWithItem(GameObject tabItem)
        {
            if (ItemHandler.SelectedItem != null)
            {
                TabConvert(tabItem);
            }
        }
    }
}

