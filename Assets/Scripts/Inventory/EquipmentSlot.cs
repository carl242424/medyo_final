using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// EquipmentSlot communicates with InventoryManager and EquippedSlot
public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    private InventoryManager inventoryManager;
    private EquipmentSOLibrary equipmentSOLibrary;

    public GameObject selectedShader;
    public bool thisItemSelected;

    // ITEM DATA
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    public ItemType itemType;

    // ITEM SLOT
    [SerializeField] private Image itemImage;

    // ITEM DESCRIPTION SLOT
    public Image ItemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    public GameObject equipmentDescriptionPanel;

    // EQUIPPED SLOTS
    [SerializeField] private EquippedSlot helmetSlot, armorSlot, accessorySlot, subWeaponSlot, weaponSlot, artifactFireSlot, artifactWindSlot, artifactElectricSlot;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        equipmentSOLibrary = GameObject.Find("InventoryCanvas").GetComponent<EquipmentSOLibrary>();
        
        FindEquippedSlots();
    }

    private void OnEnable()
    {
        // Re-find slots when menu opens (EquipmentMenu starts inactive)
        if (inventoryManager != null)
            FindEquippedSlots();
    }

    private void FindEquippedSlots()
    {
        // Include inactive - EquipmentMenu starts disabled, so slots are inactive at Start
        EquippedSlot[] allSlots = GameObject.Find("InventoryCanvas").GetComponentsInChildren<EquippedSlot>(true);
        
        foreach (EquippedSlot slot in allSlots)
        {
            string slotName = slot.gameObject.name.ToLower();
            
            if (slotName.Contains("helmet"))
                helmetSlot = slot;
            else if (slotName.Contains("armor"))
                armorSlot = slot;
            else if (slotName.Contains("accessory"))
                accessorySlot = slot;
            else if (slotName.Contains("subweapon") || slotName.Contains("sub"))
                subWeaponSlot = slot;
            else if (slotName.Contains("weapon") && !slotName.Contains("sub"))
                weaponSlot = slot;
            else if (slotName.Contains("fire"))
                artifactFireSlot = slot;
            else if (slotName.Contains("wind"))
                artifactWindSlot = slot;
            else if (slotName.Contains("electric"))
                artifactElectricSlot = slot;
        }
        
#if UNITY_EDITOR
        if (helmetSlot == null || armorSlot == null || weaponSlot == null)
            Debug.LogWarning("EquipmentSlot: Some equipped slots were not found. Check InventoryCanvas hierarchy and slot names.");
#endif
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType)
    {
        // Check if slot already full
        if (isFull)
        {
            return quantity;
        }

        // Update Item Type
        this.itemType = itemType;

        // Update Name, Image & Description
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        this.itemDescription = itemDescription;

        // Update Quantity
        this.quantity = 1;
        isFull = true;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (isFull)
        {
            if (thisItemSelected)
            {
                // Second click (or first if already selected): equip immediately
                equipmentDescriptionPanel?.SetActive(false);
                inventoryManager.DeselectAllSlots();
                var statManager = GameObject.Find("StatManager");
                if (statManager != null)
                    statManager.GetComponent<PlayerStats>().TurnOffPreviewStats();
                EquipGear();
            }
            else
            {
                // First click: select item and show description
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
                for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
                {
                    if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                    {
                        equipmentSOLibrary.equipmentSO[i].PreviewEquipment();
                        break;
                    }
                }
                if (equipmentDescriptionPanel != null)
                    equipmentDescriptionPanel.SetActive(true);
            }
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            ItemDescriptionImage.sprite = itemSprite;
            if (ItemDescriptionImage == null)
                ItemDescriptionImage.sprite = emptySprite;
        }
        else
        {
            GameObject.Find("StatManager")?.GetComponent<PlayerStats>()?.TurnOffPreviewStats();
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            equipmentDescriptionPanel?.SetActive(false);
        }
    }

    private void EquipGear()
    {
        equipmentDescriptionPanel?.SetActive(false);
        bool equipped = false;
        
        switch (itemType)
        {
            case ItemType.helmet:
                if (helmetSlot != null) { helmetSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.armor:
                if (armorSlot != null) { armorSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.accessory:
                if (accessorySlot != null) { accessorySlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.subWeapon:
                if (subWeaponSlot != null) { subWeaponSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.weapon:
                if (weaponSlot != null) { weaponSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.artifactFire:
                if (artifactFireSlot != null) { artifactFireSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.artifactWind:
                if (artifactWindSlot != null) { artifactWindSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
            case ItemType.artifactElectric:
                if (artifactElectricSlot != null) { artifactElectricSlot.EquipGear(itemSprite, itemName, itemDescription); equipped = true; }
                break;
        }
        
        if (equipped)
            EmptySlot();
        else
            Debug.LogError($"EquipmentSlot: Could not equip {itemName} - target slot for {itemType} is null. Check FindEquippedSlots.");
    }

    private void EmptySlot()
    {
        itemImage.sprite = emptySprite;
        isFull = false;
        this.quantity = 0;

        itemSprite = emptySprite;
        itemImage.sprite = emptySprite;
        itemDescription = "";
        itemName = "";

        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        ItemDescriptionImage.sprite = emptySprite;
        equipmentDescriptionPanel.SetActive(false);
    }

    public void OnRightClick()
    {
        if (!isFull) return;
        
        // Right-click on equipment = equip immediately (easier than double-click)
        if (itemType != ItemType.consumable && itemType != ItemType.collectible && itemType != ItemType.none)
        {
            equipmentDescriptionPanel?.SetActive(false);
            inventoryManager.DeselectAllSlots();
            GameObject.Find("StatManager")?.GetComponent<PlayerStats>()?.TurnOffPreviewStats();
            EquipGear();
            return;
        }
        
        // Right-click on consumable/collectible = drop
        GameObject itemToDrop = new GameObject(itemName);
        
        // Add Rigidbody2D FIRST so ItemDrop can find it
        Rigidbody2D rb = itemToDrop.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        
        // Add collider
        BoxCollider2D collider = itemToDrop.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        // Add ItemDrop (uses Rigidbody2D in Start)
        itemToDrop.AddComponent<ItemDrop>();
        
        // Create Sprite Renderer with sprite
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        if (itemSprite != null)
        {
            sr.sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning($"Item {itemName} has no sprite!");
            sr.sprite = emptySprite;
        }
        sr.sortingOrder = 5;
        sr.sortingLayerName = "Ground";
        
        // Add Item component with data
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;
        newItem.itemType = this.itemType;

        // Set position and scale
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(1f, 0, 0);
        itemToDrop.transform.localScale = new Vector3(1f, 1f, 1f);

        // Remove the item from inventory
        this.quantity -= 1;
        if (this.quantity <= 0)
        {
            EmptySlot();
            equipmentDescriptionPanel.SetActive(false);
        }
    }

}
