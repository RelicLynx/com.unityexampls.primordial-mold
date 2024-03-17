using UnityEditor;

namespace QuixoticTools.PrimordialMold
{

	public class Primordial_Mold
	{

		[MenuItem( "Assets/Primordial Mold", false, 0 )]
		private static void NewFileFromTemplate()
		{
			AssetMenu_Window.OpenWindow( QuixoticTools_Utilities.GetAssetFolderPath() );
		}

		//[MenuItem( "Assets/Primordial Mold/Blank.txt", false, 8 )]
		//private static void NewTextFile()
		//{
		//	string pathToNewFile = EditorUtility.SaveFilePanel( "Create File As", QT_Utilities.GetAssetFolderPath(), "Blank.txt", "txt" ); // Open save file pannel
		//	File.WriteAllText( pathToNewFile, "" );
		//	AssetDatabase.Refresh();
		//}
	}
}