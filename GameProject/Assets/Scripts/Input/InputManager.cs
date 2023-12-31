using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIQuickSlot m_uIQuickSlot;
    private PlayerInput m_playerInput;
    private PlayerInput.OnFootActions m_onFoot;
    private PlayerInput.MenuActions m_onMenu;
    private PlayerInput.InventoryActions m_inventoryActions;

    private PlayerMovement m_playerMovement;
    private PlayerLook m_playerLook;

    public PlayerInput.OnFootActions OnFoot => m_onFoot;
    public PlayerInput.InventoryActions Inventory => m_inventoryActions;
    private void Awake()
    {
        m_playerInput = new PlayerInput();
        m_onFoot = m_playerInput.OnFoot;
        m_onMenu = m_playerInput.Menu;
        m_inventoryActions = m_playerInput.Inventory;
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerLook = GetComponent<PlayerLook>();
    }
    private void Start()
    {
        m_uIQuickSlot = UIQuickSlot.instance;
    }
    private void FixedUpdate()
    {
        m_playerMovement.ProcessMove(m_onFoot.Movement.ReadValue<Vector2>());
        m_onFoot.Jump.performed += ctx => m_playerMovement.Jump();

        m_inventoryActions.LookInventory.performed += ctx => m_playerLook.ProcessLookInventory();
        m_inventoryActions.LookCraftingPanel.performed += ctx => m_playerLook.ProcessLookCraftingPanel();

        m_onMenu.Open.performed += ctx => m_playerLook.ProcessLookMenu();

        m_inventoryActions.QuickSlot1.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(0);
        m_inventoryActions.QuickSlot2.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(1);
        m_inventoryActions.QuickSlot3.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(2);
        m_inventoryActions.QuickSlot4.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(3);
        m_inventoryActions.QuickSlot5.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(4);
        m_inventoryActions.QuickSlot6.performed += ctx => m_uIQuickSlot.QuickSlotInputAction(5);
    }

    private void LateUpdate()
    {
        m_playerLook.ProcessLook(m_onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        m_onFoot.Enable();
        m_onMenu.Enable();
        m_inventoryActions.Enable();
    }

    private void OnDisable()
    {
        m_onFoot.Disable();
        m_onMenu.Disable();
        m_inventoryActions.Disable();
    }
}
