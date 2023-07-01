using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace GDE.GenericSelectionUI
{
    public enum SelectionType { List, Grid }
    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        List<T> items;
        public int selectedItem = 0;
        SelectionType selectionType;
        int gridWidth = 2;

        float selectionTimer = 0;

        const float selectionSpeed = 5;

        public event Action<int> OnSelected;
        public event Action OnBack;
        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            this.items = items;

            items.ForEach(i => i.Init());
            UpdateSelectionInUI();
        }

        public virtual void ClearItems()
        {
            items.ForEach(i => i.Clear());
            this.items = null;
        }
        public virtual void HandleUpdate()
        {
            UpdateSelectionTimer();
            int prevSelection = selectedItem;
            if (selectionType == SelectionType.List)
                HandleListSelection();
            else if (selectionType == SelectionType.Grid)
                HandleGridSelection();
            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

            if (selectedItem != prevSelection) UpdateSelectionInUI();

            if (Input.GetButtonDown("Submit"))
                HandleSubmit();
            else if (Input.GetButtonDown("Cancel"))
                HandleCancel();
        }
        protected void HandleSubmit()
        {
            OnSelected?.Invoke(selectedItem);
        }
        protected void HandleCancel()
        {
            OnBack?.Invoke();
        }
        protected void HandleListSelection()
        {
            // TODO: 메뉴에는 이게 맞는데, 파티창같이 좌우로도 움직일 수 있는건 다른것으로 고쳐야 함
            float v = Input.GetAxis("Vertical");
            if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
            {
                selectedItem += -(int)Mathf.Sign(v);
                selectionTimer = 1 / selectionSpeed;
            }
        }

        void HandleGridSelection()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            if (selectionTimer == 0 && (Mathf.Abs(v) > 0.2f || Mathf.Abs(h) > 0.2f))
            {
                if (Mathf.Abs(h) > Mathf.Abs(v))
                    selectedItem += (int)Mathf.Sign(h);
                else
                    selectedItem += -(int)Mathf.Sign(v) * gridWidth;
                selectionTimer = 1 / selectionSpeed;
            }
        }
        public virtual void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChange(i == selectedItem);
            }
        }
        protected void UpdateSelectionTimer()
        {
            if (selectionTimer > 0)
                selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }
    }
}
