using System;
using Bolt.Matchmaking;
using Bolt.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.HeadlessServer
{
    public class HeadlessServerManager : Bolt.GlobalEventListener
    {
        [SerializeField]
        private string _map = "";
        [SerializeField]
        private string _gameType = "";
        [SerializeField]
        private string _roomID = "";
        [SerializeField]
        private bool _isServer = false;

        public bool IsServer { get => _isServer; set => _isServer = value; }

        public override void BoltStartBegin()
        {
            // Register any Protocol Token that are you using
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        }

        public override void BoltStartDone()
        {
            if (BoltNetwork.IsServer)
            {
                // Create some room custom properties
                PhotonRoomProperties roomProperties = new PhotonRoomProperties();

                roomProperties.AddRoomProperty("t", _gameType); // ex: game type
                roomProperties.AddRoomProperty("m", _map); // ex: map id

                roomProperties.IsOpen = true;
                roomProperties.IsVisible = true;

                // If RoomID was not set, create a random one
                if (_roomID.Length == 0)
                {
                    _roomID = Guid.NewGuid().ToString();
                }

                // Create the Photon Room
                BoltMatchmaking.CreateSession(
                    sessionID: _roomID,
                    token: roomProperties,
                    sceneToLoad: _map
                );
            }
        }

        // Use this for initialization
        void Awake()
        {
            // Get custom arguments from command line
            _isServer = "true" == (GetArg("-s", "-isServer") ?? (_isServer ? "true" : "false"));
            _map = GetArg("-m", "-map") ?? _map;
            _gameType = GetArg("-t", "-gameType") ?? _gameType; // ex: get game type from command line
            _roomID = GetArg("-r", "-room") ?? _roomID;

            if (IsServer)
            {
                // Validate the requested Level
                var validMap = false;

                foreach (string value in BoltScenes.AllScenes)
                {
                    if (SceneManager.GetActiveScene().name != value)
                    {
                        if (_map == value)
                        {
                            validMap = true;
                            break;
                        }
                    }
                }

                if (!validMap)
                {
                    BoltLog.Error("Invalid configuration: please verify level name");
                    Application.Quit();
                }

                // Start the Server
                BoltLauncher.StartServer();
                DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Utility function to detect if the game instance was started in headless mode.
        /// </summary>
        /// <returns><c>true</c>, if headless mode was ised, <c>false</c> otherwise.</returns>
        public static bool IsHeadlessMode()
        {
            return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
        }

        static string GetArg(params string[] names)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                foreach (var name in names)
                {
                    if (args[i] == name && args.Length > i + 1)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }
    }
}