using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessages
{
    void OnMessage(Messages.Param param);
}

public static class Messages
{
    public enum Type : int
    {
        Null,
        StartTurn,
        EndTurn,
        TurnStarted,
        TurnEnded,
        BallCount,
        OnBlockHit,
        OnBlockDeath,
        BlockDead,
        AvatarChanged,
        BallPurchased,
        UseAbility,
    }

    public class Param
    {
        public Type type = Type.Null;
        public object value = null;
        public bool Is(Type expected) { return type == expected; }
        public bool Is<T>() { return value != null && value is T; }
        public T As<T>() { return (T)value; }
    }

    public const string MethodName = "OnMessage";

    public static void BroadcastMessage(Component sender, Type type, object parameter = null)
    {
        var data = new Param();
        data.type = type;
        data.value = parameter;
        sender.BroadcastMessage(MethodName, data, SendMessageOptions.DontRequireReceiver);
    }

    public static void Broadcast(this Component sender, Type type, object parameter = null)
    {
        BroadcastMessage(sender, type, parameter);
    }

    public static void Broadcast(this GameObject sender, Type type, object parameter = null)
    {
        BroadcastMessage(sender.transform, type, parameter);
    }

    public static void SendMessage(Component sender, Type type, object parameter = null)
    {
        var data = new Param();
        data.type = type;
        data.value = parameter;
        sender.SendMessage(MethodName, data, SendMessageOptions.DontRequireReceiver);
    }

    public static void Message(this Component sender, Type type, object parameter = null)
    {
        SendMessage(sender, type, parameter);
    }

    public static void Message(this GameObject sender, Type type, object parameter = null)
    {
        SendMessage(sender.transform, type, parameter);
    }

}
