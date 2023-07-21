using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;
    public AudioSource contextualSource;

    private Transform _canvas;
    protected GameObject healthbar;
    protected BoxCollider _collider;
    public GameObject fov;
    public virtual Unit Unit { get; set; }
    public int ownerMaterialSlotIndex = 0;

    private bool _selected = false;
    public bool IsSelected { get => _selected; }
    private int _selectIndex = -1;
    public int SelectIndex { get => _selectIndex; }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
    }

    public void Initialize(Unit unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    private void Update()
    {
        if (Unit.Data.canBePromoted)
        {
            Unit.CurrentPromotionLevel = Unit.CurrentXP / Unit.Data.XPPromotionThreshold;

            int numPromotionsNeeded = Unit.CurrentPromotionLevel - Unit.TimesPromoted;
            if (numPromotionsNeeded > 0) Debug.Log($"{Unit.Code} will be promoted {numPromotionsNeeded} times!");
            for (int i = 0; i < numPromotionsNeeded; i++)
            {
                Unit.Promote();
                Unit.TimesPromoted += 1;
            }
        }
    }

    private void OnMouseDown()
    {
       Select(true, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    protected virtual bool IsActive()
    {
        return true;
    }

    public void Select() { Select(false, false); }
    public void Select(bool singleClick, bool holdingShift)
    {
        // basic case: using the selection box
        if (!singleClick)
        {
            _SelectUtil();
            return;
        }

        // building placement case
        if (Globals.CURRENT_PLACED_BUILDING != null)
        {
            return;
        }

        // single click: check for shift key
        if (!holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
                um.Deselect();
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
                _SelectUtil();
            else
                Deselect();
        }
    }

    public void Deselect()
    {
        Globals.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
        Destroy(healthbar);
        healthbar = null;

        EventManager.TriggerEvent("DeselectUnit", Unit);

        //FIXME stop following unit with camera
        //CameraController.instance.followTransform = null;
        
        _selected = false;
        _selectIndex = -1;
    }

    private void _SelectUtil()
    {
        // abort if not active
        if (!IsActive()) return;
        // abort if already selected
        if (Globals.SELECTED_UNITS.Contains(this)) return;

        Globals.SELECTED_UNITS.Add(this);
        EventManager.TriggerEvent("SelectUnit", Unit);
        selectionCircle.SetActive(true);

        //FIXME follow unit with camera
        //CameraController.instance.followTransform = transform;

        // play sound
        contextualSource.PlayOneShot(Unit.Data.onSelectSound);

        _selected = true;
        _selectIndex = Globals.SELECTED_UNITS.Count - 1;
        Debug.Log("SelectIndex is: " + _selectIndex);
    }

    public void SetOwnerMaterial(int owner)
    {
        Color playerColor = GameManager.instance.gamePlayersParameters.players[owner].color;
        Material[] materials = transform.GetComponent<Renderer>().materials;
        materials[ownerMaterialSlotIndex].color = playerColor;
        transform.GetComponent<Renderer>().materials = materials;
    }

    public void EnableFOV()
    {
        fov.SetActive(false); //FIXME
    }

    public void Attack(Transform target)
    {
        UnitManager um = target.GetComponent<UnitManager>();
        if (um == null) return;
        int targetHealth = um.TakeHit(Unit.AttackDamage);
        
        if (targetHealth <= 0) 
        {
            Unit.CurrentXP += um.Unit.Data.XPGivenOnDeath;
            if (Unit.CurrentXP > Unit.Data.maxXP)
            {
                Unit.CurrentXP = Unit.Data.maxXP;
            }

            // kill target
            um._Die();
        }
    }

    public int TakeHit(int attackPoints)
    {
        Unit.HP -= attackPoints;

        //update healthbar eventually
        Debug.Log($"{Unit.Code} health: {Unit.HP}");
        
        return Unit.HP;
    }

    private void _Die()
    {
        if (_selected)
            Deselect();            
        Destroy(gameObject);
    }
}