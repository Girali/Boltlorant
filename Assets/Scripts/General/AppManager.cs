using Bolt.Samples.HeadlessServer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    private static AppManager _instance;

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
