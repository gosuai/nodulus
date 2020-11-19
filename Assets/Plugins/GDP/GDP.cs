using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDP
{
    AndroidJavaObject gdp = null;
    AndroidJavaClass gdpClass = null;
    static GDP instance = null;
	Firebase.FirebaseApp app;

    public static GDP GetInstance()
    {
        Debug.LogWarning("GDP::GetInstance");
        //ShowNotification();
        //var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //var context = activity.Call<AndroidJavaObject>("getApplicationContext");
        if (instance == null) {
            instance = new GDP();
        }
        //this.gdp.Call<AndroidJavaObject>("event", "unity-awake").Call("record");
        Debug.LogWarning("GDP::GetInstance Done");
        return instance;
    }
    GDP() {
        Debug.LogWarning("GDP Creating instance");
        gdpClass = new AndroidJavaClass("ai.gosu.dataplatform.sdk.GDP");
        gdp = gdpClass.CallStatic<AndroidJavaObject>("getInstance");
        gdp.Call("start");
        initializeFirebase();
    }
    public void record(string eventName) {
        gdpClass.CallStatic<AndroidJavaObject>("event", eventName).Call("record");
    }
    public void initializeFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                gdp.Call("initializeFirebase");
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
}
