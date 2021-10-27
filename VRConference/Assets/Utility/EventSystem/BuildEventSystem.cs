using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Utility
{
    public class MenuItems
    {

        static string path  = "Assets/Utility/EventSystem/";
        
        [MenuItem("Tools/Build Event System")]
        private static void NewMenuOption()
        {
            string content = File.ReadAllText(path + "PublicEventMaster.cs");
            createConvtert("PublicEvent", content);
            
            /*
            content = File.ReadAllText(path + "PublicMaster.cs");
            createConvtert("Public", content);
            */
        }

        private static void createConvtert(string fileprefix, string content)
        {
            string[] types = new []
            {
                "bool",
                "int",
                "int2",
                "int3",
                "int4",
                "float",
                "float2",
                "float3",
                "float4",
                "string",
                "int[]",
                "string[]"
            };
            
            foreach (string type in types)
            {
                string typeForName = FirstCharToUpper(type).Replace("[]", "Array");

                string newContent = content.Replace("Master", typeForName);
                newContent = newContent.Replace("object", type);
                File.WriteAllText(path +"/Generated/"+ fileprefix + typeForName + ".cs", newContent);
            }
        }
        
        public static string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        
    }
}
