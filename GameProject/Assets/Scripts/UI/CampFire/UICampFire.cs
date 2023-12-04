using System;
using TheIslandKOD;
using UnityEngine;
using UnityEngine.UI;

public class UICampFire : MonoBehaviour, IUIInventory
{
    
    public static UICampFire instance { get; private set; }

    [SerializeField] private UICampFireButton m_uIBbuttonFireOnOff;


    private UIInventorySlot[] m_uISlots;
    private InventoryWithSlots m_contentsCampFire;
    private PlayerLook m_playerLook;
    private PlayerMovement m_playerMovement;
    public InventoryWithSlots inventory => m_contentsCampFire;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;

        m_uISlots = GetComponentsInChildren<UIInventorySlot>();
    }

    private void Start()
    {
        m_playerLook = ReferenceSystem.instance.player.GetComponent<PlayerLook>();
        m_playerMovement = ReferenceSystem.instance.player.GetComponent<PlayerMovement>();
        gameObject.SetActive(false);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        m_playerLook.ProcessLookCampFire(visible);
        m_playerMovement.SetMove(!visible);
    }



    public void SetupContentCampFireUI(InventoryWithSlots contents, CampFire campFire)
    {
        m_uIBbuttonFireOnOff.SetaupButton(campFire);
        m_contentsCampFire = contents;
        m_contentsCampFire.OnInventoryStateChangedEvent += OnContentsCampFireStateChanged;
        var allSlots = contents.GetAllSlots();
        var allSlotsCount = allSlots.Length;
        for (int i = 0; i < allSlotsCount; i++)
        {
            var slot = allSlots[i];
            var uISlot = m_uISlots[i];
            uISlot.SetSlot(slot);
            uISlot.Refresh();
        }
    }

    public void UnSetupContentCampFireUI(CampFire campFire)
    {
        if (m_contentsCampFire != null)
        {
            m_contentsCampFire.OnInventoryStateChangedEvent -= OnContentsCampFireStateChanged;
            m_uIBbuttonFireOnOff.UnSetaupButton(campFire);
            m_contentsCampFire = null;       
        }
    }

    public void OnContentsCampFireStateChanged(object obj)
    {
        foreach (var uISlot in m_uISlots)
        {
            uISlot.Refresh();
        }
    }

   

}
