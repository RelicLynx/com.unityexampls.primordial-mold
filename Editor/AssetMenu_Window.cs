using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;
using static QuixoticTools.PrimordialMold.TemplateHolder_SO_Script;

namespace QuixoticTools.PrimordialMold
{
	public class AssetMenu_Window : EditorWindow
	{
		// ================================================================ «» ================================================================ //

		#region | Fields |

		private static AssetMenu_Window instance;
		public static AssetMenu_Window Instance
		{
			get
			{
				if ( instance == null )
				{
					instance = new AssetMenu_Window();
				}
				return instance;
			}
		}
		private void OnDestroy()
		{
			instance = null;
		}

		private string _path = "\\Assets\\";
		//private float scrollPos = 0.5f;

		private TemplateHolder_SO_Script _SO_Ref;
		private SerializedObject _serialized_SO;
		private SerializedProperty _prop;
		private int _cur = 0;

		private Vector2 scrollPosition;

		#endregion

		// ================================================================ «» ================================================================ //

		#region | Events & Calls |

		//[MenuItem( "Assets/Create/From Primordial Mold", false, 0 )]
		//private static void NewFileFromTemplate()
		//{
		//	OpenWindow( GetAssetFolderPath() );
		//}

		//[MenuItem( "Assets/Create/Blank.txt", false, 1 )]
		//private static void NewTextFile()
		//{
		//	string pathToNewFile = EditorUtility.SaveFilePanel( "Create File As", GetAssetFolderPath(), "Blank.txt", "txt" ); // Open save file pannel
		//	File.WriteAllText( pathToNewFile, "" );
		//	AssetDatabase.Refresh();
		//}

		public static void OpenWindow( string _assetFolderPath )
		{
			instance ??= GetWindow<AssetMenu_Window>();
			instance._path = _assetFolderPath;

			Debug.Log( "what?" );

			string[] guids = AssetDatabase.FindAssets( "TemplateHolder_SO" ); // This searches assets by name and other criteria, not just name			
			string path = AssetDatabase.GUIDToAssetPath( guids[ 0 ] );
			TemplateHolder_SO_Script obj_SO = AssetDatabase.LoadAssetAtPath<TemplateHolder_SO_Script>( path );
			instance._SO_Ref = obj_SO;
			instance._serialized_SO = new SerializedObject( obj_SO );
			instance._prop = instance._serialized_SO.FindProperty( "templates" );

			Debug.Log( "Clicks" );

			//instance.serialized_SO = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("")[0]))
			instance.Focus();
		}

		private void OnEnable()
			=> _path ??= GetAssetFolderPath();

		#endregion

		// ================================================================ «» ================================================================ //

		#region | OnGUI |

		private void OnGUI()
		{
			bool canBeClosed = false;
			Event e = Event.current;
			if ( e.type == EventType.KeyDown && new KeyCode[] { KeyCode.Escape, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow }.Contains( e.keyCode ) )
			{
				switch ( e.keyCode )
				{
					case KeyCode.Escape:
						instance.Close();
						break;

					case
						KeyCode.Return or KeyCode.KeypadEnter:
						instance.hideFlags = HideFlags.HideAndDontSave;
						CreateNewFile( _cur );
						canBeClosed = true;
						break;

					case KeyCode.W or KeyCode.UpArrow:
						if ( _cur > 0 ) _cur--;
						break;

					case KeyCode.S or KeyCode.DownArrow:
						if ( _SO_Ref.Templates.Count - 1 > _cur ) _cur++;
						break;
				}
				Repaint();
			}

			if ( canBeClosed )
			{
				instance.Close();
			}

			using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
			{
				ListView listView = new ListView();

				using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) )
				{
					using ( new GUILayout.VerticalScope() )
					{
						scrollPosition = GUILayout.BeginScrollView( scrollPosition );

						for ( int i = 0; i < _SO_Ref.Templates.Count; i++ )
						{

							if ( i == _cur ) GUI.backgroundColor = Color.green;
							if ( GUILayout.Button( _SO_Ref.Templates[ i ].Caption ) )
							{
								CreateNewFile( i );
								instance.Close();
							}
							if ( i == _cur ) GUI.backgroundColor = default;
						}

						GUILayout.EndScrollView();
					}
				}

