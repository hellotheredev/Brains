using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

using Brains;

namespace Brains.Core
{
	public class BrainUtils
	{

		[MenuItem("Brains/Create Brain")]
		[MenuItem("Assets/Create/Brain")]
		public static void CreateBrain()
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);

			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName(AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}

			Brain brain = ScriptableObject.CreateInstance<Brain>();

			AssetDatabase.CreateAsset(brain, string.Format("{0}/Fresh Brain.asset", path));
	 
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = brain;
		}

	}

}
