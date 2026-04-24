using Unity.Netcode;
using UnityEngine;

//this is where all netcode relate initialization should occur
public class RpcTest : NetworkBehaviour
{
   public override void OnNetworkSpawn()
   {
       if (!IsServer && IsOwner) 
       {
           TestServerRpc(0, NetworkObjectId);
       }
   }
   //a message gets printed to the console of client and host, Every time RPC is invoked on owning clients, value will go up by 1
   [Rpc(SendTo.ClientsAndHost)]
   void TestClientRpc(int value, ulong sourceNetworkObjectId)
   {
       Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
      
       if (value >= 20) return;   

    if (IsOwner)
        TestServerRpc(value + 1, sourceNetworkObjectId);
   }
   [Rpc(SendTo.Server)]
   void TestServerRpc(int value, ulong sourceNetworkObjectId)
   {
       Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
       TestClientRpc(value, sourceNetworkObjectId);
   }
}