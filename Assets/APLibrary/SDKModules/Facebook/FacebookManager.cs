#if AP_FACEBOOK_SDK_INSTALLED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{

	public static FacebookManager Instance;

#region Public Variables

	[Header("Debug Reference")]
	public Image userProfilePicture;
	public Text userID;
	public Text userName;
	public Text userEmail;
	public Text userPermission;
	public Text errorDisplay;

	[Header("Configuretion  :   Log In")]
	public UnityEvent OnFacebookLoggedInSuccessful;
	public UnityEvent OnFacebookLoggedInFailed;

	[Space(5.0f)]
	[Header("Configuretion  :   Share")]
#if UNITY_ANDROID
	[Space(5.0f)]
	public string appURL_Android;
#elif UNITY_IOS
    [Space (5.0f)]
    public string appURL_iOS;
#endif

	public UnityEvent OnFacebookShare;

#endregion

	//----------
#region Private Variables

	private string appURL;
	private string FACEBOOK_USER_ACCESS_TOKEN = "FACEBOOK_USER_ACCESS_TOKEN";
	private string FACEBOOK_USER_NAME = "FACEBOOK_USER_NAME";
	private string FACEBOOK_USER_EMAIL = "FACEBOOK_USER_EMAIL";
	private string FACEBOOK_USER_PROFILE_PICTURE_LOCATION = "USER_ACCESS_TOKEN_FOR_FACEBOOK";

#endregion


	void Awake()
	{

		if (Instance == null)
		{

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (Instance != this)
		{

			Destroy(gameObject);
		}

#if UNITY_ANDROID
		appURL = appURL_Android;
#elif UNITY_IOS
        appURL = appURL_iOS;
#endif

		if (!FB.IsInitialized)
		{

			FB.Init(InitCallback, OnHideUnity);

		}
		else
		{

			FB.ActivateApp();

		}
	}

	//----------
#region Configuretion

	private void InitCallback()
	{

		if (FB.IsInitialized)
		{
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...

			if (FB.IsLoggedIn)
			{
				FB.API("/me?fields=name", HttpMethod.GET, StoreUserName);
				FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, StoreUserProfilePicture);
			}
		}
		else
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
			if (errorDisplay != null)
			{
				errorDisplay.text = "Failed to Initialize the Facebook SDK";
			}
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			// Pause the game - we will need to hide
			//Time.timeScale = 0;
		}
		else
		{
			// Resume the game - we're getting focus again
			//Time.timeScale = 1;
		}
	}

#endregion

	//----------
#region FACEBOOK LOGIN

	List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };

	public string GetUserAccessToken()
	{
		return PlayerPrefs.GetString(FACEBOOK_USER_ACCESS_TOKEN);
	}

	public bool IsFacebookLoggedIn()
	{
		if (FB.IsLoggedIn)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void LogInFacebook()
	{

		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{

			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			PlayerPrefs.SetString(FACEBOOK_USER_ACCESS_TOKEN, aToken.UserId);

			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			if (userID != null)
			{
				userID.text = aToken.UserId;
			}

			string userPermissionInfo = "";
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log(perm);
				userPermissionInfo += (perm + ",");
			}
			if (userPermission != null)
			{
				userPermission.text = userPermissionInfo;
			}

			FB.API("/me?fields=name", HttpMethod.GET, StoreUserName);
			FB.API("/me?fields=email", HttpMethod.GET, StoreUserEmail);
			FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, StoreUserProfilePicture);

			OnFacebookLoggedInSuccessful.Invoke();

			Debug.Log("Login Successful");


		}
		else
		{
			Debug.Log("User cancelled login");
			if (errorDisplay != null)
			{
				errorDisplay.text = "User cancelled login";
			}

			OnFacebookLoggedInFailed.Invoke();

		}
	}

#endregion

	//----------
