using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    PlayAgain,
    ExitGame,
    MatchDetails,
    Null,
}

public class Fader : MonoBehaviour
{
    public static Fader INSTANCE;

    [SerializeField] private Animator m_FadeInOutAnime;

    private TransitionType m_TransitionType;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(m_TransitionType == TransitionType.PlayAgain)
        {
            var cfg = Config.INSTANCE.GetGlobalConfig();

            RematchManager.INSTANCE.InitRematchModule(cfg);

            m_TransitionType = TransitionType.Null;
        }
    }

}
