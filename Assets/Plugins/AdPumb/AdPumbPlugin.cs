using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdPumbPlugin {
    public class DisplayManager{
        private static readonly DisplayManager instance = new DisplayManager();
        #if UNITY_ANDROID
        AndroidJavaClass DisplayManagerClass  = new AndroidJavaClass("com.adpumb.ads.display.DisplayManager");
        #endif
        public static DisplayManager Instance {  
            get {  
                return instance;  
            }  
        }
        public DisplayManager(){
            #if UNITY_ANDROID
            // DisplayManagerClass = new AndroidJavaClass("com.adpumb.ads.display.DisplayManager");
            #endif
        }
        public void showAd(AdPlacementBuilder placementBuilder){
            #if UNITY_ANDROID
            DisplayManagerClass.CallStatic<AndroidJavaObject>("getInstance").Call("showAd",placementBuilder.build());
            #endif
        }
    }
    public class AdPlacementBuilder{
        #if UNITY_ANDROID
        AndroidJavaObject PlacementBuilderObject;
        #endif
        public static AdPlacementBuilder Interstitial(){
            return new AdPlacementBuilder("InterstitialPlacementBuilder");
        }
        public static AdPlacementBuilder Rewarded(){
            return new AdPlacementBuilder("RewardedPlacementBuilder");
        }
        public static AdPlacementBuilder AppOpenInterstitial(){
            return new AdPlacementBuilder("AppOpenInterstitialPlacementBuilder");
        }
        public static AdPlacementBuilder RewardedInterstitial(){
            return new AdPlacementBuilder("RewardedInterstitialPlacementBuilder");
        }
        public AdPlacementBuilder(string builderClassName ){
            #if UNITY_ANDROID
            PlacementBuilderObject = new AndroidJavaObject("com.adpumb.ads.display."+builderClassName);
            #endif
        }
        public AdPlacementBuilder name(string name){
            #if UNITY_ANDROID
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("name", new object[] { name} );
            #endif
            return this;
        }
        public AdPlacementBuilder showLoaderTillAdIsReady(bool showLoader){
            #if UNITY_ANDROID
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("showLoaderTillAdIsReady", new object[] { 
                new AndroidJavaObject("java.lang.Boolean",showLoader.ToString().ToLower()) } );
            #endif
            return this;
        }
        public AdPlacementBuilder loaderTimeOutInSeconds(long duration){
            #if UNITY_ANDROID
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("loaderTimeOutInSeconds", new object[] { 
                new AndroidJavaObject("java.lang.Long",""+duration) } );
            #endif
            return this;
        }
        public AdPlacementBuilder frequencyCapInSeconds(long duration){
            #if UNITY_ANDROID
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("frequencyCapInSeconds", new object[] { 
                new AndroidJavaObject("java.lang.Long",""+duration) } );
            #endif
            return this;
        }
        public AdPlacementBuilder onAdCompletion(AdCompletionDelegate onAdCompletion){
            #if UNITY_ANDROID
            AdPumbPluginAdCompletionCallbackProxy obj = new AdPumbPluginAdCompletionCallbackProxy();
            obj.setCallback(onAdCompletion);
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("onAdCompletion", new object[] { obj } );
            #endif
            return this;
        }
        public AdPlacementBuilder loaderUISetting(LoaderSettings loadersettings){
            #if UNITY_ANDROID
            PlacementBuilderObject = PlacementBuilderObject.Call<AndroidJavaObject>("loaderUISetting", new object[] { loadersettings.get() } );
            #endif
            return this;
        }
        public AndroidJavaObject build(){
            #if UNITY_ANDROID
            return PlacementBuilderObject.Call<AndroidJavaObject>("build");
            #endif
            return null;
        }
    }
    public class ExternalAnalytics{
        private static readonly ExternalAnalytics instance = new ExternalAnalytics();
        #if UNITY_ANDROID
        AndroidJavaClass AdPumbConfigurationClass  = new AndroidJavaClass("com.adpumb.lifecycle.AdPumbConfiguration");
        #endif
        public static ExternalAnalytics Instance {  
            get {  
                return instance;  
            }  
        }
        public ExternalAnalytics(){
            #if UNITY_ANDROID
            #endif
        }
        public void onEvent(AdPumbAnalyticsEventDelegate callBack2){
            #if UNITY_ANDROID
            AdPumbAnalyticsCallbackProxy obj = new AdPumbAnalyticsCallbackProxy();
            obj.setCallback(callBack2);
            AdPumbConfigurationClass.CallStatic<AndroidJavaObject>("getInstance").Call("setExternalAnalytics", new object[] { obj } );
            #endif
        }
    }
    public delegate void AdPumbAnalyticsEventDelegate (string Placement,float eCPM);
    class AdPumbAnalyticsCallbackProxy : AndroidJavaProxy {
        AdPumbAnalyticsEventDelegate deligate;
        public void setCallback( AdPumbAnalyticsEventDelegate callback2 ){
            deligate = callback2;
        }
        public AdPumbAnalyticsCallbackProxy() : base("com.adpumb.ads.analytics.AdPumbAnalyticsListener") { }

        public void onEvent( AndroidJavaObject impressionData) {
            if(deligate==null){
                return;
            }
            string placement =   impressionData.Call<string>("getPlacementName");
            float eCPM =  impressionData.Call<float>("getEcpm");
            deligate(placement,eCPM);
        }
    }
    //
    public class LoaderSettings{
        AndroidJavaObject LoaderSettingsObject;
        public LoaderSettings(){
            #if UNITY_ANDROID
            LoaderSettingsObject = new AndroidJavaObject("com.adpumb.ads.display.LoaderSettings") ;
            #endif
        }
        public void setLogoResID(string activityName){
            #if UNITY_ANDROID
            AndroidJavaObject unityActivity = new AndroidJavaClass(activityName).GetStatic<AndroidJavaObject>("currentActivity"); ////
            string packageName = unityActivity.Call<string>("getPackageName");
            AndroidJavaObject resource = unityActivity.Call<AndroidJavaObject>("getResources");
            int app_icon = resource.Call<int>("getIdentifier", new object[] { "app_icon", "mipmap", packageName } );
            LoaderSettingsObject.Call("setLogoResID",new object[] { new AndroidJavaObject("java.lang.Integer",""+app_icon)  } );
            #endif
        }
        public AndroidJavaObject get(){
            return LoaderSettingsObject;
        }
    }
    public delegate void AdCompletionDelegate (bool success,bool isAdFailed);
    class AdPumbPluginAdCompletionCallbackProxy : AndroidJavaProxy {
        AdCompletionDelegate deligate;
        public void setCallback( AdCompletionDelegate callback2 ){
            deligate = callback2;
        }
        public AdPumbPluginAdCompletionCallbackProxy() : base("com.adpumb.ads.display.AdCompletion") { }

        public void onAdCompletion(bool success, AndroidJavaObject placementDisplayStatus) {
            string displayStatus =   placementDisplayStatus.Call<string>("name");
            deligate(success,displayStatus=="NO_AD_AVAILABLE");
        }
    }
    public interface AdCompletion{
        public void onAdCompletion(bool success,bool isAdFailed);
    }  
}