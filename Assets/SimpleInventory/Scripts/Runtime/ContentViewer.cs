﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleInventory {

    /// <summary>
    /// 상속하여 이벤트 및 내용 처리하시면 됩니다.
    /// </summary>

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ContentViewer : MonoBehaviour {

        private static readonly Vector3[] RECT_WORLD_CORNERS = new Vector3[4];

        public enum ViewerAnchor { TopLeft, TopRight, BottomLeft, BottomRight }
        public enum ViewerStandard { Slot, Cursor }

        [Header("Viewer Setting")]
        public RectTransform viewer = null; //뷰어 (이동용)
        public CanvasGroup group = null; //캔버스 그룹 (페이드용)

        [Space (8f)]
        public ViewerAnchor anchor = ViewerAnchor.TopLeft; //뷰어 정렬
        public ViewerStandard standard = ViewerStandard.Slot; //뷰어 위치 기준

        [Space(8f)]
        public float displayDelay = 0.5f; //커서를 올리고 뷰어가 나타나는 시간
        public float disappearDelay = 0.2f; //포인터가 나갈 때 뷰어가 사라지는 시간
        public float fadeDuration = 0.1f; //캔버스 그룹이 페이드 되는 시간
        public bool blockRaycast = false;

        public bool IsEnabled { get; private set; } = false;

        private IEnumerator displayViewer = null;
        private IEnumerator disappearViewer = null;
        private WaitForSeconds waitForDisplay = null;
        private WaitForSeconds waitForDisappear = null;
        private Vector3 viewerPos = Vector3.zero;

        private void Awake () {
            if (viewer == null) viewer = GetComponent<RectTransform> ();
            if (group == null) group = GetComponent<CanvasGroup> ();
            if (!viewer.gameObject.activeSelf) viewer.gameObject.SetActive (true); //뷰어 활성화

            //캔버스 그룹 초기 설정
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;

            waitForDisplay = new WaitForSeconds(displayDelay);
            waitForDisappear = new WaitForSeconds(disappearDelay);
        }

        private void Start () {
            EventCall (); //이벤트 등록 함수 호출 (오버라이딩)
        }

        //뷰어 활성화
        public void ViewerEnable (InventorySlot slot) {
            IsEnabled = true;
            if (disappearViewer != null) StopCoroutine (disappearViewer);
            displayViewer = DisplayViewer (slot);
            StartCoroutine (displayViewer);
        }

        IEnumerator DisplayViewer (InventorySlot slot) {
            if (group.alpha <= 0) yield return waitForDisplay;

            if (slot.Item != null && IsEnabled) {

                //컨텐츠 내용 설정
                DrawContent (slot);

                //뷰어 위치 설정 (슬롯 기준)
                Rect viewerRect = GetWorldRect(viewer);
                Vector2 viewerSize = new Vector2(viewerRect.width, viewerRect.height); //뷰어 크기 가져오기

                if (standard == ViewerStandard.Slot) {

                    //슬롯의 위치, 크기 가져오기
                    RectTransform slot_rt = slot.GetComponent<RectTransform> ();
                    Rect slotRect = GetWorldRect(slot_rt);

                    viewerPos = slotRect.center; //슬롯 위치로 뷰어 위치 설정
                    viewerPos += GetFirstViewerPos(viewerSize.x * 0.5f + slotRect.width * 0.5f, viewerSize.y * 0.5f - slotRect.height * 0.5f);

                    //뷰어 위치 설정 (커서 기준)
                } else if (standard == ViewerStandard.Cursor) {
                    viewerPos = Input.mousePosition; //커서 위치로 뷰어 위치 설정
                    viewerPos += GetFirstViewerPos (viewerSize.x * 0.5f, viewerSize.y * 0.5f);
                }

                viewerPos.x += GetFixedViewerPosX (viewerPos, viewerSize);
                viewerPos.y += GetFixedViewerPosY (viewerPos, viewerSize);
                viewer.position = viewerPos; //뷰어 위치 적용

                //캔버스 그룹 페이드 인
                group.interactable = true;
                if (blockRaycast) group.blocksRaycasts = true;

                if (group.alpha < 1) {
                    float firstAlpha = group.alpha;
                    float alpha = firstAlpha;
                    float time = 0f;

                    while (alpha < 1f && IsEnabled) {
                        time += Time.deltaTime / fadeDuration;
                        alpha = Mathf.Lerp (firstAlpha, 1, time);
                        group.alpha = alpha;
                        yield return null;
                    }
                }
            }
        }

        //렉트 트랜스폼의 월드 좌표 Rect 가져오기
        private Rect GetWorldRect (RectTransform rectTransform) {
            rectTransform.GetWorldCorners(RECT_WORLD_CORNERS);
            Vector3 leftUp = RECT_WORLD_CORNERS[0];
            Vector3 rightDown = RECT_WORLD_CORNERS[2];

            return new Rect(leftUp.x, leftUp.y, rightDown.x - leftUp.x, rightDown.y - leftUp.y);
        }

        //정렬 기준에 따라 뷰어 초기 위치 설정
        private Vector3 GetFirstViewerPos (float posX, float posY) {
            switch (anchor) {
                case ViewerAnchor.TopLeft: return new Vector3 (-posX, posY);
                case ViewerAnchor.TopRight: return new Vector3 (posX, posY);
                case ViewerAnchor.BottomLeft: return new Vector3 (-posX, -posY);
                case ViewerAnchor.BottomRight: return new Vector3 (posX, -posY);
                default: return Vector3.zero;
            }
        }

        //정렬 기준 (Left, Right)에 따라 뷰어 위치 재설정
        private float GetFixedViewerPosX (Vector3 pos, Vector3 size) {
            switch (anchor) {
                case ViewerAnchor.TopLeft:
                case ViewerAnchor.BottomLeft:
                    float limit_Left = pos.x - size.x * 0.5f;
                    if (limit_Left < 0) return -limit_Left;
                    else return 0;

                case ViewerAnchor.TopRight:
                case ViewerAnchor.BottomRight:
                    float limit_Right = Screen.width - (pos.x + size.x * 0.5f);
                    if (limit_Right < 0) return limit_Right;
                    else return 0;

                default: return 0;
            }
        }

        //정렬 기준 (Top, Bottom)에 따라 뷰어 위치 재설정
        private float GetFixedViewerPosY (Vector3 pos, Vector3 size) {
            switch (anchor) {
                case ViewerAnchor.TopLeft:
                case ViewerAnchor.TopRight:
                    float limit_Top = Screen.height - (pos.y + size.y * 0.5f);
                    if (limit_Top < 0) return limit_Top;
                    else return 0;

                case ViewerAnchor.BottomLeft:
                case ViewerAnchor.BottomRight:
                    float limit_Bottom = pos.y - size.y * 0.5f;
                    if (limit_Bottom < 0) return -limit_Bottom;
                    else return 0;

                default: return 0;
            }
        }

        //뷰어 비활성화
        public void ViewerDisable () {
            IsEnabled = false;
            if (displayViewer != null) StopCoroutine (displayViewer);
            disappearViewer = DisappearViewer ();
            StartCoroutine (disappearViewer);
        }

        IEnumerator DisappearViewer () {
            if (group.alpha >= 1) yield return waitForDisappear;

            //뷰어 페이드 아웃
            if (!IsEnabled && group.alpha > 0) {
                float firstAlpha = group.alpha;
                float alpha = firstAlpha;
                float time = 0f;

                while (alpha > 0f && !IsEnabled) {
                    time += Time.deltaTime / fadeDuration;
                    alpha = Mathf.Lerp (firstAlpha, 0, time);
                    group.alpha = alpha;
                    yield return null;
                }

                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        //추상 메서드
        protected abstract void EventCall (); //이벤트 할당 메서드
        protected abstract void OnDisplay (PointerEventData eventData, InventorySlot slot); //활성화 이벤트
        protected abstract void OnDisappear (PointerEventData eventData, InventorySlot slot); //비활성화 이벤트
        protected abstract void DrawContent (InventorySlot slot); //뷰어 내용 그리기 메서드
    }
}