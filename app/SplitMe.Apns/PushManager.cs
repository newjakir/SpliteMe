using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SplitMe.Apns
{
    public class PushManager
    {
        public void SendTestPush(string deviceToken, string msg)
        {
            

            if(deviceToken.Length != 64)
            {
                deviceToken = "a619b266b4b3c384f7072493a67d71a9da71121ecf1cfcc6dc6f963c39d05603";
            }

            if(string.IsNullOrEmpty(msg))
            {
                msg = "Why null string dude?";
            }

            //string path = "LyrixPushServerCertificates.p12";
            //string path = Path.Combine(GetApplicationFolder(), @"E:\Projects\Lyrix\ApnsServices\Certificates\LyrixPushServerCertificates.p12");

            var appDomain = System.AppDomain.CurrentDomain;
            string path = Path.Combine(appDomain.BaseDirectory, "Content", "LyrixPushServerCertificates.pfx");

            var payload = new NotificationPayload(deviceToken, msg, 1, "default");
            //payload1.AddCustom("CustomKey", "CustomValue");
            var notificationList = new List<NotificationPayload> { payload };
            var push = new PushNotification(true, path, "123");
            var rejected = push.SendToApple(notificationList);
        }

        public void SendPush(string deviceToken, string msg)
        {
            var appDomain = System.AppDomain.CurrentDomain;
            string path = Path.Combine(appDomain.BaseDirectory, "Content", "LyrixPushServerCertificates.pfx");

            var payload = new NotificationPayload(deviceToken, msg, 1, "default");
            //payload1.AddCustom("CustomKey", "CustomValue");
            var notificationList = new List<NotificationPayload> { payload };
            var push = new PushNotification(true, path, "123");
            var rejected = push.SendToApple(notificationList);
        }

        public void Challenge(string deviceToken, Guid code, string fromUserFbName)
        {
            if (deviceToken.Length != 64) return;
            string msg = fromUserFbName + " has challenged you to a Lyrix Game.";

            var appDomain = System.AppDomain.CurrentDomain;
            string path = Path.Combine(appDomain.BaseDirectory, "Content", "LyrixPushServerCertificates.pfx");

            var payload = new NotificationPayload(deviceToken, msg, 1, "default");
            payload.AddCustom("chKey", code.ToString());
            payload.AddCustom("FromFbName", fromUserFbName);
            var notificationList = new List<NotificationPayload> { payload };
            var push = new PushNotification(true, path, "123");
            var rejected = push.SendToApple(notificationList);
        }
    }
}
