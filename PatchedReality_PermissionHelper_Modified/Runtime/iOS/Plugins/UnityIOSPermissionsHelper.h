//
//  UnityPermissionsObjC.h
//  UnityPermissionsObjC
//
//  Created by Joshua Welber on 5/10/19.
//  Copyright © 2019 Joshua Welber. All rights reserved.
//
// Modified by Scott Vander Pryt on 2022/07/13

#ifndef UnityIOSPermissionsHelper_h
#define UnityIOSPermissionsHelper_h


#import <Foundation/NSException.h>
#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>
//#import <Speech/Speech.h>
#import <CoreLocation/CoreLocation.h>
#import <CoreMotion/CoreMotion.h>

@interface UnityIOSPermissionsHelper : NSObject {}

- (void) requestPermission:(NSString *)NSGameObject withSuccessCallback:(NSString *)NSSuccessCallback withFailureCallback:(NSString *)NSFailureCallback withPermissionType:(int)permissionType;
- (int) getPermissionStatus:(int)permissionType;

//- (void) verifyCameraPermission:(NSString *)gameObject withCallback:(NSString *)callback;
//
//- (void) doPermissionsCallback:(NSString *)gameObject withCallback:(NSString *)callback withPermission:(int)permissionType withResult:(NSString *)result;
@end
#endif
