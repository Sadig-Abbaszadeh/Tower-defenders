using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DartsGames.CUT
{
    /// <summary>
    /// Base class for element of a group content controller
    /// </summary>
    public abstract class GroupContentElement<T> : MonoBehaviour, IPointerClickHandler
    {
        public int Index { get; protected set; } = -1;
        public T CellData { get; protected set; }

        protected Action<GroupContentElement<T>> OnClick;

        /// <summary>
        /// Sets index of element (or cell)
        /// </summary>
        public virtual void SetIndex(int index) => this.Index = index;

        /// <summary>
        /// Sets cell data
        /// </summary>
        public virtual void SetData(T cellData)
        {
            this.CellData = cellData;
        }

        public void SetOnClick(Action<GroupContentElement<T>> OnClick)
        {
            this.OnClick += OnClick;
        }

        public abstract void ResetData();

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }
    }
}