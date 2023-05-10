using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using static TMPro.TMP_Dropdown;
using System;

public enum SettingsItemType { Axis, Allies, }

public class SettingsItemData {
    public SettingsItemType Type;

    public string TeamName;
    public int TeamCount;

    public Sprite TeamEmblem;
    public int TeamEmblemSpriteIndex;

    public SettingsItemData() { }
    public SettingsItemData(
        SettingsItemType tY,
        string tN,
        int tC,
        Sprite tE,
        int tESI = 0) {

        Type = tY;
        TeamName = tN;
        TeamCount = tC;
        TeamEmblem = tE;
        TeamEmblemSpriteIndex = tESI;
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
    private int m_TeamNameIndex = -1;
    [SerializeField] private TMP_Dropdown m_TeamCountDropdown;
    private int m_TeamCount;

    private List<OptionData> m_TMP_TNDropdownData = new List<OptionData>();
    private List<Sprite> m_TMP_EmblemSprites = new List<Sprite>();

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

        // Temp Data Setup
        m_TeamNameDropdown.options.ForEach((OPTION) => {
            m_TMP_TNDropdownData.Add(OPTION);
        });

        m_EmblemSprites.ForEach((SPRITE) => {
            m_TMP_EmblemSprites.Add(SPRITE);
        });
        //
    }

    private void Sub()
    {
        m_RightArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex++;
            if(m_CurrentEmblemIndex > m_EmblemSprites.Count-1) { m_CurrentEmblemIndex = 0; }

            m_CurrentEmblem = m_EmblemSprites[m_CurrentEmblemIndex];
            m_EmblemImage.sprite = m_CurrentEmblem;
            m_EmblemImage.transform.DOLocalJump(new Vector3(0,20,0), 2, 1, .5f).OnComplete(() => {
                m_EmblemImage.transform.DOLocalJump(Vector3.zero, 2, 1, .5f);
            });

            if(m_EmblemQuestionMark.isActiveAndEnabled) { m_EmblemQuestionMark.enabled = false; }

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, 0, m_CurrentEmblem, m_CurrentEmblemIndex));           
        });

        m_LeftArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex--;
            if (m_CurrentEmblemIndex < 0) { m_CurrentEmblemIndex = m_EmblemSprites.Count - 1; }

            m_CurrentEmblem = m_EmblemSprites[m_CurrentEmblemIndex];
            m_EmblemImage.sprite = m_CurrentEmblem;

            m_EmblemImage.transform.DOLocalJump(new Vector3(0, 20, 0), 2, 1, .5f).OnComplete(() => {
                m_EmblemImage.transform.DOLocalJump(Vector3.zero, 2, 1, .5f);
            });

            if (m_EmblemQuestionMark.isActiveAndEnabled) { m_EmblemQuestionMark.enabled = false; }

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, 0, m_CurrentEmblem, m_CurrentEmblemIndex));
        });

        m_TeamNameDropdown.onValueChanged.AddListener((choice) => {
            m_TeamNameIndex = choice;
            m_TeamName = (string)m_TeamNameDropdown.options[choice].text;
            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, m_TeamName, 0, null));
        });

        m_TeamCountDropdown.onValueChanged.AddListener((choice) => {
            m_TeamCount = int.Parse(m_TeamCountDropdown.options[choice].text);

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, m_TeamCount, null));
        });
    }

    public void DistinctTeamEmblemsPlease(string tEmblemID)
    {
        m_EmblemSprites.Clear();

        for (int i = 0; i < m_TMP_EmblemSprites.Count; i++)
        {
            if(m_TMP_EmblemSprites[i].name != tEmblemID)
            {
                m_EmblemSprites.Add(m_TMP_EmblemSprites[i]);
            }
        }
        if(m_CurrentEmblem != null)
        {
            m_EmblemImage.sprite = m_CurrentEmblem;
            m_EmblemImage.transform.DOLocalJump(new Vector3(0, 20, 0), 2, 1, .5f).OnComplete(() => {
                m_EmblemImage.transform.DOLocalJump(Vector3.zero, 2, 1, .5f);
            });
        }
    }

    private bool isDistinctTeamNameValuesPleaseExecuting = false;

    public void DistinctTeamNameValuesPlease(string tNameString)
    {
        if (isDistinctTeamNameValuesPleaseExecuting)
            return;

        isDistinctTeamNameValuesPleaseExecuting = true;


        if (m_TeamNameIndex != -1)
        {
            // Store the selected value
            int selectedValue = m_TeamNameDropdown.value;
            string value = m_TeamNameDropdown.options[selectedValue].text;

            // Clear the dropdown options
            m_TeamNameDropdown.ClearOptions();

            // Add the options except the selected one
            List<OptionData> newOptions = new List<OptionData>();

            foreach (var option in m_TMP_TNDropdownData)
            {
                if (option.text != tNameString)
                {
                    newOptions.Add(option);
                }
            }

            // Update the dropdown options
            m_TeamNameDropdown.AddOptions(newOptions);

            // Restore the selected value
            int index = m_TeamNameDropdown.options.FindIndex((i) => { return i.text.Equals(value); });
            m_TeamNameDropdown.value = index;

            // Refresh the shown value
            m_TeamNameDropdown.RefreshShownValue();
        } 
        else
        {
            m_TeamNameDropdown.options.RemoveRange(1, m_TeamNameDropdown.options.Count - 1);

            // Add the options except the selected one
            foreach (var option in m_TMP_TNDropdownData)
            {
                if (option.text != tNameString)
                {
                    m_TeamNameDropdown.options.Add(option);
                }
            }
        }

        isDistinctTeamNameValuesPleaseExecuting = false;
    }

    private void DeSub()
    {
        m_RightArrow.onClick.RemoveAllListeners();
        m_LeftArrow.onClick.RemoveAllListeners();

        m_TeamNameDropdown.onValueChanged.RemoveAllListeners();
        m_TeamCountDropdown.onValueChanged.RemoveAllListeners();
    }
}
