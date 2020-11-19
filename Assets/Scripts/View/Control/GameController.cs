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
			Debug.LogWarning("GameController Start");
			ShowNotification("0", "0", 0);
			ShowNotification("60", "60", 60);
		}

		public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
			UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
		}

		public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
			UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
		}


		void ShowNotification(string title, string text, int delay)
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
            var notification = new AndroidNotification(title, text, System.DateTime.Now.AddSeconds(delay));
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
#endif
		}

	    void OnApplicationPause(bool pauseStatus)
        {
			Debug.Log("GameController " + (pauseStatus ? "pause" : "unpause"));
		}
		void Awake()
		{
			Debug.LogWarning("GameController Awake");
			GDP.GetInstance().record("unity-awake");
			/*ShowNotification();
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var context = activity.Call<AndroidJavaObject>("getApplicationContext");
			var gdp = new AndroidJavaClass("ai.gosu.dataplatform.sdk.GDP");
			Debug.LogWarning("Before init call");
			//gdp.CallStatic("init", context);*/
			Debug.LogWarning("GameController Awake done");
		}
	}
}
