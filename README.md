# AdPumb unity integration guide

## Prerequisite ##

You need to register with us before starting the integration. You can drop a mail to sales@adpumb.com

## Steps ##

  

1) Enable custom Manifest and gradle. **Edit > Project Settings > Player** then click on **Android > publishing settings** to add admob publisher id and gradle library dependences 

  

![alt text](https://github.com/ad-pump/unity-demo/blob/main/screen-shot/project-settings.png?raw=true)

  

  

  

* Add your admob publisher id to the android-manifest.xml ( located in **Assets > Plugins > Android** ) of your app: Adpump doesn't use your admob adunits, however adpump uses the underlying admob APIs for which publisher id is mandatory. 


* Add library dependency: it is needed to add the repository details and dependency  to your gradle script.

  

Add repository to you baseProjectTemplate.gradle file ( located in **Assets > Plugins > Android** )  of your app

```gradle

repositories{
    maven {
        url 'https://maven.adpumb.com/repository/adpumb/'
    }
}

```

Add dependency to you mainTemplate.gradle file ( located in **Assets > Plugins > Android** )  of your app

```gradle


dependencies {

implementation 'com.adpumb:bidmachine:2.3.6'

*********************

```

Also needed to activate AndroidX and enable Jetifier in **gradleTemplate.properties** file and enable **multiDex** 
```gradle
android.useAndroidX=true
android.enableJetifier=true
```
in **launcherTemplate.gradle**
```gradle
    defaultConfig {
        .....
        multiDexEnabled true
        .....
    }
```

2) Adding config key to AndroidManifest

   On AndroidManifest.xml add meta data with name 'com.adpumb.config.key' values given by us.

```
   <meta-data 
   android:name="com.adpumb.config.key" 
   android:value="adpumb-key-goes-here" />
    
```


3) Create a Folder **AdPumb** in **Assets > Plugins** then copy contents from With [**THIS FILE**](https://raw.githubusercontent.com/ad-pump/unity-demo/main/Assets/Plugins/AdPumb/AdPumbPlugin.cs)  to a C# Script file **AdPumbPlugin.cs** .

4) Create placement: Adpump is designed on the concept of placement rather than adunit. A placement is a predefined action sequence which ends up in showing an Ad. 

```c#
AdPlacementBuilder placementObject1 = AdPlacementBuilder.Interstitial()
        .name("ad_placement_name")    
        .showLoaderTillAdIsReady(true)
        .loaderTimeOutInSeconds(5)
        .frequencyCapInSeconds(15);
DisplayManager.Instance.showAd(placementObject1);
```

In this example if the ad is not ready within 5 seconds then the loader will be removed. However if the ad is already loaded or it got loaded while the loader is shown, then ad loader will be hidden and ad will be shown to user.

5) For a particular placement you need to create only one placement object, which can be used to show multiple ads.

For example:

```c#
AdPlacementBuilder placementObject2 = AdPlacementBuilder.Interstitial()
        .name("2nd_ad_placement_name");
```

```c#
// somewhere in the class calling an ad
DisplayManager.Instance.showAd(placementObject2);
```
```c#
// somewhere in the class calling another ad
DisplayManager.Instance.showAd(placementObject2);
```

6) Callbacks: You can register callbacks to the placement

```c#
AdPlacementBuilder placementObject3 = AdPlacementBuilder.Interstitial()
        .name("3rd_ad_placement_name")    
        .showLoaderTillAdIsReady(true)
        .loaderTimeOutInSeconds(5)
        .frequencyCapInSeconds(15)
        .onAdCompletion( this.onAdCompletion );
DisplayManager.Instance.showAd(placementObject3);

......... 

public void onAdCompletion(bool success,bool isAdFailed){ 
    if(isAdFailed){
    // ad failed to load
    } else if(isSuccess){ 
    // watched ad
    } else {
    // Didn't watched the ad
    }
}
```

7) Customising loader : You can customize the loader using the loader settings for each placement

```c#
LoaderSettings loader = new LoaderSettings();
loader.setLogoResID(); // this will set loader logo to your app logo
AdPlacementBuilder placementObject4 = AdPlacementBuilder.Interstitial()
            .name("4th_ad_placement_name")
            .showLoaderTillAdIsReady(true)
            .loaderTimeOutInSeconds(5)
            .frequencyCapInSeconds(15)
            .loaderUISetting(loader)
            .onAdCompletion( this.onAdCompletion ) ;
DisplayManager.Instance.showAd(placementObject4);

```

