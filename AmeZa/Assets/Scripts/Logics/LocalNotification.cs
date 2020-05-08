using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using SeganX;

public class LocalNotification : MonoBehaviour
{
    private bool localPushScheduled = false;

    private void Start()
    {
        AndroidNotificationCenter.CancelAllNotifications();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) ScheduleLocalPush();
    }

    private void OnApplicationQuit()
    {
        ScheduleLocalPush();
    }

    private void ScheduleLocalPush()
    {
        if (localPushScheduled) return;
        localPushScheduled = true;

        try
        {
            OnScheduleNotification?.Invoke();
        }
        catch { }

        var c = new AndroidNotificationChannel()
        {
            Id = Application.identifier,
            Name = Application.productName,
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);

        foreach (var item in GlobalConfig.Notifications.items)
        {
            if (item.texts.Count < 0) continue;

            int index = PlayerPrefs.GetInt(name + ".Notif." + item.id + ".Index", 0);
            PlayerPrefs.SetInt(name + ".Notif." + item.id + ".Index", index + 1);

            SendNotification(item.delaySeconds, item.texts[index % item.texts.Count]);
        }
    }


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    public static event System.Action OnScheduleNotification = null;

    public static void SendNotification(int delaySeconds, string text, string largIcon = "app_icon")
    {
        var parts = text.Split('|');
        if (parts.Length < 2 || parts[0].IsNullOrEmpty() || parts[1].IsNullOrEmpty()) return;
        SendNotification(delaySeconds, parts[0], parts[1], largIcon);
    }

    public static void SendNotification(int delaySeconds, string title, string message, string largIcon)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = message,
            FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
            LargeIcon = largIcon
        };

        AndroidNotificationCenter.SendNotification(notification, Application.identifier);
    }

    [Console("game", "notif", "(delayInSeconds) Test local notification system")]
    public static void TestNotif(int delayInSeconds)
    {
        var notification = new AndroidNotification();
        notification.Title = "Test Notif";
        notification.Text = "This is a test notification!";
        notification.FireTime = System.DateTime.Now.AddSeconds(delayInSeconds);
        notification.LargeIcon = "app_icon";

        AndroidNotificationCenter.SendNotification(notification, Application.identifier);
    }
}
