using NuIEEE.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NuIEEE
{
    public sealed partial class MainPage : Page
    {
        private const string CHECKING = "Checking if someone is in the room ...";
        private const string YES = "Yes, someone is at NuIEEE working.";
        private const string NO = "No, nobody is at NuIEEE at the moment.";
        private const string NO_CONNECTION = "Not connected. Please make sure your data connection is on.";
        private const string SERVER_ERROR = "An internal server error has occurred. Code monkeys have been dispatched to fix the problem.";

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Status.Text = CHECKING;
            UpdateStatusText();
            RegisterBackgroundTask();
        }

        private async void UpdateStatusText()
        {
            try
            {
                var someonePresent = await API.GetAsync<SomeonePresent>(API.Actions.CheckIfSomeoneIsPresent);
                if (someonePresent.Result.HasValue == false)
                {
                    Status.Text = SERVER_ERROR;
                }
                else
                {
                    Status.Text = someonePresent.Result.Value ? YES : NO;
                }
            }
            catch
            {
                Status.Text = NO_CONNECTION;
            }
        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }

        private const string taskName = "TileManagerBackgroundTask";
        private const string taskEntryPoint = "BackgroundTask.TileManagerBackgroundTask";
    }
}