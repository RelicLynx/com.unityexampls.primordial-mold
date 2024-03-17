using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QuixoticTools.PrimordialMold
{

	[CreateAssetMenu( fileName = "TemplateHolder_SO", menuName = "SO/TemplateHolder_SO" )] // menu Option to Re-Create TemplateHolder_SO
	public class TemplateHolder_SO_Script : ScriptableObject
	{

		// ================================================================ «» ================================================================ //

		#region | Type Definitions |

		[System.Serializable]
		public struct Filetype
		{
			public string Caption;
			public string Extension;

			public Filetype(
				string caption
				, string extension )
			{
				Caption = caption;
				Extension = extension;
			}
		}

		[System.Serializable]
		public struct KeywordReplace
		{
			public string Caption;
			public string TargetText;
			[TextAreaAttribute( 1, 8 )] public string ReplacementText;

			public KeywordReplace(
				string caption,
				string targetText,
				string replacementText )
			{
				Caption = caption;
				TargetText = targetText;
				ReplacementText = replacementText;
			}
		}

		[System.Serializable]
		public struct TemplateHolder
		{
			public string Caption;
			public string FileName;
			[TextAreaAttribute( 1, 8 )] public string RAW_Text;
			public Filetype FileType;
			[Space( 0.5f )]
			[Header( "Options" )]
			public bool RunDefaultScriptParsing;
			public bool RunExtendedScriptParsing;
			public bool ForceSpacesToTabs;
			public List<KeywordReplace> KeywordReplace;

			public TemplateHolder(
				string caption,
				string fileName,
				string raw_text,
				Filetype fileType,
				bool runDefaultScriptParsing,
				bool runExtendedScriptParsing,
				bool forceSpacesToTabs,
				List<KeywordReplace> keywordReplace )
			{
				Caption = caption;
				FileName = fileName;
				RAW_Text = raw_text;
				FileType = fileType;
				RunDefaultScriptParsing = runDefaultScriptParsing;
				RunExtendedScriptParsing = runExtendedScriptParsing;
				ForceSpacesToTabs = forceSpacesToTabs;
				KeywordReplace = keywordReplace;
			}
		}

		#endregion | Type Definitions |

		// ================================================================ «» ================================================================ //

		#region | Fields |

		[ContextMenuItem( "Add New Blank Template", "AddBlankTemplate" )]
		public List<TemplateHolder> Templates;

		#endregion | Fields |

		// ================================================================ «» ================================================================ //

		#region | Events |

		[OnOpenAsset()]
		public static bool OpenEditor( int instanceID, int line )
		{
			TemplateHolder_SO_Script obj = EditorUtility.InstanceIDToObject( instanceID ) as TemplateHolder_SO_Script;
			if ( obj != null )
			{
				TemplateEditor_Window.OpenWindow( obj );
				return true;
			}
			return false;
		}

		#endregion | Events |

		// ================================================================ «» ================================================================ //

		#region | Functions |

		public void AddBlankTemplate()
		{
			Templates ??= new();

			Templates.Add(
				new(
					"New Template",
					"NewFile",
					"",
					new( "C#", ".cs" ),
					true,
					true,
					true,
					new() { new( "Target", "#NewTarget#", "Caption" ) }
				)
			);
		}

		#endregion | Functions |

		// ================================================================ «» ================================================================ //
	}
}
