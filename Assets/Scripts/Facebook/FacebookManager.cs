using UnityEngine;
using System.Collections;
using Facebook.Unity;
using Facebook;
using System.Collections.Generic;
using System;

public class FacebookManager : MonoBehaviour
{
    //OTHER
    float defaultTimescale = 1;

    #region MyFacebookUtilities

    void Awake()
    {
        //If facebook is not initialized yet
        if (!FB.IsInitialized)
        {
            //We initialize the facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        //If facebook is initialized
        if (FB.IsInitialized)
        {
            //We activate it
            FB.ActivateApp();

            //continue...
        }
        else
        {
            Debug.Log("Failed to intialize the facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            //We pause the game - we will need to hide.
            Time.timeScale = 0;
        }
        else
        {
            //Resume the game
            Time.timeScale = defaultTimescale;
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }

    #endregion

    //This method launches facebook login.
    public void LaunchLogin()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    //Present the user with an opportunity to share a story to their timeline.
    public void LaunchShareContent()
    {
        //TMP URL
        FB.ShareLink(
            new Uri("https://developers.facebook.com/"), callback: ShareCallback);
    }

    //An example of how we could log an app event.
    public void TMPVOID()
    {
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = "tutorial_step_1";
        tutParams[AppEventParameterName.Description] = "_description of the task";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent(
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
    }

    //completion event.
    public void SendRequest()
    {
        FB.AppRequest(
            "Come play this great game!",
            null, null, null, null, null, null,
            delegate (IAppRequestResult result) {
            Debug.Log(result.RawResult);


            /*message: "I Just got " + GameStateManager.Score.ToString() + " points! Can you beat it?",
            to: recipients,
            data: "{\"challenge_score\":" + GameStateManager.Score.ToString() + "}"
            title: "-Game challenge!",
            callback: appRequestCallback*/


        }
);
    }
}
