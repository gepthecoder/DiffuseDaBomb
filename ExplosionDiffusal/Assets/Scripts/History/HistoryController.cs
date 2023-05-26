using UnityEngine;
using UnityEngine.UI;

public class HistoryController : MonoBehaviour
{
    [SerializeField] private HistoryItemObject m_HistoryObject;
    [SerializeField] private Transform m_Parent;
    [SerializeField] private Button m_HistoryButton;
    [SerializeField] private GameObject m_NotificationIcon;
    [SerializeField] private Button m_HistoryExitButton;
    [SerializeField] private Animator m_HistoryAnimator;

    private int m_Index = 1;
    private int m_LastHistoryCount = -1;
    private const string m_LastHistoryCountPrefKey = "LastHistoryCount";

    private void Awake()
    {
        if(PlayerPrefs.HasKey(m_LastHistoryCountPrefKey)) {
            // we had a previous session
            m_LastHistoryCount = PlayerPrefs.GetInt(m_LastHistoryCountPrefKey, 0);
        }
        else
        {
            // save
            PlayerPrefs.SetInt(m_LastHistoryCountPrefKey, m_LastHistoryCount);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        var items = SaveLoadManager.INSTANCE?.LoadHistoryObjects();

        if (items != null && items.Count > 0)
        {
            if(m_LastHistoryCount < items.Count) {
                m_LastHistoryCount = items.Count;
                // save
                PlayerPrefs.SetInt(m_LastHistoryCountPrefKey, m_LastHistoryCount);
                PlayerPrefs.Save();

                m_NotificationIcon.SetActive(true);
            } else
            {
                m_NotificationIcon.SetActive(false);
            }

            m_HistoryButton.interactable = true;

            items.ForEach((item) => {
                AddObjectToHistory(item);
            });

            m_HistoryButton.transform.parent.GetComponent<Animator>().enabled = true;
        }
        else
        {
            m_HistoryButton.interactable = false;
            m_HistoryButton.transform.parent.GetComponent<Animator>().enabled = false;
        }

        m_HistoryButton.onClick.AddListener(() => {
            m_HistoryButton.interactable = false;
            m_HistoryAnimator.SetTrigger("show");

            if (m_NotificationIcon.activeSelf) {
                m_NotificationIcon.SetActive(false);
            }
        });

        m_HistoryExitButton.onClick.AddListener(() => {
            m_HistoryButton.interactable = true;
            m_HistoryAnimator.SetTrigger("hide");
        });
    }

    private void OnDestroy()
    {
        m_HistoryButton.onClick.RemoveAllListeners();
        m_HistoryExitButton.onClick.RemoveAllListeners();
    }

    public void AddObjectToHistory(HistoryItemData data)
    {
        var historyObj = Instantiate(m_HistoryObject.gameObject, m_Parent).GetComponent<HistoryItemObject>();
        historyObj.SetHistoryItemObjectData(data, m_Index);
        m_Index++;
    }
}
