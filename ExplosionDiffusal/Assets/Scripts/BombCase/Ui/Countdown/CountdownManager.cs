using UnityEngine;
// TODO: 
// -> countdown timer can be set in settings
// -> after the time runs out, hide the timer (shrinken to a visible position), touch blocker and go to initial game state
// -> nice to have: audio signalization that the match has started
public class CountdownManager : MonoBehaviour
{
    [SerializeField] private MainCanvas m_MainCanvas;
    [SerializeField] private float m_CountDownTimeInSec = 100;

    public void InitCountdown()
    {
        m_MainCanvas.InitCountdown(m_CountDownTimeInSec);
    }
}
