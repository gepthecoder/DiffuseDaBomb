using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SnapToItem : MonoBehaviour
{
    [SerializeField] private ScrollRect m_Scroll;
    [SerializeField] private RectTransform m_ContentPanel;
    [SerializeField] private RectTransform m_ContentItemList;

    [SerializeField] private HorizontalLayoutGroup m_HorizontalLG;
    [Space(5)]
    [SerializeField] [Range(20, 300)] private float m_SnapForce = 100;

    public bool m_Inited = false;
    private bool m_IsSnapped = false;

    private float m_SnapSpeed;

    private int m_PreviousItemIndex = 0;

    [HideInInspector] public int CurrentItem = 0;
    [HideInInspector] public UnityEvent<int> OnCurrentItemChangedEvent = new UnityEvent<int>();

    private void Start()
    {
        m_IsSnapped = false;
    }

    public void InitSnap()
    {
        m_Inited = true;
    }

    public void DeinitSnap()
    {
        m_Inited = false;
    }

    private void Update()
    {
        if(m_Inited)
        {
            CurrentItem =
                Mathf.RoundToInt(0 - m_ContentPanel.localPosition.x / (m_ContentItemList.rect.width + m_HorizontalLG.spacing));

            if(m_Scroll.velocity.magnitude < 200 && !m_IsSnapped)
            { // SNAPPING PROCESS
                m_Scroll.velocity = Vector2.zero;
                m_SnapSpeed += m_SnapForce * Time.deltaTime;

                float targetX = 0 - (CurrentItem * (m_ContentItemList.rect.width + m_HorizontalLG.spacing));
                m_ContentPanel.localPosition = new Vector3(
                    Mathf.MoveTowards(m_ContentPanel.localPosition.x, targetX, m_SnapSpeed),
                    m_ContentPanel.localPosition.y,
                    m_ContentPanel.localPosition.z
                );

                // Use a tolerance for comparison
                if (Mathf.Approximately(m_ContentPanel.localPosition.x, targetX))
                {
                    m_IsSnapped = true;

                    if (m_PreviousItemIndex != CurrentItem)
                    {
                        OnCurrentItemChangedEvent?.Invoke(CurrentItem);
                        m_PreviousItemIndex = CurrentItem;
                    }
                }

            }
            if(m_Scroll.velocity.magnitude > 200)
            {
                m_IsSnapped = false;
                m_SnapSpeed = 0;
            }
        }
    }
}
