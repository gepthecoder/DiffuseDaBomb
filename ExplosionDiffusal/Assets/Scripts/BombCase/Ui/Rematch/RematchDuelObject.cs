using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class RematchDuelObject : MonoBehaviour
{
    [Header("Team Avatar")]
    [SerializeField] private TeamIcon m_TeamAvatarObject;
    [SerializeField] private Transform m_TeamAvatarContentParent;
    [SerializeField] private Material m_MaterialTemplate;
    [SerializeField] private List<Sprite> m_BuildInEmblems = new List<Sprite>();
    [SerializeField] private SnapToItem m_TeamAvatarSnap;

    [Header("Team Name")]
    [SerializeField] private TeamName m_TeamNameObject;
    [SerializeField] private Transform m_TeamNameContentParent;
    [SerializeField]
    private List<string> m_BuildInTeamNames = new List<string>()
    {
        "Navy SEALs", "MARCOS", "Spetsnaz", "SAS", "SASR", "Army Delta Force",
    };
    [SerializeField] private SnapToItem m_TeamNamesSnap;

    [Header("Team Count")]
    [SerializeField] private TeamName m_TeamCountObject;
    [SerializeField] private Transform m_TeamCountContentParent;
    [SerializeField] private SnapToItem m_TeamCountSnap;

    [HideInInspector] public UnityEvent<TeamIconImageMapper> OnTeamAvatarChangedEvent = new UnityEvent<TeamIconImageMapper>();
    [HideInInspector] public UnityEvent<string> OnTeamNameChangedEvent = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<int> OnTeamCountChangedEvent = new UnityEvent<int>();

    private string m_DirectoryPath_Emblems = "";
    private int m_MaxTeamCount = 50;
    private const string m_TeamNamesPrefsKey = "TeamNames";

    private void Awake()
    {
        m_DirectoryPath_Emblems = Path.Combine(Application.persistentDataPath, "CustomEmblems");

        m_TeamAvatarSnap.OnCurrentItemChangedEvent.AddListener((index) => {
            TeamIconImageMapper mapper = new TeamIconImageMapper();
            var tIcon = m_TeamAvatarContentParent.GetChild(index).GetComponent<TeamIcon>();
            var icnMat = tIcon.GetSpriteMaterial();

            if(icnMat.Item1 != null)
            {
                mapper.hasMaterial = false;
                mapper.SPRITE = icnMat.Item1;
            }

            if (icnMat.Item2 != null)
            {
                mapper.hasMaterial = true;
                mapper.MATERIAL = icnMat.Item2;
            }

            OnTeamAvatarChangedEvent?.Invoke(mapper);
        });

        m_TeamNamesSnap.OnCurrentItemChangedEvent.AddListener((index) => {
            var tName = m_TeamNameContentParent.GetChild(index).GetComponent<TeamName>();
            OnTeamNameChangedEvent?.Invoke(tName.GetTeamName());
        });

        m_TeamCountSnap.OnCurrentItemChangedEvent.AddListener((index) => {
            var tCount = m_TeamCountContentParent.GetChild(index).GetComponent<TeamName>();
            OnTeamCountChangedEvent?.Invoke(int.Parse(tCount.GetTeamName()));
        });
    }

    private void OnDestroy()
    {
        m_TeamAvatarSnap.OnCurrentItemChangedEvent.RemoveAllListeners();

        m_TeamNamesSnap.OnCurrentItemChangedEvent.RemoveAllListeners();

        m_TeamCountSnap.OnCurrentItemChangedEvent.RemoveAllListeners();
    }

    public void InitRematchDuelObject(SettingsItemData data)
    {
        // Data Contains Defaults (Previous Selected Values)

        // Team Avatar
        RefreshTeamIcons(data.TeamEmblem);

        // Team Name
        RefreshTeamNames(data.TeamName);

        // Team Count
        RefreshTeamCount(data.TeamCount);
    }

    #region Team Avatar
    private void RefreshTeamIcons(TeamIconImageMapper defaultAvatar)
    {
        // clean
        List<TeamIconImageMapper> ORDERED_MAPPERS = new List<TeamIconImageMapper>();

        for (int i = 0; i < m_TeamAvatarContentParent.childCount; i++)
        {
            Destroy(m_TeamAvatarContentParent.GetChild(i).gameObject);
        }

        // First
        ORDERED_MAPPERS.Add(defaultAvatar);

        try
        {
            // 1st add custom icons
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


                TeamIconImageMapper mapper_MAT = new TeamIconImageMapper();
                mapper_MAT.hasMaterial = true;
                mapper_MAT.MATERIAL = newMaterial;

                if(mapper_MAT.MATERIAL.mainTexture.name != 
                    (defaultAvatar.hasMaterial ? 
                    defaultAvatar.MATERIAL.mainTexture.name : defaultAvatar.SPRITE.name))
                {
                    ORDERED_MAPPERS.Add(mapper_MAT);
                }
            }
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }


        // add default icons
        for (int i = 0; i < m_BuildInEmblems.Count; i++)
        {
            TeamIconImageMapper mapper_SPRITE = new TeamIconImageMapper();
            mapper_SPRITE.hasMaterial = false;
            mapper_SPRITE.SPRITE = m_BuildInEmblems[i];

            if (mapper_SPRITE.SPRITE.name !=
                    (defaultAvatar.hasMaterial ?
                    defaultAvatar.MATERIAL.mainTexture.name : defaultAvatar.SPRITE.name))
            {
                ORDERED_MAPPERS.Add(mapper_SPRITE);
            }
        }

        // add last one alpha 0
        TeamIconImageMapper mapper = new TeamIconImageMapper();
        mapper.hasMaterial = false;
        mapper.SPRITE = m_BuildInEmblems[0];
        ORDERED_MAPPERS.Add(mapper);

        // Instantiate
        for (int i = 0; i < ORDERED_MAPPERS.Count; i++)
        {
            var avatar = Instantiate(m_TeamAvatarObject, m_TeamAvatarContentParent);
            avatar.SetTeamIconImageViaMapper(ORDERED_MAPPERS[i]);

            if(i == ORDERED_MAPPERS.Count - 1)
            {
                // last hack
                avatar.SetTeamIconImageAlpha(0f);
            }
        }
    }
    #endregion

    #region Team Name
    private void RefreshTeamNames(string defaultTeamName)
    {
        // clear current children
        for (int i = 0; i < m_TeamNameContentParent.childCount; i++)
        {
            Destroy(m_TeamNameContentParent.GetChild(i).gameObject);
        }

        // load custom team names
        var tNames = LoadOrderedTeamNames(defaultTeamName);
        tNames.Insert(0, defaultTeamName);

        for (int i = 0; i < tNames.Count; i++)
        {
            GameObject tName = Instantiate(m_TeamNameObject.gameObject, m_TeamNameContentParent);
            tName.GetComponent<TeamName>().SetTeamNameText(tNames[i]);
        }
    }

    private List<string> LoadOrderedTeamNames(string defaultName)
    {
        List<string> tNames = new List<string>();

        string currentTeams = PlayerPrefs.GetString(m_TeamNamesPrefsKey, "");

        var teams = currentTeams.Split(',');

        for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i] == string.Empty || teams[i] == defaultName)
            {
                continue;
            }

            tNames.Add(teams[i]);
        }

        tNames.Reverse();

        // load build it team names
        for (int i = 0; i < m_BuildInTeamNames.Count; i++)
        {
            if(m_BuildInTeamNames[i] == defaultName)
            {
                continue;
            }

            tNames.Add(m_BuildInTeamNames[i]);
        }

        return tNames;
    }
    #endregion

    #region Team Count
    private void RefreshTeamCount(int defaultCount)
    {
        // clear children
        for (int i = 0; i < m_TeamCountContentParent.childCount; i++)
        {
            Destroy(m_TeamCountContentParent.GetChild(i));
        }

        // add numbers
        for (int i = 1; i < m_MaxTeamCount; i++)
        {
            var tCount = Instantiate(m_TeamCountObject, m_TeamCountContentParent);
            tCount.GetComponent<TeamName>().SetTeamNameText($"{i}");

            if(i == defaultCount)
            {
                tCount.transform.SetAsFirstSibling();
            }
        }
    }
    #endregion
}
