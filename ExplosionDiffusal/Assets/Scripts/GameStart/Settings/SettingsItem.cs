using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public enum SettingsItemType { Axis, Allies, }

public class SettingsItemData {
    public SettingsItemType Type;
    public string TeamName;
    public int TeamCount;
    public Sprite TeamEmblem;

    public SettingsItemData() { }
    public SettingsItemData(
        SettingsItemType tY,
        string tN,
        int tC,
        Sprite tE) {

        Type = tY;
        TeamName = tN;
        TeamCount = tC;
        TeamEmblem = tE;
    }
}


public class SettingsItem : MonoBehaviour
{
    public SettingsItemType Type;

    [HideInInspector] public UnityEvent<SettingsItemData> OnSettingsItemChanged = new UnityEvent<SettingsItemData>();

    [Header("Emblem Selector")]
    [SerializeField] private List<Sprite> m_EmblemSprites = new List<Sprite>();
    [SerializeField] private Image m_EmblemImage;
    [SerializeField] private Image m_EmblemQuestionMark;
    private Sprite m_CurrentEmblem;
    private int m_CurrentEmblemIndex = 0;
    [Space(5)]
    [SerializeField] private Button m_RightArrow;
    [SerializeField] private Button m_LeftArrow;

    [Header("Team Selector")]
    [SerializeField] private TMP_Dropdown m_TeamNameDropdown;
    private string m_TeamName;
    [SerializeField] private TMP_Dropdown m_TeamCountDropdown;
    private int m_TeamCount;

    private void Awake()
    {
        Sub();
    }

    private void Start()
    {
        InitItem();
    }

    private void OnDestroy()
    {
        DeSub();
    }

    public void InitItem()
    {
        m_CurrentEmblem = m_EmblemSprites[0];
        m_EmblemImage.sprite = m_CurrentEmblem;
    }

    private void Sub()
    {
        m_RightArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex++;
            if(m_CurrentEmblemIndex > m_EmblemSprites.Count-1) { m_CurrentEmblemIndex = 0; }

            m_CurrentEmblem = m_EmblemSprites[m_CurrentEmblemIndex];
            m_EmblemImage.sprite = m_CurrentEmblem;

            if(m_EmblemQuestionMark.isActiveAndEnabled) { m_EmblemQuestionMark.enabled = false; }

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, 0, m_CurrentEmblem));           
        });

        m_LeftArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex--;
            if (m_CurrentEmblemIndex < 0) { m_CurrentEmblemIndex = m_EmblemSprites.Count - 1; }

            m_CurrentEmblem = m_EmblemSprites[m_CurrentEmblemIndex];
            m_EmblemImage.sprite = m_CurrentEmblem;

            if (m_EmblemQuestionMark.isActiveAndEnabled) { m_EmblemQuestionMark.enabled = false; }

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, 0, m_CurrentEmblem));
        });

        m_TeamNameDropdown.onValueChanged.AddListener((choice) => {
            m_TeamName = (string)m_TeamNameDropdown.options[choice].text;

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, m_TeamName, 0, null));
        });

        m_TeamCountDropdown.onValueChanged.AddListener((choice) => {
            m_TeamCount = int.Parse(m_TeamCountDropdown.options[choice].text);

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, m_TeamCount, null));
        });
    }

    private void DeSub()
    {
        m_RightArrow.onClick.RemoveAllListeners();
        m_LeftArrow.onClick.RemoveAllListeners();

        m_TeamNameDropdown.onValueChanged.RemoveAllListeners();
        m_TeamCountDropdown.onValueChanged.RemoveAllListeners();
    }


}
