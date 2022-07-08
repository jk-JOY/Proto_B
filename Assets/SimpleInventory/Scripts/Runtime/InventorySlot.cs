using UnityEngine;
using UnityEngine.UI;

namespace SimpleInventory {

    [RequireComponent (typeof (RectTransform))]
    public class InventorySlot : MonoBehaviour {

        public SlotManager slotManager = null; //슬롯 매니저
        public Image slotIcon = null; //슬롯 아이콘
        public Text itemCountText = null; //아이템 갯수 표시 텍스트
        public int allowType = -1; //허용되는 아이템 타입 (비트 플래그 사용, -1은 전체)

        public ItemHandler itemHandler = null; //아이템 핸들러

        public int Index { get; internal set; } //슬롯 인덱스
        public SlotItem Item { get; internal set; } //현재 슬롯 아이템

        private void Awake () {

            //슬롯 매니저 없을 경우 디버깅
            if (slotManager == null) {
                Debug.Log ("슬롯 매니저가 캐싱되지 않았습니다.");
                return;
            }

            //이미지 할당이 안 되어 있을 경우 이미지 가져오기
            if (slotIcon == null) {
                Image image;
                if ((image = GetComponent<Image> ()) != null)
                    slotIcon = image;
            }

            //탭 매니저에 슬롯 등록
            if (!slotManager.slotList.Contains (this))
                slotManager.slotList.Add (this);

            //아이템 핸들러 설정
            itemHandler = slotManager.itemHandler;
        }

        //슬롯 설정
        internal void SetSlot (SlotItem slotItem) {
            
            if (slotItem != null) {
                this.Item = slotItem;
                if (slotIcon != null) slotIcon.sprite = slotItem.Icon; //아이콘 설정
                if (itemCountText != null) { //갯수 텍스트 설정
                    if (slotItem.MaxCount == 1) itemCountText.text = "";
                    else itemCountText.text = slotItem.Count.ToString ();
                }

            } else {
                //슬롯 기본값으로 초기화
                this.Item = null;
                if (slotIcon != null) slotIcon.sprite = slotManager.defaultIcon;
                if (itemCountText != null) itemCountText.text = "";
            }
        }

        //해당 타입이 수용가능한지 여부 확인
        public bool HasType (int type) {
            return (allowType & type) == type;
        }
    }
}