				using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) )
				{
					GUILayout.Space( 8 );

					if ( GUILayout.Button( "Ok" ) )
					{
						CreateNewFile( _cur );
						instance.Close();
					}
					if ( GUILayout.Button( "Cancel" ) )
					{
						instance.Close();
					}
				}
			}
		}

		private void CreateNewFile( int cur )
		{
			TemplateHolder curTemplate = instance._SO_Ref.Templates[ cur ];

			string pathToNewFile = EditorUtility.SaveFilePanel(
				curTemplate.Caption, GetAssetFolderPath()
				, $"{curTemplate.FileName}{curTemplate.FileType.Extension}"
				, curTemplate.FileType.Extension.Replace( ".", "" )
			); // Open save file pannel

			if ( string.IsNullOrEmpty( pathToNewFile ) ) return; // early exit

			string text = curTemplate.RAW_Text;
			string destFileName = Path.GetFileNameWithoutExtension( pathToNewFile );
			string destFileNameNoSpaces = destFileName.Replace( " ", "" );

			if ( curTemplate.RunDefaultScriptParsing )
			{
				text = text.Replace( "#NOTRIM#", "" );
				text = text.Replace( "#NAME#", destFileName );
				text = text.Replace( "#SCRIPTNAME#", destFileNameNoSpaces );

				string rootNamespace = null;
				if ( Path.GetExtension( pathToNewFile ) == ".cs" )
				{
					rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath( pathToNewFile );
				}

				string rootStart = "#ROOTNAMESPACEBEGIN#";
				string rootEnd = "#ROOTNAMESPACEEND#";
				if ( text.Contains( rootStart ) || text.Contains( rootEnd ) )
				{


					if ( string.IsNullOrEmpty( rootNamespace ) )
					{
						text = Regex.Replace( text, "((\\r\\n)|\\n)[ \\t]*" + rootStart + "[ \\t]*", string.Empty );
						text = Regex.Replace( text, "((\\r\\n)|\\n)[ \\t]*" + rootEnd + "[ \\t]*", string.Empty );

					}
					else
					{

						string separator = ( text.Contains( "\r\n" ) ? "\r\n" : "\n" );
						List<string> list = new List<string>( text.Split( new string[ 3 ] { "\r\n", "\r", "\n" }, StringSplitOptions.None ) );
						int i;
						for ( i = 0; i < list.Count && !list[ i ].Contains( rootStart ); i++ ) { }

						string text3 = list[ i ];
						string text4 = text3.Substring( 0, text3.IndexOf( "#" ) );
						list[ i ] = "namespace " + rootNamespace;
						list.Insert( i + 1, "{" );
						for ( i += 2; i < list.Count; i++ )
						{
							string text5 = list[ i ];
							if ( !string.IsNullOrEmpty( text5 ) && text5.Trim().Length != 0 )
							{
								if ( text5.Contains( rootEnd ) )
								{
									list[ i ] = "}";
									break;
								}

								list[ i ] = text4 + text5;
							}
						}

						text = string.Join( separator, list.ToArray() );
					}
				}

				if ( curTemplate.RunExtendedScriptParsing && curTemplate.KeywordReplace.Count > 0 )
				{
					foreach ( KeywordReplace replace in curTemplate.KeywordReplace )
					{
						text = text.Replace( replace.TargetText, replace.ReplacementText );
					}
				}

				if ( curTemplate.ForceSpacesToTabs )
				{
					text = text.Replace( "    ", "	" );
				}

				if ( char.IsUpper( destFileNameNoSpaces, 0 ) )
				{
					destFileNameNoSpaces = char.ToLower( destFileNameNoSpaces[ 0 ] ) + destFileNameNoSpaces.Substring( 1 );
					text = text.Replace( "#SCRIPTNAME_LOWER#", destFileNameNoSpaces );
				}

				destFileNameNoSpaces = "my" + char.ToUpper( destFileNameNoSpaces[ 0 ] ) + destFileNameNoSpaces.Substring( 1 );
				text = text.Replace( "#SCRIPTNAME_LOWER#", destFileNameNoSpaces );
			}

			File.WriteAllText( pathToNewFile, text );
			AssetDatabase.Refresh();

			UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( pathToNewFile[ pathToNewFile.IndexOf( "Assets" ).. ] );
			Selection.activeObject = asset;
		}


		#endregion

		// ================================================================ «» ================================================================ //

		#region | Functions |

		static string GetAssetFolderPath()
		{
			string path = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ 0 ] );
			if ( path.Contains( "." ) )
			{
				int index = path.LastIndexOf( "/" );
				path = path.Substring( 0, index );
			}
			return path;
		}

		#endregion | Functions |

		// ================================================================ «» ================================================================ //

		#region | old S'crap |
		//private static void OpenMyWindow() {			

		//		  // Find MyScriptableObject in Packages folder
		//		  string[] guids = AssetDatabase.FindAssets( "TemplateHolder_SO" /*,new string[] { "Packages" } */);
		//		  if( guids.Length == 0 ) {
		//			  Debug.LogError( "Could not find TemplateHolder_SO in Packages folder." );
		//			  return;
		//		  }
		//		  string assetPath = AssetDatabase.GUIDToAssetPath( guids[0] );

		//	//TemplateHolder_SOD myObject = AssetDatabase.LoadAssetAtPath<TemplateHolder_SOD>( assetPath );

		//	// Open Editor window and attach MyScriptableObject reference
		//	MyEditorWindow window = EditorWindow.GetWindow<MyEditorWindow>();
		//		  //window.myObject = myObject;
		//		  //window.folderLocation = AssetDatabase.GetAssetPath( myObject ).Replace( assetPath ,"" );

		//		  // Focus on Editor window

		//		  window.Focus();
		//	  }

		#endregion | old S'crap |
	}

	#region | older S'crap |
	//public class MyEditorWindow : EditorWindow {
	//	/*
	//		public FileTemplate_SOD myObject;
	//		public string folderLocation;

	//		private SerializedObject serializedObject;

	//		private void OnEnable() {
	//			// Create SerializedObject from MyScriptableObject reference
	//			serializedObject = new SerializedObject( myObject );
	//		}

	//		private void OnGUI() {
	//			// Draw MyScriptableObject parameters
	//			EditorGUILayout.LabelField( "MyScriptableObject Parameters" ,EditorStyles.boldLabel );

	//			// Begin SerializedObject update
	//			serializedObject.Update();

	//			// Iterate over SerializedProperties and draw them dynamically
	//			SerializedProperty iterator = serializedObject.GetIterator();
	//			bool enterChildren = true;
	//			while( iterator.NextVisible( enterChildren ) ) {
	//				// Skip some built-in properties
	//				if( iterator.name == "m_Script" ) continue;

	//				// Draw property
	//				EditorGUILayout.PropertyField( iterator ,true );

	//				enterChildren = false;
	//			}

	//			// End SerializedObject update
	//			serializedObject.ApplyModifiedProperties();

	//			// Draw folder location field
	//			EditorGUILayout.LabelField( "Folder Location" ,EditorStyles.boldLabel );
	//			EditorGUI.indentLevel++;
	//			EditorGUILayout.TextField( folderLocation );
	//			EditorGUI.indentLevel--;
	//		}
	//	}


	//	public class MyEditorWindow : EditorWindow {
	//		public FileTemplate_SOD myObject;
	//		public string folderLocation;

	//		private void OnGUI() {
	//			// Draw MyScriptableObject parameters
	//			EditorGUILayout.LabelField( "MyScriptableObject Parameters" ,EditorStyles.boldLabel );
	//			EditorGUI.indentLevel++;
	//			//myObject.RAW_Text = EditorGUILayout.TextField( "Parameter 1" ,myObject.RAW_Text );
	//			//myObject.parameter1 = EditorGUILayout.FloatField( "Parameter 1" ,myObject.parameter1 );
	//			//myObject.parameter2 = EditorGUILayout.Toggle( "Parameter 2" ,myObject.parameter2 );
	//			EditorGUI.indentLevel--;

	//			// Draw folder location field
	//			EditorGUILayout.LabelField( "Folder Location" ,EditorStyles.boldLabel );
	//			EditorGUI.indentLevel++;
	//			EditorGUILayout.TextField( folderLocation );
	//			EditorGUI.indentLevel--;
	//		}
	//  */
	//}

	//if ( Input.GetKey(KeyCode.A) ) { _cur++; Debug.Log("+"); }
	//if ( Input.GetKey(KeyCode.S) ) { _cur--; Debug.Log("-"); }
	//_cur = (int)Mathf.Clamp(_cur, 0, _SO_Ref.Templates.Count - 1);

	//if ( Input.GetKey(KeyCode.Return) )
	//{
	//	Debug.Log("Bang Bang");
	//}

	//// Create a two-pane view with the left pane being fixed with
	//var splitView = new TwoPaneSplitView( 0 ,250 ,TwoPaneSplitViewOrientation.Horizontal );

	//// Add the panel to the visual tree by adding it as a child to the root element
	//rootVisualElement.Add( splitView );

	//// A TwoPaneSplitView always needs exactly two child elements
	//var leftPane = new ListView();
	//splitView.Add( leftPane );

	//var rightPane = new VisualElement();
	//splitView.Add( rightPane );


	#endregion | older S'crap |
}
