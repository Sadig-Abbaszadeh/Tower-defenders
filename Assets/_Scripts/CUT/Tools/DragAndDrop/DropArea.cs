using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DartsGames.CUT
{
    [RequireComponent(typeof(RectTransform))]
    public class DropArea<TDrop> : MonoBehaviour, IDropHandler where TDrop : DragAndDropController
    {
        public event Action<TDrop> OnItemDroppedOnThis;

        public RectTransform rt { get; private set; }

        protected virtual void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent(out TDrop dropObj))
            {
                dropObj.DragComplete(this);
                OnDropped(dropObj);
            }
        }

        protected virtual void OnDropped(TDrop dropObj) => OnItemDroppedOnThis?.Invoke(dropObj);
    }
}