
//#if UNITY_EDITOR
#if ADS_MODULE

using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Prototype.Ads
{

    [System.Serializable]
    public class InterstitialAd : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] string _androidAdUnitId = "Interstitial_Android";
        [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";

        string _adUnitId;
        public bool isLoaded;
        public void Init()
        {
            // Get the Ad Unit ID for the current platform:
            _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOsAdUnitId
                : _androidAdUnitId;
        }

        // Load content to the Ad Unit:
        public void LoadAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);

            Advertisement.Load(_adUnitId, this);
        }

        // Show the loaded content in the Ad Unit:
        public void ShowAd()
        {
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + _adUnitId);

            Advertisement.Show(_adUnitId, this);
        }

        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            isLoaded = true;

            Debug.Log("Ad Loaded: " + adUnitId);
            // Optionally execute code if the Ad Unit successfully loads content.
        }

        public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }

        public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        }

        public void OnUnityAdsShowStart(string _adUnitId) { }
        public void OnUnityAdsShowClick(string _adUnitId) { }
        public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            LoadAd();
        }
    }

    [System.Serializable]
    public class RewardedAds : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] string _androidAdUnitId = "Rewarded_Android";
        [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
        string _adUnitId = null; // This will remain null for unsupported platforms
        private Action m_SnowCompleteCallback;

        private Action m_LoadCallback;
        public bool isLoaded;

        public void Init()
        {
            // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
            _adUnitId = _androidAdUnitId;
#endif
        }

        // Call this public method when you want to get an ad ready to show.
        public void LoadAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);
            Advertisement.Load(_adUnitId, this);
        }

        // Call this public method when you want to get an ad ready to show.
        public void LoadAd(Action loadCallback)
        {
            m_LoadCallback = loadCallback;
            LoadAd();
        }

        // If the ad successfully loads, add a listener to the button and enable it:
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad Loaded: " + adUnitId);

            if (adUnitId.Equals(_adUnitId))
            {
                isLoaded = true;
                m_LoadCallback?.Invoke();
            }
        }

        // Implement a method to execute when the user clicks the button:
        public void ShowAd()
        {
            Advertisement.Show(_adUnitId, this);

        }

        public void ShowAd(Action callback = null)
        {
            Advertisement.Show(_adUnitId, this);
            m_SnowCompleteCallback = callback;
        }

        // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Debug.Log("Unity Ads Rewarded Ad Completed");
                // Grant a reward.

                m_SnowCompleteCallback?.Invoke();
            }

            LoadAd();
        }

        // Implement Load and Show Listener error callbacks:
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Use the error details to determine whether to try to load another ad.
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Use the error details to determine whether to try to load another ad.
        }

        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
    }

    public interface IRewardAdPlayer
    {
        public void ShowRewardAd(Action callback);
        public bool RewardIsReady { get; }
    }
    public interface IInterstitialAdPlayer
    {
        public void ShowInterstitialAd();
        public bool InterstitialIsReady { get; }
    }

    public interface IAdsPlayer : IInterstitialAdPlayer, IRewardAdPlayer
    { 

    }

    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IAdsPlayer
    {
        [SerializeField] string _androidGameId;
        [SerializeField] string _iOSGameId;
        [SerializeField] bool _testMode = true;
        private string _gameId;
        public bool disableAds;
        [SerializeField]
        InterstitialAd m_InterstitialAdd;

        [SerializeField]
        RewardedAds m_RewardAds;

        void Awake()
        {
            InitializeAds();
            m_RewardAds.Init();
            m_InterstitialAdd.Init();
        }

        public void ShowInterstitialAd()
        {
            if (disableAds)
                return;

            m_InterstitialAdd.ShowAd();
        }

        public void ShowRewardAd(Action callback)
        {
            if (disableAds)
            {
                callback?.Invoke();
                return;
            }

            m_RewardAds.ShowAd(callback);
        }

        public bool InterstitialIsReady => m_InterstitialAdd.isLoaded;
        public bool RewardIsReady => m_InterstitialAdd.isLoaded;

        private void InitializeAds()
        {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
            _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            m_RewardAds.LoadAd();
            m_InterstitialAdd.LoadAd();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
    }
}
#endif