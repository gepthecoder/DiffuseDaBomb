using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using static TMPro.TMP_Dropdown;
using System.IO;
using System.Linq;

public enum SettingsItemType { Axis, Allies, }

public class SettingsItemData {
    public SettingsItemType Type;

    public string TeamName;
    public int TeamCount;

    public TeamIconImageMapper TeamEmblem;
    public int TeamEmblemSpriteIndex;

    public SettingsItemData() { }
    public SettingsItemData(
        SettingsItemType tY,
        string tN,
        int tC,
        TeamIconImageMapper tE,
        int tESI = 0) {

        Type = tY;
        TeamName = tN;
        TeamCount = tC;
        TeamEmblem = tE;
        TeamEmblemSpriteIndex = tESI;
    }
}

[System.Serializable]
public class TeamIconImageMapper
{
    public Sprite SPRITE;
    public Material MATERIAL;
    public bool hasMaterial;

    public TeamIconImageMapper() { }
}


public class SettingsItem : MonoBehaviour
{
    public SettingsItemType Type;

    [HideInInspector] public UnityEvent<SettingsItemData> OnSettingsItemChanged = new UnityEvent<SettingsItemData>();

    [Header("Emblem Selector")]
    [SerializeField] private List<Sprite> m_EmblemSprites = new List<Sprite>();
    [SerializeField] private TeamIcon m_EmblemImage;
    [SerializeField] private Image m_EmblemQuestionMark;
    private TeamIconImageMapper m_CurrentEmblem;
    private int m_CurrentEmblemIndex = 0;
    [Space(5)]
    [SerializeField] private Button m_RightArrow;
    [SerializeField] private Button m_LeftArrow;
    [Space(5)]
    [SerializeField] private Material m_MaterialTemplate;

    [Header("Team Selector")]
    [SerializeField] private TMP_Dropdown m_TeamNameDropdown;
    private string m_TeamName;
    private int m_TeamNameIndex = -1;
    [SerializeField] private TMP_Dropdown m_TeamCountDropdown;
    private int m_TeamCount;

    [SerializeField]
    private List<string> m_BuildInTeamNames = new List<string>()
    {
        "Navy SEALs", "MARCOS", "Spetsnaz", "SAS", "SASR", "Army Delta Force",
    };

    private List<OptionData> m_TMP_TNDropdownData = new List<OptionData>();
    public List<TeamIconImageMapper> m_TMP_EmblemSprites = new List<TeamIconImageMapper>();

    private string m_DirectoryPath_Emblems = "";
    private const string m_TeamNamesPrefsKey = "TeamNames";
    private const string m_DefaultTeamSpriteName = "soldier";

    public List<TeamIconImageMapper> m_ImageMapper = new List<TeamIconImageMapper>();

    private void Awake()
    {
        m_DirectoryPath_Emblems = Path.Combine(Application.persistentDataPath, "CustomEmblems");

        if (!Directory.Exists(m_DirectoryPath_Emblems))
        {
            Directory.CreateDirectory(m_DirectoryPath_Emblems);
        }

        InitItem();

        Sub();
    }

    private void OnDestroy()
    {
        DeSub();
    }

    public void InitItem()
    {
        RefreshTeamIcons();

        m_CurrentEmblem = m_ImageMapper[0];

        m_EmblemImage.SetTeamIconImageViaMapper(m_CurrentEmblem);

        // Temp Data Setup

        m_TeamNameDropdown.ClearOptions();
        List<string> allTeamNames = new List<string>();

        // Custom TN
        var customTeamNames = LoadTeamNames();

        for (int i = 0; i < customTeamNames.Count; i++)
        {
            allTeamNames.Add(customTeamNames[i]);
        }

        // Build-in TN
        for (int i = 0; i < m_BuildInTeamNames.Count; i++)
        {
            allTeamNames.Add(m_BuildInTeamNames[i]);
        }
        
        m_TeamNameDropdown.AddOptions(allTeamNames);

        m_TeamNameDropdown.options.ForEach((OPTION) => {
            
            m_TMP_TNDropdownData.Add(OPTION);
        });
        //

        m_ImageMapper.ForEach((mapper) => {
            m_TMP_EmblemSprites.Add(mapper);
        });
    }

