using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace View.Control
{
	/// <summary>
	/// The main game controller.
	/// </summary>
	public class GameController : MonoBehaviour
	{
		private void Update() 
		{			
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
		}

		void Start()
		{
			Invoke(nameof(ShowNotifications), 10);
		}
		void ShowNotifications () {
			ShowNotification(0, "0", "0", 0, 100, "intent-data-0", true);
			ShowNotification(60, "60", "60", 60, 600, "intent-data-60", false);
			ShowNotification(120, "120", "120", 120, 1200, "intent-data-120", false);
		}
	
		public void InitializeFirebase() {
			Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				var dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available) {
					// Create and hold a reference to your FirebaseApp,
					// where app is a Firebase.FirebaseApp property of your application class.

					Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
					Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
				} else {
					Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
					// Firebase Unity SDK is not safe to use here.
				}
			});
		}

		public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
			Debug.Log("GameController::OnTokenReceived Token: " + token.Token);
		}

		public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
			Debug.Log("GameController::OnMessageReceived from: " + e.Message.From);
		}

		void ShowNotification(int id, string title, string text, int delay, int number, string intentData, bool showTimestamp)
		{
#if UNITY_ANDROID
            var channel = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
			var notification = new AndroidNotification(title, text, System.DateTime.Now.AddSeconds(delay))
			{
				Number = number, IntentData = intentData, ShowTimestamp = showTimestamp
			};
			AndroidNotificationCenter.SendNotification(notification, channel.Id);
#endif
		}

	    void OnApplicationPause(bool pauseStatus)
        {
			Debug.Log("GameController " + (pauseStatus ? "pause" : "unpause"));
		}
		void Awake()
		{
			Debug.Log("GameController Awake");
		}
	}
}
