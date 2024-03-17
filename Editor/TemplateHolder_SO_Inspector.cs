using UnityEditor;
using UnityEngine;

namespace QuixoticTools.PrimordialMold
{

	[CustomEditor( typeof( TemplateHolder_SO_Script ) )]
	public class TemplateHolder_SO_Inspector : Editor
	{
		public override void OnInspectorGUI()
		{
			using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
			{
				if ( GUILayout.Button( "Open Template Editor" ) )
				{
					TemplateEditor_Window.OpenWindow( (TemplateHolder_SO_Script)target );
				}

				using ( new GUILayout.HorizontalScope() )
				{
					if ( GUILayout.Button( "Add Blank" ) )
					{
						( (TemplateHolder_SO_Script)target ).AddBlankTemplate();
					}

					if ( GUILayout.Button( "Save to JSON" ) )
					{
						QuixoticTools_Utilities.SaveSOToJSON( (TemplateHolder_SO_Script)target );
					}

					if ( GUILayout.Button( "Load from JSON" ) )
					{
						QuixoticTools_Utilities.LoadSOFromJSON( (TemplateHolder_SO_Script)target );
					}
				}
			}

			// Draw Standard Inspector below
			base.OnInspectorGUI();
		}
	}
}