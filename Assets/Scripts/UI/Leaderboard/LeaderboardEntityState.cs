// Networkwork Data Sync variable has to be a struct ~ not a class with refs

using System;
using Unity.Collections;
using Unity.Netcode;


//  + Impliment a serializable state
// Transform State data into bytes, send over network, deserialize via ()NetworkSerialize
// IEquatable compares equality of data types ()Equals. Looking for change.
public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{

    public ulong ClientId;
    public FixedString32Bytes PlayerName; //    Network string
    public int Coins;



    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderboardEntityState other)
    {
        return ClientId == other.ClientId &&
            PlayerName.Equals(other.PlayerName) &&
            Coins == other.Coins;
    }

}