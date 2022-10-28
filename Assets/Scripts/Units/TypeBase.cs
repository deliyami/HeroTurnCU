using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Type", menuName = "Unit/Create new types")]
public class TypeBase : ScriptableObject
{
    [SerializeField] Sprite none;
    [SerializeField] Sprite normal;
    [SerializeField] Sprite fire;
    [SerializeField] Sprite water;
    [SerializeField] Sprite grass;
    [SerializeField] Sprite electric;
    [SerializeField] Sprite ice;
    [SerializeField] Sprite courage;
    [SerializeField] Sprite poison;
    [SerializeField] Sprite soil;
    [SerializeField] Sprite sky;
    [SerializeField] Sprite psycho;
    [SerializeField] Sprite wind;
    [SerializeField] Sprite stone;
    [SerializeField] Sprite ghost;
    [SerializeField] Sprite dragon;
    [SerializeField] Sprite devil;
    [SerializeField] Sprite steel;
    [SerializeField] Sprite strange;

    public Sprite None {
        get { return none; }
    }
    public Sprite Normal {
        get { return normal; }
    }
    public Sprite Fire {
        get { return fire; }
    }
    public Sprite Water {
        get { return water; }
    }
    public Sprite Grass {
        get { return grass; }
    }
    public Sprite Electric {
        get { return electric; }
    }
    public Sprite Ice {
        get { return ice; }
    }
    public Sprite Courage {
        get { return courage; }
    }
    public Sprite Poison {
        get { return poison; }
    }
    public Sprite Soil {
        get { return soil; }
    }
    public Sprite Sky {
        get { return sky; }
    }
    public Sprite Psycho {
        get { return psycho; }
    }
    public Sprite Wind {
        get { return wind; }
    }
    public Sprite Stone {
        get { return stone; }
    }
    public Sprite Ghost {
        get { return ghost; }
    }
    public Sprite Dragon {
        get { return dragon; }
    }
    public Sprite Devil {
        get { return devil; }
    }
    public Sprite Steel {
        get { return steel; }
    }
    public Sprite Strange {
        get { return strange; }
    }
}