    private void Sub()
    {
        m_RightArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex++;
            if(m_CurrentEmblemIndex > m_ImageMapper.Count-1) { m_CurrentEmblemIndex = 0; }

            m_CurrentEmblem = m_ImageMapper[m_CurrentEmblemIndex];
            m_EmblemImage.SetTeamIconImageViaMapper(m_CurrentEmblem);
            m_EmblemImage.transform.DOLocalJump(new Vector3(0,20,0), 2, 1, .5f).OnComplete(() => {
                m_EmblemImage.transform.DOLocalJump(Vector3.zero, 2, 1, .5f);
            });

            if(m_EmblemQuestionMark.isActiveAndEnabled) { m_EmblemQuestionMark.enabled = false; }

            OnSettingsItemChanged?.Invoke(new SettingsItemData(this.Type, string.Empty, 0, m_CurrentEmblem, m_CurrentEmblemIndex));           
        });

        m_LeftArrow.onClick.AddListener(() => {
            m_CurrentEmblemIndex--;
            if (m_CurrentEmblemIndex < 0) { m_CurrentEmblemIndex = m_ImageMapper.Count - 1; }

            m_CurrentEmblem = m_ImageMapper[m_CurrentEmblemIndex];
            m_EmblemImage.SetTeamIconImageViaMapper(m_CurrentEmblem);

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
        m_ImageMapper.Clear();

        for (int i = 0; i < m_TMP_EmblemSprites.Count; i++)
        {
            if(m_TMP_EmblemSprites[i].hasMaterial)
            {
                if(m_TMP_EmblemSprites[i].MATERIAL.mainTexture.name != tEmblemID)
                {
                    m_ImageMapper.Add(m_TMP_EmblemSprites[i]);
                }
            }else
            {
                if (m_TMP_EmblemSprites[i].SPRITE.name != tEmblemID)
                {
                    m_ImageMapper.Add(m_TMP_EmblemSprites[i]);
                }
            }
          
        }
        if(m_CurrentEmblem != null)
        {
            m_EmblemImage.SetTeamIconImageViaMapper(m_CurrentEmblem);
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
            for (int i = 0; i < m_TeamNameDropdown.options.Count; i++)
            {
                bool isDefaultOption = m_TeamNameDropdown.options[i].text == "SELECT TEAM";
                if (isDefaultOption)
                {
                    continue;
                }
                m_TeamNameDropdown.options.RemoveAt(i);
            }


            //m_TeamNameDropdown.options.RemoveRange(1, m_TeamNameDropdown.options.Count - 1);

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

    private void RefreshTeamIcons()
    {
        // add first custom icons
        string[] filePaths = Directory.GetFiles(m_DirectoryPath_Emblems, "*.png");

        List<byte[]> retrievedTextures = new List<byte[]>();

        // Read each file into a byte array and add it to the list
        foreach (string filePath in filePaths)
        {
            byte[] byteArray = File.ReadAllBytes(filePath);
            retrievedTextures.Add(byteArray);
        }

        // Create materials and apply textures
        foreach (byte[] byteArray in retrievedTextures)
        {
            Texture2D texture = new Texture2D(512, 512); // Create a new texture (modify the dimensions as needed).
            texture.LoadImage(byteArray); // Load the image data from the byte array.

            // Convert the Texture2D to a Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Create a new material based on the template
            Material newMaterial = new Material(m_MaterialTemplate);
            newMaterial.mainTexture = sprite.texture; // Set the texture for the material.

            // SET MATERIALS
            m_ImageMapper.Add(new TeamIconImageMapper { MATERIAL = newMaterial, hasMaterial = true  });
        }

        // DEFAULT SPRITES
        for (int i = 0; i < m_EmblemSprites.Count; i++)
        {
            if (m_EmblemSprites[i].name == m_DefaultTeamSpriteName)
                continue;

            m_ImageMapper.Add(new TeamIconImageMapper { SPRITE = m_EmblemSprites[i], hasMaterial = false });
        }

        // First Image Is SOLDIER Image with ?
        var defaultTeamSprite = m_EmblemSprites.FirstOrDefault(sprite => sprite.name == m_DefaultTeamSpriteName);
        m_ImageMapper.Insert(0, new TeamIconImageMapper { SPRITE = defaultTeamSprite, hasMaterial = false });
    }

    private List<string> LoadTeamNames()
    {
        List<string> tNames = new List<string>();

        string currentTeams = PlayerPrefs.GetString(m_TeamNamesPrefsKey, "");

        var teams = currentTeams.Split(',');

        for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i] == string.Empty)
            {
                continue;
            }

            tNames.Add(teams[i]);
        }

        tNames.Reverse();

        return tNames;
    }

    private void DeSub()
    {
        m_RightArrow.onClick.RemoveAllListeners();
        m_LeftArrow.onClick.RemoveAllListeners();

        m_TeamNameDropdown.onValueChanged.RemoveAllListeners();
        m_TeamCountDropdown.onValueChanged.RemoveAllListeners();
    }
}
