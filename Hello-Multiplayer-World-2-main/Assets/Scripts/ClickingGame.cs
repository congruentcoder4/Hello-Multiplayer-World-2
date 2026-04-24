using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class ClickGame : NetworkBehaviour
    {
        // Shared Scores
        public NetworkVariable<int> HostScore = new NetworkVariable<int>(0);
        public NetworkVariable<int> ClientScore = new NetworkVariable<int>(0);

        // Game State
        public NetworkVariable<bool> GameOver = new NetworkVariable<bool>(false);
        public NetworkVariable<ulong> WinnerClientId = new NetworkVariable<ulong>(999);

        private const int WIN_SCORE = 50;

        // Called when a player clicks the button
        [ServerRpc(RequireOwnership = false)]
        public void AddPointServerRpc(ServerRpcParams rpcParams = default)
        {
            // Prevent scoring after game ends
            if (GameOver.Value)
                return;

            ulong senderId = rpcParams.Receive.SenderClientId;

            // Increment correct player's score
            if (senderId == 0)
                HostScore.Value++;
            else
                ClientScore.Value++;

            CheckForWinner(senderId);
        }

        // Server checks win condition
        private void CheckForWinner(ulong lastScorer)
        {
            if (HostScore.Value >= WIN_SCORE)
            {
                GameOver.Value = true;
                WinnerClientId.Value = 0;
            }
            else if (ClientScore.Value >= WIN_SCORE)
            {
                GameOver.Value = true;
                WinnerClientId.Value = lastScorer;
            }
        }

        // Reset game
        [ServerRpc(RequireOwnership = false)]
        public void ResetScoresServerRpc()
        {
            HostScore.Value = 0;
            ClientScore.Value = 0;
            GameOver.Value = false;
            WinnerClientId.Value = 999;
        }
    }
}