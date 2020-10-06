using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    private static AppManager _instance;

    public string Username
    {
        get
        {
            return PlayerPrefs.GetString("Username","None");
        }

        set
        {
            PlayerPrefs.GetString("Username",value);
        }
    }

    public static AppManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AppManager>();                               

            return _instance;
        }
    }

    [SerializeField]
    private HeadlessServerManager _headlessServerManager = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!_headlessServerManager.IsServer)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
