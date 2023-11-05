using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HistoryItemData
{
    public string WinningTeamEmblemNameString;
    public string LosingTeamEmblemNameString;

    public string WinningTeamNameString;
    public string LosingTeamNameString;

    public int WinningTeamScore;
    public int LosingTeamScore;

    public string EndMatchDateString;
    public string EndMatchTimeString;

    public HistoryItemData() { }
    public HistoryItemData(string Wemblem, string Lemblem, string WteamName, string LteamName, int Wscore, int Lscore, string endDate, string endTime)
    {

        this.WinningTeamEmblemNameString = Wemblem;
        this.LosingTeamEmblemNameString = Lemblem;
        this.WinningTeamNameString = WteamName;
        this.LosingTeamNameString = LteamName;
        this.WinningTeamScore = Wscore;
        this.LosingTeamScore = Lscore;
        this.EndMatchDateString = endDate;
        this.EndMatchTimeString = endTime;
    }
}

public class HistoryItemObject : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private TextMeshProUGUI m_IndexText;
    [Header("Emblem")]
    [SerializeField] private List<Sprite> m_AllSprites;
    [SerializeField] private TeamIcon m_WinningTeamImage;
    [SerializeField] private TeamIcon m_LosingTeamImage;
    [SerializeField] private Material m_MaterialTemplate;
    [Header("Team Name")]
    [SerializeField] private TextMeshProUGUI m_WinningTeamNameText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamNameText;
    [Header("Team Score")]
    [SerializeField] private TextMeshProUGUI m_WinningTeamScoreText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamScoreText;
    [Header("End Match")]
    [SerializeField] private TextMeshProUGUI m_EndMatchDateText;
    [SerializeField] private TextMeshProUGUI m_EndMatchTimeText;
    [Header("Status")]
    [SerializeField] private EndMatchTeamStatusIcon m_EndMatchTeamStatusIcon;


    public void SetHistoryItemObjectData(HistoryItemData data, int index)
    {
        if (data == null)
            return;

        m_IndexText.text = $"{index}";

        var wMapper = GetMapperByName(data.WinningTeamEmblemNameString);
        m_WinningTeamImage.SetTeamIconImageViaMapper(wMapper);

        var lMapper = GetMapperByName(data.LosingTeamEmblemNameString);
        m_LosingTeamImage.SetTeamIconImageViaMapper(lMapper);

        m_WinningTeamNameText.text = data.WinningTeamNameString;
        m_LosingTeamNameText.text = data.LosingTeamNameString;

        m_WinningTeamScoreText.text = $"{data.WinningTeamScore}";
        m_LosingTeamScoreText.text = $"{data.LosingTeamScore}";

        m_EndMatchDateText.text = $"{data.EndMatchDateString}";
        m_EndMatchTimeText.text = $"{data.EndMatchTimeString}";

        bool isDraw = data.WinningTeamScore == data.LosingTeamScore;
        m_EndMatchTeamStatusIcon.EnableStatusIcons(isDraw);
    }

    private TeamIconImageMapper GetMapperByName(string name)
    {
        // check default
        for (int i = 0; i < m_AllSprites.Count; i++)
        {
            if(m_AllSprites[i].name == name)
            {
                TeamIconImageMapper tiim = new TeamIconImageMapper();
                tiim.hasMaterial = false;
                tiim.SPRITE = m_AllSprites[i];
                return tiim;
            }
        }

        // check custom
        var customIcons = GetAllCustomIcons();

        for (int i = 0; i < customIcons.Count; i++)
        {
            if(customIcons[i].MATERIAL.name == name)
            {
                return customIcons[i];
            }
        }

        return null;
    }

    private List<TeamIconImageMapper> GetAllCustomIcons()
    {
        string[] filePaths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "CustomEmblems"), "*.png");

        List<byte[]> retrievedTextures = new List<byte[]>();
        List<TeamIconImageMapper> mappers = new List<TeamIconImageMapper>();

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

            TeamIconImageMapper mapper = new TeamIconImageMapper();
            mapper.hasMaterial = true;
            mapper.MATERIAL = newMaterial;

            mappers.Add(mapper);
        }

        return mappers;
    }
}
