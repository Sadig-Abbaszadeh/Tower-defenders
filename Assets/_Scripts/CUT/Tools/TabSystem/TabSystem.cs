using DartsGames.CUT.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DartsGames.CUT
{
    [ExtendEditor]
    public class TabSystem : MonoBehaviour
    {
        private List<TabButton> tabButtons = new List<TabButton>();

        [SerializeField]
        private Sprite tabNormal, tabHover, tabActive;
        [SerializeField, Space(8), Tooltip("If set to true, list of control objects are children of one master object, otherwise control objects should be serialized manually")]
        private bool useMasterObject = false;

        [SerializeField, InspectCondition(nameof(useMasterObject), true)]
        private GameObject masterObject = null;
        [SerializeField, InspectCondition(nameof(useMasterObject), false)]
        private List<GameObject> toggleObjects;

        private TabButton selected = null;

        private void Start()
        {
            if (useMasterObject && masterObject != null)
            {
                toggleObjects = new List<GameObject>();

                foreach (Transform t in masterObject.transform)
                {
                    toggleObjects.Add(t.gameObject);
                }
            }

            ResetTabs();
        }

        public void SubscribeButton(TabButton button)
        {
            tabButtons.Add(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();

            if (selected != button)
            {
                SetSprite(tabHover, button);
            }
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();


        }

        public void OnTabSelected(TabButton button)
        {
            if (selected != null)
                selected.OnDeactivate();

            selected = button;
            selected.OnActivate();

            ResetTabs();

            SetSprite(tabActive, button);

            SetIndexActive(button.Index);
        }

        private void SetIndexActive(int index)
        {
            for (int i = 0; i < toggleObjects.Count; i++)
                toggleObjects[i].SetActive(i == index);
        }

        private void SetSprite(Sprite s, TabButton b)
        {
            if (s != null) b.SetSprite(s);
        }

        public void ResetTabs()
        {
            if (tabNormal == null) return;

            foreach (var t in tabButtons)
            {
                if (t != selected)
                    t.SetSprite(tabNormal);
            }
        }
    }
}