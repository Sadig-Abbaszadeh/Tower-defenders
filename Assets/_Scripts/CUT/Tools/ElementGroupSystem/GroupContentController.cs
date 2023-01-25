using DartsGames.CUT.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace DartsGames.CUT
{
    /// <summary>
    /// Inherit from the class to create behaviour that will control a collection of elements children to it (UI based)
    /// Example: an inherited class can manage the cells of inventory by assigning or deleting data on cells when necessary, assigning on pointer behaviour and so on
    /// </summary>
    [ExtendEditor]
    public abstract class GroupContentController<El, TData> : MonoBehaviour where El : GroupContentElement<TData>
    {
        [SerializeField, Tooltip("How the elements are collected: from children components, instantiated on awake, instantiated later")]
        protected ElementInstantiation instantiationRegime = ElementInstantiation.GetFromChildrenOnAwake;
        [SerializeField, Tooltip("When setting data, destroy excess amount of elements")]
        protected bool destroyExcessElements = false;
        [SerializeField, Tooltip("Instantiate elements when there is not enough")]
        protected bool addElementsWhenNeeded = true;

        [SerializeField, InspectCondition(nameof(instantiationRegime), ElementInstantiation.GetFromChildrenOnAwake, true), Required("Element prefab is required", RequiredMessageType.Error)]
        protected El prefab;
        [SerializeField, InspectCondition(nameof(instantiationRegime), ElementInstantiation.InstantiateOnAwake)]
        private int instanceCount;

        [SerializeField, Tooltip("If not null, when an element with drag and drop controller is dropped on the area, OnDrop method will be called with the element's controller")]
        private DropArea<DragAndDropController> dropArea = null;

        protected List<El> elements = new List<El>();

        protected virtual void Awake()
        {
            if (instantiationRegime == ElementInstantiation.GetFromChildrenOnAwake)
            {
                elements = new List<El>(
                    GetComponentsInChildren<El>());

                for (int i = 0; i < elements.Count; i++)
                {
                    var el = elements[i];
                    el.SetIndex(i);
                    StartElement(el);
                }
            }
            else if (instantiationRegime == ElementInstantiation.InstantiateOnAwake)
            {
                InitiateElements(instanceCount);
            }

            if (dropArea != null)
                dropArea.OnItemDroppedOnThis += DroppedItemOnArea;
        }

        private void DroppedItemOnArea(DragAndDropController obj)
        {
            if (obj.TryGetComponent(out El element))
                OnElementDropped(element);
        }

        /// <summary>
        /// Instantiate elements under this gameobject
        /// </summary>
        public void InitiateElements(int countOfElements)
        {
            var index = elements.Count;

            for (int i = 0; i < countOfElements; i++)
            {
                var el = Instantiate(prefab, transform);

                StartElement(el);
                elements.Add(el);

                el.SetIndex(index);
                index++;
            }
        }

        public virtual void SetData(IList<TData> data)
        {
            // too much data
            if (data.Count >= elements.Count)
            {
                if (addElementsWhenNeeded)
                    InitiateElements(data.Count - elements.Count);
            }
            // too much elements
            else if (destroyExcessElements)
            {
                RemoveExcessElements(elements.Count - data.Count);
            }

            for (int i = 0; i < data.Count && i < elements.Count; i++)
            {
                elements[i].SetData(data[i]);
            }

            // if removed excess, this will not work
            for (int i = data.Count; i < elements.Count; i++)
            {
                elements[i].ResetData();
            }
        }

        public virtual void ResetData()
        {
            foreach (var el in elements)
                el.ResetData();
        }

        private void RemoveExcessElements(int destroyCount)
        {
            var z = elements.Count - 1;

            for (int i = 0; i < destroyCount; i++)
            {
                Destroy(elements[z].gameObject);
                elements.RemoveAt(z);
                z--;
            }
        }

        /// <summary>
        /// inherit to determine what happens to each element at awake or when instantiated with function
        /// </summary>
        protected virtual void StartElement(El element) => element.SetOnClick(el => OnElementClicked(element));

        /// <summary>
        /// Inherit to determine what happens when an element is drop into the drop area
        /// </summary>
        protected virtual void OnElementDropped(El element) { }

        protected virtual void OnElementClicked(El element) { }
    }

    public enum ElementInstantiation
    {
        GetFromChildrenOnAwake,
        InstantiateOnAwake,
        WaitForInstantiationWithMethod,
    }
}