using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperTimeMenuItem : DeveloperItem
{
    [SerializeField] private Slider m_TimeScaleSlider;
    [SerializeField] private TextMeshProUGUI m_TimeScaleValueText;

    public override void Init() {
        base.Init();

        m_TimeScaleSlider.onValueChanged.AddListener((value) => {
            Time.timeScale = value;
            m_TimeScaleValueText.text = $"{value}";
        });
    }

    public override void Deinit() {
        base.Deinit();

        m_TimeScaleSlider.onValueChanged.RemoveAllListeners();
    }
}
