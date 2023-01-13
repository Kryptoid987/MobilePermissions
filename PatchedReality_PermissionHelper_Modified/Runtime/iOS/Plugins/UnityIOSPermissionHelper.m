//
//  UnityIOSPermissionsHelper.m
//  UnityIOSPermissionsHelper
//
//  Created by Joshua Welber on 5/10/19.
//  Copyright © 2019 Joshua Welber. All rights reserved.
//
//  Updated by Scott Vander Pryt on 2022/07/13	

#import "UnityIOSPermissionsHelper.h"
//these consts used to simplify interface between unity and obj-c.
const int PRCameraPermissions=0;
const int PRMicrophonePermissions=1;
const int PRLocationWhileUsingPermissions=2;
const int PRSpeechRecognitionPermissions=3;
const int PRMotionUsagePermissions=4;

//these consts will be returned from calls to HasPermission - indicating whether the permission
const int PRPermissionStatusUnknown=0; //never asked.,
const int PRPermissionStatusAuthorized=1; //granted already
const int PRPermissionStatusDenied=2; //denied once, user will need to manual override in settings
const int PRPermissionStatusRestricted=3; //user doesn't have authority, they will need to ask an admin.
const int PRPermissionStatusUnknownPermission=255; //you asked about a device/permission we don't know about.

@implementation UnityIOSPermissionsHelper
- (void) requestPermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSuccessCallback withFailureCallback:(NSString *)NSFailureCallback withPermissionType:(int)permissionType
{
    switch(permissionType)
    {
        case PRCameraPermissions:
            [self requestCameraPermission:NSGameObject withSuccessCallback:NSSuccessCallback withFailureCallback:NSFailureCallback];
            break;
        case PRMicrophonePermissions:
            [self requestMicrophonePermission:NSGameObject withSuccessCallback:NSSuccessCallback withFailureCallback:NSFailureCallback];
            break;
        case PRLocationWhileUsingPermissions:
            [self requestLocationWhileUsingPermission:NSGameObject withSuccessCallback:NSSuccessCallback withFailureCallback:NSFailureCallback];
            break;
        case PRSpeechRecognitionPermissions:
            [self requestSpeechRecognitionPermission:NSGameObject withSuccessCallback:NSSuccessCallback withFailureCallback:NSFailureCallback];
            break;
    }
}

- (int) getPermissionStatus:(int)permissionType
{
    switch(permissionType)
    {
        case PRCameraPermissions:
            return [self GetCameraPermissions];
        case PRMicrophonePermissions:
            return [self GetMicrophonePermissions];
        case PRLocationWhileUsingPermissions:
            return [self GetLocationWhileUsingPermissions];
        case PRSpeechRecognitionPermissions:
            return [self GetSpeechRecognitionPermissions];
		case PRMotionUsagePermissions:
			return [self GetMotionUsagePermissions];
    }
    
    return PRPermissionStatusUnknownPermission;
}

- (void) requestCameraPermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSucessCallback withFailureCallback:(NSString *)NSFailureCallback
{
    int permissionStatus = [self GetCameraPermissions];
    if(permissionStatus==PRPermissionStatusRestricted || permissionStatus==PRPermissionStatusDenied)
    {
        //should not ask again, already restricted or denied. call the failure callback.
        [ self doPermissionsCallback:NSGameObject withCallback:NSFailureCallback withPermission:PRCameraPermissions ];
        return;
    }
    else if(permissionStatus==PRPermissionStatusAuthorized)
    {
        //already have permission, call success callback.
        [ self doPermissionsCallback:NSGameObject withCallback:NSSucessCallback withPermission:PRCameraPermissions ];
        return;
    }
    
    [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
        
        if (granted == YES) {
            [ self doPermissionsCallback:NSGameObject withCallback:NSSucessCallback withPermission:PRCameraPermissions ];
        }
        else {
             [ self doPermissionsCallback:NSGameObject withCallback:NSFailureCallback withPermission:PRCameraPermissions ];
        }
    }];
}

-(int) GetCameraPermissions
{
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    switch (authStatus) {
        case AVAuthorizationStatusAuthorized:
            return PRPermissionStatusAuthorized;
        case AVAuthorizationStatusNotDetermined:
            return PRPermissionStatusUnknown;
        case AVAuthorizationStatusDenied:
            return PRPermissionStatusDenied;
        case AVAuthorizationStatusRestricted:
            return PRPermissionStatusRestricted;
    }
    
    return PRPermissionStatusUnknown;
}

-(int) GetMotionUsagePermissions
{
    CMAuthorizationStatus status = [CMMotionActivityManager authorizationStatus];
    switch(status)
    {
        case CMAuthorizationStatusDenied:
            return PRPermissionStatusDenied;
        case CMAuthorizationStatusRestricted:
            return PRPermissionStatusRestricted;
        case CMAuthorizationStatusAuthorized:
            return PRPermissionStatusAuthorized;
        case CMAuthorizationStatusNotDetermined:
            return PRPermissionStatusUnknown;
    }
    
    return PRPermissionStatusUnknown;
}

