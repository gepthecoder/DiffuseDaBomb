using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    PlayAgain,
    ExitGame,
    MatchDetails,
    Null,
    Default
}

public class Fader : MonoBehaviour
{
    public static Fader INSTANCE;

    [SerializeField] private Animator m_FadeInOutAnime;

    private TransitionType m_TransitionType = TransitionType.Default;

    private bool m_CanInitialize = true;
    private float time;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        print("FADER AWAKE");
    }

    private void Update()
    {
        if(!m_CanInitialize)
        {
            time += Time.deltaTime;

            if(time > 3f)
            {
                m_CanInitialize = true;
                time = 0f;
            }
        }
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
        print(scene.name + " | " + m_TransitionType);

        if(m_CanInitialize)
        {
            var gm = FindObjectOfType<GameManager>();
            gm.InitializeGame(m_TransitionType);

            m_CanInitialize = false;
        }
    }

}
