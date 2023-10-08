using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    PlayAgain,
    ExitGame,
    MatchDetails,
}

public class Fader : MonoBehaviour
{
    public static Fader INSTANCE;

    [SerializeField] private Animator m_FadeInOutAnime;

    private TransitionType m_TransitionType;

    private void Awake()
    {
        if(INSTANCE == null) {
            INSTANCE = this;
            DontDestroyOnLoad(this);
        }
    }

    public void FadeToMainScene(TransitionType transtionType)
    {
        m_TransitionType = transtionType;
        m_FadeInOutAnime.Play("FadeInOut");
    }

    public void LoadScene() // CALLED FROM ANIME
    {
        SceneManager.LoadScene(0);
    }


}
