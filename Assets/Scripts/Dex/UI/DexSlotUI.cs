using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DexSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    RectTransform rectTransform;
    private void Awake()
    {
    }

    public TextMeshProUGUI NameText => nameText;
    public float Height => rectTransform.rect.height;
    public void SetData(UnitBase unit)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = unit.Name;
    }
}
