using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Developer : MonoBehaviour
{
    public static Developer INSTANCE;

    [SerializeField] private bool m_DeveloperMode = false;

    [Header("DEV MENU")]
    [SerializeField] private List<DeveloperItem> m_DeveloperItems = new List<DeveloperItem>();
    [SerializeField] private Animator m_DeveloperAnimator;
    [Header("LOGGER")]
    [SerializeField] private BombLogger m_BombLogger;

    protected DeveloperItemType m_CurrentDeveloperItem = DeveloperItemType.TimeMenu;

    private void Awake()
    {
        m_BombLogger?.InitBombLogger(m_DeveloperMode);

        if (!m_DeveloperMode)
        {
            Destroy(this);
        }
        else
        {
            if(INSTANCE == null)
            {
                INSTANCE = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        StartCoroutine(DelayShowDevMenuButton(2f));
    }

    #region Interface

    public void OnDevButtonClicked() {
        m_DeveloperAnimator?.Play("SHOW");
        ShowDeveloperItemByType(0);
    }

    public void OnExitDevMenuButtonClicked() {
        m_DeveloperAnimator?.Play("HIDE");
    }

    public void OnTimeMenuButtonClicked() {
        ShowDeveloperItemByType(DeveloperItemType.TimeMenu);
    }
    public void OnLogMenuButtonClicked() {
        ShowDeveloperItemByType(DeveloperItemType.LogMenu);
    }
    public void OnTriggerMenuButtonClicked() {
        ShowDeveloperItemByType(DeveloperItemType.TriggerMenu);
    }

    #endregion

    #region Private Methods
    private void ShowDeveloperItemByType(DeveloperItemType type) {
        m_DeveloperItems.ForEach((item) => { 
            if(item.ID == type) {
                item.InitItem();
            } else { item.DeinitItem(); }
        });
    }

    private IEnumerator DelayShowDevMenuButton(float delay) {
        yield return new WaitForSeconds(delay);
        m_DeveloperAnimator?.Play("SHOWBTN");
    }
    #endregion

    #region Public Methods
    public bool IsDeveloperMode() { return m_DeveloperMode; }
    #endregion

}
