using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DartsGames.CUT
{
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Tooltip("Auto fetches from parent if null")]
        protected TabSystem tabSystem;
        [SerializeField, Tooltip("Auto fetches from gameObject if null")]
        protected Image image;
        [SerializeField, Tooltip("Same as sibling index if negative, uses serialized value otherwise")]
        protected int index = -1;

        public int Index => index;

        protected virtual void Awake()
        {
            // try auto assign dependency
            if (tabSystem == null)
            {
                tabSystem = GetComponentInParent<TabSystem>();

                if(tabSystem == null)
                {
                    Destroy(this);
                    return;
                }
            }

            if(image == null)
            {
                image = GetComponent<Image>();
            }

            if (index < 0)
                index = transform.GetSiblingIndex();

            tabSystem.SubscribeButton(this);
        }

        public virtual void OnActivate() { }

        public virtual void OnDeactivate() { }

        public void SetSprite(Sprite s)
        {
            if (image != null) image.sprite = s;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            tabSystem.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabSystem.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabSystem.OnTabExit(this);
        }
    }
}