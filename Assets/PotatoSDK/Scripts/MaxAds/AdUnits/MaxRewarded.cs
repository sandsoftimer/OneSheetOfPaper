using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PotatoSDK
{
#if POTATO_MAX
    public class MaxRewarded
    {
        bool log;
        public MaxRewarded(string adUnitId, bool log = false)
        {
            this.log = log;
            //Instance = this;
            this.adUnitId = adUnitId;
            InitializeRewardedAds();

        }
        string adUnitId = "YOUR_AD_UNIT_ID";



        Action<bool> requestedActionOnComplete;
        bool requestPending;

        void OnRequestComplete(bool success)
        {
            requestedActionOnComplete?.Invoke(success);
            requestPending = false;
        }
        public void ShowAd(Action<bool> onComplete)
        {
            if (requestPending)
            {
                if(log) Debug.Log("<color=#ff00ff>Previous request pending...</color>");
                onComplete?.Invoke(false);
                return;
            }
            if (!MaxSdk.IsRewardedAdReady(adUnitId))
            {
                if (log) Debug.Log("<color=#ff00ff>Ad not ready...</color>");
                onComplete?.Invoke(false);
                return;
            }
            requestedActionOnComplete = onComplete;
            requestPending = true;

            MaxSdk.ShowRewardedAd(adUnitId);
        }


        int retryAttempt;
        private void LoadRewardedAd()
        {
            if (log) Debug.LogFormat("AD load requested");
            MaxSdk.LoadRewardedAd(adUnitId);
        }
        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }



        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (log) Debug.LogFormat("AD loaded");
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            retryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            retryAttempt++;
            if (log) Debug.LogFormat("Ad failed to display... attempt {0}", retryAttempt);
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));//0,1,2,4,8,16,32,64,64,64

            Centralizer.Add_DelayedAct(LoadRewardedAd, (float)retryDelay);
            //mono.Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();

            if (log) Debug.Log("<color=#ff00ff>Ad failed to display...</color>");
            if (requestPending) OnRequestComplete(false);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
            if (log) Debug.Log("<color=#ff00ff>Ad closed by user...</color>");

            if (requestPending) OnRequestComplete(false);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            if (log) Debug.Log("<color=#ff55ff>Ad successful...</color>");
            if (requestPending) OnRequestComplete(true);
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
        }

    }
#endif
}
