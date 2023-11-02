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

        });

        m_TeamCountSnap.OnCurrentItemChangedEvent.AddListener((index) => {

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
        for (int i = 0; i < m_TeamAvatarContentParent.childCount; i++)
        {
            Destroy(m_TeamAvatarContentParent.GetChild(i).gameObject);
        }

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

                var newIcon = Instantiate(m_TeamAvatarObject.gameObject, m_TeamAvatarContentParent);
                newIcon.GetComponent<TeamIcon>().SetTeamIconMaterial(newMaterial);
            }
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }


        // add default icons
        for (int i = 0; i < m_BuildInEmblems.Count; i++)
        {
            var newIcon = Instantiate(m_TeamAvatarObject.gameObject, m_TeamAvatarContentParent);
            newIcon.GetComponent<TeamIcon>().SetTeamIconSprite(m_BuildInEmblems[i]);
        }

        // add last one alpha 0
        var lastIconHack = Instantiate(m_TeamAvatarObject.gameObject, m_TeamAvatarContentParent);
        lastIconHack.GetComponent<TeamIcon>().SetTeamIconImageAlpha(0);

        // move previous selected emblem to first position
        for (int i = 0; i < m_TeamAvatarContentParent.childCount; i++)
        {
            var icon = m_TeamAvatarContentParent.GetChild(i).GetComponent<TeamIcon>();
            if(icon.GetAvatarSourceID() == (defaultAvatar.hasMaterial ? 
                defaultAvatar.MATERIAL.mainTexture.name : defaultAvatar.SPRITE.name))
            {
                Destroy(m_TeamAvatarContentParent.GetChild(i));

                var firstIcon = Instantiate(m_TeamAvatarObject.gameObject, m_TeamAvatarContentParent);
                firstIcon.transform.SetAsFirstSibling();

                if(defaultAvatar.hasMaterial)
                {
                    firstIcon.GetComponent<TeamIcon>().SetTeamIconMaterial(defaultAvatar.MATERIAL);
                } else
                {
                    firstIcon.GetComponent<TeamIcon>().SetTeamIconSprite(defaultAvatar.SPRITE);
                }
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
        var tNames = LoadTeamNames();
        for (int i = 0; i < tNames.Count; i++)
        {
            GameObject tName = Instantiate(m_TeamNameObject.gameObject, m_TeamNameContentParent);
            tName.GetComponent<TeamName>().SetTeamNameText(tNames[i]);
        }
        // load build it team names
        for (int i = 0; i < m_BuildInTeamNames.Count; i++)
        {
            GameObject tName = Instantiate(m_TeamNameObject.gameObject, m_TeamNameContentParent);
            tName.GetComponent<TeamName>().SetTeamNameText(m_BuildInTeamNames[i]);
        }

        // move previous selected teamName to first position
        for (int i = 0; i < m_TeamNameContentParent.childCount; i++)
        {
            var tName = m_TeamNameContentParent.GetChild(i).GetComponent<TeamName>();
            if (tName.GetTeamName() == defaultTeamName)
            {
                Destroy(m_TeamNameContentParent.GetChild(i));

                var firstName = Instantiate(m_TeamNameObject.gameObject, m_TeamNameContentParent);
                firstName.transform.SetAsFirstSibling();

                firstName.GetComponent<TeamName>().SetTeamNameText(defaultTeamName);
            }
        }
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
