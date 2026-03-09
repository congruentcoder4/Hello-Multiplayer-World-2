using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;




namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {

        VisualElement root;

        Button hostButton;
        Button clientButton;
        Button serverButton;
        Button clickButton;

        Label statusLabel;
        Label scoreLabel;

        ClickGame clickGame;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;
            root.Clear();

            hostButton = CreateButton("Host", "Host");
            clientButton = CreateButton("Client", "Client");
            serverButton = CreateButton("Server", "Server");
            clickButton = CreateButton("Click", "Click!");

            statusLabel = CreateLabel("Status", "Not Connected");
            scoreLabel = CreateLabel("Score", "Host: 0 | Client: 0");

            root.Add(hostButton);
            root.Add(clientButton);
            root.Add(serverButton);
            root.Add(clickButton);
            root.Add(statusLabel);
            root.Add(scoreLabel);

            clickGame = FindFirstObjectByType<ClickGame>();

            if (clickGame != null)
{
    clickGame.HostScore.OnValueChanged += (_, _) => UpdateScore();
    clickGame.ClientScore.OnValueChanged += (_, _) => UpdateScore();
    clickGame.GameOver.OnValueChanged += OnGameOver;   // ✅ ADD THIS LINE

    UpdateScore();
}
            hostButton.clicked += () =>
            {
                NetworkManager.Singleton.StartHost();
                statusLabel.text = "Host started";
                HideStartButtons();
            };

            clientButton.clicked += () =>
            {
                NetworkManager.Singleton.StartClient();
                statusLabel.text = "Client started";
                HideStartButtons();
            };

            serverButton.clicked += () =>
            {
                NetworkManager.Singleton.StartServer();
                statusLabel.text = "Server started";
                HideStartButtons();
            };

            clickButton.clicked += () =>
            {
                if (clickGame != null)
                    clickGame.AddPointServerRpc();
            };
        }

        void HideStartButtons()
        {
            hostButton.style.display = DisplayStyle.None;
            clientButton.style.display = DisplayStyle.None;
            serverButton.style.display = DisplayStyle.None;
        }

        void UpdateScore()
        {
            if (clickGame == null) return;
            scoreLabel.text =
                $"Host: {clickGame.HostScore.Value} | Client: {clickGame.ClientScore.Value}";
        }

        void OnGameOver(bool oldValue, bool newValue)
        {
        if (!newValue) return;

         if (clickGame.WinnerClientId.Value == 0)
             statusLabel.text = "Host Wins!";
        else
             statusLabel.text = "Client Wins!";

         clickButton.SetEnabled(false);
        }
        Button CreateButton(string name, string text)
        {
            var b = new Button();
            b.name = name;
            b.text = text;
            return b;
        }

        Label CreateLabel(string name, string text)
        {
            var l = new Label();
            l.name = name;
            l.text = text;
            return l;
        }
    }
}