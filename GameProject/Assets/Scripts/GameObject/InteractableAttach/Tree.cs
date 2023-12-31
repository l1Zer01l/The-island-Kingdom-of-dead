using System.Collections;
using TheIslandKOD;
using UnityEngine;

public class Tree : InteractableAttachRaycast
{
    private const string TAG_ANIMATION_DESTROY = "Destroy";
    [SerializeField] private float health;
    [SerializeField] private InventoryItemInfo m_info;
    [SerializeField] private InventoryItemInfo m_infoApple;
    [SerializeField] private AudioClip m_clipAttack;
    [SerializeField] private AudioClip m_clipDestroy;

    private AudioSource m_audioSource;
    private Animator m_animator;
    private UIQuickSlot m_quickSlot;
    private PlayerInventory m_playerInventory;
    private Coroutine m_coroutineDestroy;
    private bool m_isDestory = false;

    private void Start()
    {
        m_quickSlot = UIQuickSlot.instance;
        ReferenceSystem.OnFindedObjecs += OnInitPlayer;
        m_audioSource = GetComponent<AudioSource>();
        m_animator = GetComponent<Animator>();
    }

    private void OnInitPlayer()
    {
        ReferenceSystem.OnFindedObjecs -= OnInitPlayer;
        m_playerInventory = ReferenceSystem.instance.player.GetComponent<PlayerInventory>();
    }

    protected override void Interact()
    {
        if (m_quickSlot.ActiveSlot.itemType == typeof(StoneAxe) && !m_isDestory)
        {
            UpdateTree();
        }
    }

    protected override void OnEffects(Vector3 position, Quaternion rotation)
    {
        if (!m_isDestory)
        {
            m_audioSource.pitch = Random.Range(0.9f, 1.1f);
            m_audioSource.clip = m_clipAttack;
            m_audioSource.Play();       
            base.OnEffects(position, rotation);
        }
    }

    private void UpdateTree()
    {
        IInventoryItem item = new ItemWood(m_info);
        item.state.amount = 45;
        m_playerInventory.inventory.TryToAdd(this, item);

        item = new Apple(m_infoApple);
        item.state.amount = 1;
        m_playerInventory.inventory.TryToAdd(this, item);

        health -= Random.Range(1, 3);
        
        if (health <= 0)
        {
            m_audioSource.pitch =  1;
            m_audioSource.clip = m_clipDestroy;
            m_audioSource.Play();
            m_isDestory = true;       
            m_animator.SetTrigger(TAG_ANIMATION_DESTROY);            
            m_coroutineDestroy = CoroutineSystem.StartRoutine(CoroutineDestory());
        }
      
    }

    private IEnumerator CoroutineDestory()
    {

        while (true)
        {
            if (!m_audioSource.isPlaying)
            {
                CoroutineSystem.StopRoutine(m_coroutineDestroy);
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
