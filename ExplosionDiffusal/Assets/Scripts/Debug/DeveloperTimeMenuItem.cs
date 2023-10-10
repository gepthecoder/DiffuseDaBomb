using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperTimeMenuItem : DeveloperItem
{
    [SerializeField] private Slider m_TimeScaleSlider;
    [SerializeField] private TextMeshProUGUI m_TimeScaleValueText;
    [Header("Admin")]
    [SerializeField] private GameObject m_AdminParent;
    [SerializeField] private TMP_InputField m_InputField;

    protected string __pass__ = "";

    private bool m_Admin = false;

    public override void Init() {
        base.Init();

        __pass__ = Developer.INSTANCE?.GetPass();

        m_TimeScaleSlider.onValueChanged.AddListener((value) => {
            Time.timeScale = value;
            m_TimeScaleValueText.text = $"{value}";
        });

        if(m_Admin)
        {
            m_AdminParent.SetActive(false);
        } else
        {
            m_InputField.onValueChanged.AddListener((pass) =>
            {
                if (pass == __pass__)
                {
                    m_AdminParent.SetActive(false);
                    m_Admin = true;
                }
            });
        }
    }
    
    public override void Deinit() {
        base.Deinit();

        m_TimeScaleSlider.onValueChanged.RemoveAllListeners();
        m_InputField.onValueChanged.RemoveAllListeners();
    }
}
