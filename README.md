This is a convenience package for dealing with android and iOS permissions. It uses a custom built iOS app settings plugin, as well as a modified version of PatchedReality's Permission Helper to add more needed permissions and checks.

Any android permissions used must be added to AndroidManifest:
<uses-permission android:name="android.permission.ACTIVITY_RECOGNITION"/> //Pedometer
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"/> //Location
<uses-permission android:name="android.permission.RECORD_AUDIO"/> //Microphone
<uses-permission android:name="android.permission.CAMERA"/> //Camera
Note: Basic permissions like camera do not need to be manually added to AndroidManifest.

iOS must have permission strings set for permission requests to work.