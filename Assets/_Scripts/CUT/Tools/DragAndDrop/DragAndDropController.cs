using DartsGames;
using DartsGames.CUT.Attributes;
using DartsGames.CUT.UnityExtensions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DartsGames.CUT
{
    [ExtendEditor]
    public class DragAndDropController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField, Required("Graphic that is dragged is required", RequiredMessageType.Error)]
        protected Graphic dragGraphic = null;
        [SerializeField, Tooltip("If true, after drop, returns to original position. Else snaps to drop area position")]
        private bool returnToPositionOnDrop = false;
        [SerializeField, Tooltip("if true, snaps to the center of drop area, else is left at the position this was dropped")]
        private bool snapsToDropArea = true;
        [SerializeField, Tooltip("If true, the new position set when dropped will become the default position to return to when the subsequent drops fail")]
        private bool dropPositionIsNewDefaultPosition = true;

        private CanvasGroup _cg = null;
        private Canvas rootCanvas;

        private bool dropped = false;

        private Vector2 dragDefaultPosition;

        protected CanvasGroup CanvGroup
        {
            get
            {
                if (_cg == null)
                    _cg = dragGraphic.gameObject.AddComponent<CanvasGroup>();

                return _cg;
            }
        }

        protected virtual void Awake()
        {
            rootCanvas = dragGraphic.rectTransform.GetCompInAncestors<Canvas>();

            dragDefaultPosition = dragGraphic.rectTransform.anchoredPosition;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CanvGroup.blocksRaycasts = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            dragGraphic.rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CanvGroup.blocksRaycasts = true;

            // drop failed
            if (!dropped)
            {
                dragGraphic.rectTransform.anchoredPosition = dragDefaultPosition;
                OnDropFailed();
            }

            dropped = false;
        }

        public void DragComplete<T>(DropArea<T> area) where T : DragAndDropController
        {
            dropped = true;
            var rt = dragGraphic.rectTransform;

            if (returnToPositionOnDrop)
            {
                rt.anchoredPosition = dragDefaultPosition;
            }
            else if (snapsToDropArea)
            {
                rt.anchoredPosition = area.rt.anchoredPosition;
            }

            if (dropPositionIsNewDefaultPosition)
                dragDefaultPosition = rt.anchoredPosition;

            OnDropComplete();
        }

        protected virtual void OnDropComplete() { }
        protected virtual void OnDropFailed() { }
    }
}