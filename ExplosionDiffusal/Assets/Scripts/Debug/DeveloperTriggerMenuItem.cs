using UnityEngine;

public class DeveloperTriggerMenuItem : DeveloperItem
{
    public override void Init() {
        base.Init();
    }

    public override void Deinit() {
        base.Deinit();
    }

    public void QuitGame()
    {
        Fader.INSTANCE?.FadeToMainScene(TransitionType.ExitGame);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        Fader.INSTANCE?.FadeToMainScene(TransitionType.PlayAgain);
    }
}
