using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyScreenMove : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI pp;
    [SerializeField] TextMeshProUGUI type;
    Image image;
    private void Awake()
    {
        image = this.GetComponent<Image>();
    }
    // public TextMeshProUGUI Name => name;
    // public TextMeshProUGUI PP => pp;
    // public TextMeshProUGUI Type => type;
    // public Image Image => image;
    public void SetName(string name)
    {
        this.name.text = name;
    }
    public void SetPP(string name)
    {
        this.pp.text = name;
    }
    public void SetType(UnitType unitType)
    {
        this.type.text = TypeDB.GetObjectByName(unitType.ToString()).Name;
    }
    public void SetSprite(UnitType unitType)
    {
        this.image.sprite = TypeDB.GetObjectByName(unitType.ToString()).Sprite;
    }
}
