using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainSettingsController : MonoBehaviour
{
    [SerializeField] private Animator m_SettingsAnimator;
    [Space(5)]
    [SerializeField] private SnapToItem m_SnapToItemTeamEmblem;
    [Space(5)]
    [SerializeField] private FilePicker m_FilePicker;
    [Header("Team Emblems")]
    [SerializeField] private Material m_UploadedTextureMat;
    [SerializeField] private Image m_UploadedTextureImagePlaceholder;
    [SerializeField] private Texture2D m_DefaultTexture;
    [Space(5)]
    [SerializeField] private List<Sprite> m_BuildInEmblems = new List<Sprite>();
    [SerializeField] private Transform m_ContentParent;
    [SerializeField] private TeamIcon m_TeamIconPrefab;
    [SerializeField] private Material m_MaterialTemplate;

    private string m_DirectoryPath = "";

    private void Start()
    {
        m_DirectoryPath = Path.Combine(Application.persistentDataPath, "CustomEmblems");


        RefreshTeamIcons();
    }

    public void InitMainSettings()
    {
        m_SettingsAnimator.Play("SHOW");
        m_SnapToItemTeamEmblem.InitSnap();

        m_UploadedTextureMat.mainTexture = m_DefaultTexture;
        m_UploadedTextureImagePlaceholder.SetMaterialDirty();
    }

    public void DeinitMainSettings()
    {
        m_SettingsAnimator.Play("HIDE");
        m_SnapToItemTeamEmblem.DeinitSnap();
    }

    #region Team Emblem Settings

    public void OnAddCustomEmblemButtonClicked()
    {
        string path = m_FilePicker.LoadSelectedFilePath();
        StartCoroutine(LoadTexture(path));
    }

    private IEnumerator LoadTexture(string PATH)
    {
        WWW www = new WWW(PATH);
        while (!www.isDone)
            yield return null;

        var TEX = www.texture;

        m_UploadedTextureMat.mainTexture = TEX;
        m_UploadedTextureImagePlaceholder.SetMaterialDirty();

        // encode
        byte[] encodedImage = TEX.EncodeToPNG();

        // write to file
        var wPath = Path.Combine(m_DirectoryPath, $"{Random.Range(0, 100000000)}_Emblem.png");
        File.WriteAllBytes(wPath, encodedImage);

        // refresh ui
        RefreshTeamIcons();
    }

    private void RefreshTeamIcons()
    {
        // clean
        for (int i = 0; i < m_ContentParent.childCount; i++)
        {
            Destroy(m_ContentParent.GetChild(i).gameObject);
        }

        try
        {
            if (!Directory.Exists(m_DirectoryPath))
            {
                Directory.CreateDirectory(m_DirectoryPath); // Ensure the directory exists
            }
            
            // add first custom icons
            string[] filePaths = Directory.GetFiles(m_DirectoryPath, "*.png");

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

                var newIcon = Instantiate(m_TeamIconPrefab.gameObject, m_ContentParent);
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
            var newIcon = Instantiate(m_TeamIconPrefab.gameObject, m_ContentParent);
            newIcon.GetComponent<TeamIcon>().SetTeamIconSprite(m_BuildInEmblems[i]);
        }

        // add last one alpha 0
        var lastIconHack = Instantiate(m_TeamIconPrefab.gameObject, m_ContentParent);
        lastIconHack.GetComponent<TeamIcon>().SetTeamIconImageAlpha(0);
    }

    #endregion

    #region Team Name Settings

    #endregion

    #region Audio Settings

    #endregion
}
