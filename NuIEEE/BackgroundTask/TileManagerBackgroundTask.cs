using NuIEEE;
using NuIEEE.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class TileManagerBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral taskDeferral = taskInstance.GetDeferral();

            UpdateLiveTile(taskDeferral);
        }

        private async void UpdateLiveTile(BackgroundTaskDeferral taskDeferral)
        {
            var someonePresent = await API.GetAsync<SomeonePresent>(API.Actions.CheckIfSomeoneIsPresent);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text03);

            string titleText = "NuIEEE Room:\n" + (someonePresent.Result.Value ? "Has people working" : "Is empty");
            tileXml.GetElementsByTagName("text")[0].InnerText = titleText;

            updater.Update(new TileNotification(tileXml));

            taskDeferral.Complete();
        }

    }
}
