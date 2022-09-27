using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace KeypadSystem
{
    public class KPDisableManager : MonoBehaviour
    {
        [SerializeField] private FirstPersonController player = null;

        public static KPDisableManager instance;

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); }
            else { instance = this; DontDestroyOnLoad(gameObject); }
        }

        public void DisablePlayer(bool disable)
        {
            player.enabled = !disable;
            KPUIManager.instance.ShowCrosshair(disable);
        }
    }
}
