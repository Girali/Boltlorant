using Bolt.Samples.HeadlessServer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    static AppManager instance;

    public static AppManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AppManager>();                               

            return instance;
        }
    }

    [SerializeField]
    private HeadlessServerManager headlessServerManager = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!headlessServerManager.IsServer)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
