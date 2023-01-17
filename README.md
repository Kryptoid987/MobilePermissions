This is a convenience package for dealing with android and iOS permissions. It uses a custom built iOS app settings plugin, as well as a modified version of PatchedReality's Permission Helper to add more needed permissions and checks.

Any android permissions used must be added to AndroidManifest:
<uses-permission android:name="android.permission.ACTIVITY_RECOGNITION"/> //Pedometer
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"/> //Location
<uses-permission android:name="android.permission.RECORD_AUDIO"/> //Microphone
<uses-permission android:name="android.permission.CAMERA"/> //Camera
Note: Basic permissions like camera do not need to be manually added to AndroidManifest.

-iOS must have permission strings set for permission requests to work.
-Be sure to set Motion Usage to true in InputSystemPackages in settings if using new input system and require Motion Usage.

WARNING:
- to request location permission the old input system or 'both' must be enabled. There is currently no Input to enabled in the new input system that enables location services allowing us to force the request ahead of it actually being used.
- to request motion usage the new input system or 'both' must be enabled. There is no Input AFAIK in the old input system that can be used to trigger the motion usage permission request ahead of time.
*These permissions will still be requested by device on enabling something that uses location or motion providing the permission strings are set by external unity packages such as a AR SDK accessing fine location or a native plugin accessing devices pedometer directly.