- (void) requestMicrophonePermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSucessCallback withFailureCallback:(NSString *)NSFailureCallback
{
    int permissionStatus = [self GetMicrophonePermissions];
    if(permissionStatus==PRPermissionStatusRestricted || permissionStatus==PRPermissionStatusDenied)
        {
        //should not ask again, already restricted or denied. call the failure callback.
        [ self doPermissionsCallback:NSGameObject withCallback:NSFailureCallback withPermission:PRMicrophonePermissions ];
        return;
        }
    else if(permissionStatus==PRPermissionStatusAuthorized)
        {
        //already have permission, call success callback.
        [ self doPermissionsCallback:NSGameObject withCallback:NSSucessCallback withPermission:PRMicrophonePermissions ];
        return;
        }
    
    [AVCaptureDevice requestAccessForMediaType:AVMediaTypeAudio completionHandler:^(BOOL granted) {
        
        if (granted == YES) {
            [ self doPermissionsCallback:NSGameObject withCallback:NSSucessCallback withPermission:PRMicrophonePermissions ];
        }
        else {
            [ self doPermissionsCallback:NSGameObject withCallback:NSFailureCallback withPermission:PRMicrophonePermissions ];
        }
    }];
}

-(int) GetMicrophonePermissions
{
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeAudio];
    switch (authStatus) {
        case AVAuthorizationStatusAuthorized:
            return PRPermissionStatusAuthorized;
        case AVAuthorizationStatusNotDetermined:
            return PRPermissionStatusUnknown;
        case AVAuthorizationStatusDenied:
            return PRPermissionStatusDenied;
        case AVAuthorizationStatusRestricted:
            return PRPermissionStatusRestricted;
    }
    
    return PRPermissionStatusUnknown;
}



- (void) requestLocationWhileUsingPermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSucessCallback withFailureCallback:(NSString *)NSFailureCallback
{
    //We always callback with success, immmediately. On the C# side, requesting permission is
    //actually handled by working with unity locations. However, we do expose methods to get
    //location permissions, since that is easier to work with.
    [ self doPermissionsCallback:NSGameObject withCallback:NSSucessCallback withPermission:PRLocationWhileUsingPermissions ];
}

-(int) GetLocationWhileUsingPermissions
{
    CLAuthorizationStatus status = [CLLocationManager authorizationStatus];
    switch(status)
    {
        case kCLAuthorizationStatusDenied:
            return PRPermissionStatusDenied;
        case kCLAuthorizationStatusRestricted:
            return PRPermissionStatusRestricted;
        case kCLAuthorizationStatusAuthorizedWhenInUse:
        case kCLAuthorizationStatusAuthorizedAlways:
            return PRPermissionStatusAuthorized;
        case kCLAuthorizationStatusNotDetermined:
            return PRPermissionStatusUnknown;
    }
    
    return PRPermissionStatusUnknown;
}

- (void) requestSpeechRecognitionPermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSucessCallback withFailureCallback:(NSString *)NSFailureCallback
{
    //fail - we don't have support for speech. Use with_reco branch if you want speech
    [self doPermissionsCallback:NSGameObject withCallback:NSFailureCallback withPermission:PRSpeechRecognitionPermissions ];
    
    
}

-(int) GetSpeechRecognitionPermissions
{
    //we don't support - cause we don't include it in this branch. Use speech-reco branch if you
    //want speech.
    return PRPermissionStatusUnknown;
}


- (void) doPermissionsCallback:(NSString *)gameObject withCallback:(NSString *)callback withPermission:(int)permissionType
{
    NSString * permissionAsString = [[NSNumber numberWithInt:permissionType] stringValue];
    //tell unity about it.
    UnitySendMessage(([gameObject cStringUsingEncoding:NSUTF8StringEncoding]),
                     ([callback cStringUsingEncoding:NSUTF8StringEncoding]),
                     ([permissionAsString cStringUsingEncoding:NSUTF8StringEncoding]));
}
@end


//useful in case you asked for a setting status and got back a denied status
extern void _openSettings () {
    NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
    [[UIApplication sharedApplication] openURL: url];
}

//called from C# to trigger request for one of the permissions we know how to ask for.
//note: you should call getPermissionStatus first before calling this.
extern void _requestPermission(int permissionType, const char* gameObject, const char* successCallback, const char* failureCallback)
{
    NSString *NSGameObject = [[NSString alloc] initWithUTF8String:gameObject];
    NSString *NSSuccessCallback = [[NSString alloc] initWithUTF8String:successCallback];
    NSString *NSFailureCallback = [[NSString alloc] initWithUTF8String:failureCallback];
    UnityIOSPermissionsHelper* helper = [[UnityIOSPermissionsHelper alloc] init];
    [helper requestPermission:NSGameObject withSuccessCallback:NSSuccessCallback withFailureCallback:NSFailureCallback withPermissionType:permissionType];
}

//returns an int flag indicating whether permission has been granted, denied, restricted or not-yet-asked
extern int _getPermissionStatus(int permissionType)
{
    UnityIOSPermissionsHelper* helper = [[UnityIOSPermissionsHelper alloc] init];
    int result = [helper getPermissionStatus:permissionType];
    return result;
}


