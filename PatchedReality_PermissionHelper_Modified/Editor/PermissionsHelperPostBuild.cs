namespace MobilePermissions.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;
    using System.IO;
	
	public class PermissionsHelperPostBuild {

        //use this list of key value pairs to add plist entries needed for your permissions that are not already 
        //offered in unity build settings.
		private static List<KeyValuePair<string,string>> UsageStringsToAdd = new List<KeyValuePair<string,string>>()
        {
          //  new KeyValuePair<string,string>("NSSpeechRecognitionUsageDescription","Speech recognition so we can understand what you say."),
        };


        //use this list to add frameworks needed for ios that are not already available from checklist in unity settings.
        //many are already there.
        private static List<string> XCodeFrameworksToAdd = new List<string>()
        {
          //  "Speech.framework"
        };
	
		private static string nameOfPlist = "Info.plist";
		[PostProcessBuild]
        public static void PermissionsPostProcess(BuildTarget buildTarget, string pathToBuiltProject) {
           ChangeXcodePlist(buildTarget,pathToBuiltProject);
           AddFrameworks(buildTarget,pathToBuiltProject);
        }

        protected static void AddFrameworks(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS && XCodeFrameworksToAdd.Count > 0)
			{
				// Get target for Xcode project
				string projPath = PBXProject.GetPBXProjectPath(path);

				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

                string targetName = proj.GetUnityFrameworkTargetGuid(); //PBXProject.GetUnityTargetName();
				string projectTarget = proj.TargetGuidByName(targetName);

				// Add dependencies
//				Debug.Log("KKSpeechRecognizer Unity: Adding Speech Framework");
                foreach(string entry in XCodeFrameworksToAdd)
                {
                    proj.AddFrameworkToProject(projectTarget, entry, true);
                }
				

				File.WriteAllText(projPath, proj.WriteToString());

			}
        }
		protected static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

			if (buildTarget == BuildTarget.iOS && UsageStringsToAdd.Count > 0) {

				// Get plist
				string plistPath = pathToBuiltProject + "/" + nameOfPlist;
				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));

				// Get root
				PlistElementDict rootDict = plist.root;
                foreach(KeyValuePair<string,string> entry in UsageStringsToAdd)
                {
                    rootDict.SetString(entry.Key,entry.Value);
                }

				// Write to file
				File.WriteAllText(plistPath, plist.WriteToString());
			}
		}
	}
}