#region FACEBOOK SHARE

	public void ShareOnFacebook()
	{

		FB.ShareLink(
			contentURL: new System.Uri(appURL),
			callback: OnShareCallback);
	}

	private void OnShareCallback(IShareResult result)
	{

		if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
		{

			Debug.Log("ShareLink Error: " + result.Error);
			if (errorDisplay != null)
			{
				errorDisplay.text = "ShareLink Error: " + result.Error;
			}
		}
		else if (!string.IsNullOrEmpty(result.PostId))
		{
			Debug.Log("PostID : " + result.PostId);
			if (errorDisplay != null)
			{
				errorDisplay.text = "PostID : " + result.PostId;
			}
		}
		else
		{

			OnFacebookShare.Invoke();
			Debug.Log("ShareLink success!");
			if (errorDisplay != null)
			{
				errorDisplay.text = "ShareLink success!";
			}
		}
	}

#endregion

	//----------
#region FACEBOOK USER INFO

	private string userFacebookProfilePicture = "userFacebookProfilePicture.png";
	private string userFacebookInfoFolderName = "userFacebookInfoFolder";
	private string userFacebookInfoFolder;

	private void StoreUserName(IResult result)
	{
		if (result.Error != null)
		{
			Debug.Log("Failed to fetch user name : " + result.Error);
			if (errorDisplay != null)
			{
				errorDisplay.text = "Failed fetch user name : " + result.Error;
			}
		}
		else
		{

			PlayerPrefs.SetString(FACEBOOK_USER_NAME, result.ResultDictionary["name"].ToString());
			if (userName != null)
			{
				userName.text = result.ResultDictionary["name"].ToString();
			}

		}
	}

	public void StoreUserEmail(IResult result)
	{
		if (result.Error != null)
		{
			Debug.Log("Failed to fetch email : " + result.Error);
			if (errorDisplay != null)
			{
				errorDisplay.text = "Failed to fetch email : " + result.Error;
			}
		}
		else
		{

			PlayerPrefs.SetString(FACEBOOK_USER_EMAIL, result.ResultDictionary["email"].ToString());
			if (userEmail != null)
			{
				userEmail.text = result.ResultDictionary["email"].ToString();
			}

		}
	}

	private void StoreUserProfilePicture(IGraphResult result)
	{

		if (result.Error == null && result.Texture != null)
		{

			userFacebookInfoFolder = Application.persistentDataPath + "/" + userFacebookInfoFolderName;

			//  if(userProfilePicture != null){

			if (!System.IO.Directory.Exists(userFacebookInfoFolder))
			{
				System.IO.Directory.CreateDirectory(userFacebookInfoFolder);
			}

			System.IO.File.WriteAllBytes(userFacebookInfoFolder + "/" + userFacebookProfilePicture, result.Texture.EncodeToPNG());

			if (userProfilePicture != null)
			{

				userProfilePicture.sprite = GetUserProfilePicture();
			}

			//   }

		}
		else
		{
			Debug.Log("Unable to fetch profile picture : " + result.Error);
			if (errorDisplay != null)
			{
				errorDisplay.text = "Unable to fetch profile picture : " + result.Error;
			}
		}
	}

	public string GetUserName()
	{
		return PlayerPrefs.GetString(FACEBOOK_USER_NAME);
	}

	public string GetUsereEmail()
	{
		return PlayerPrefs.GetString(FACEBOOK_USER_EMAIL);
	}

	public Sprite GetUserProfilePicture()
	{

		byte[] imageData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + userFacebookInfoFolderName + "/" + userFacebookProfilePicture);//userFacebookInfoFolder + "/" + userFacebookProfilePicture);
		Texture2D userPPTexture = new Texture2D(128, 128, TextureFormat.PVRTC_RGB2, false);
		userPPTexture.LoadImage(imageData);

		return Sprite.Create(
			userPPTexture,
			new Rect(0, 0, userPPTexture.width, userPPTexture.height),
			new Vector2(0.5f, 0.5f),
			128f
		);
	}

#endregion
}
#endif