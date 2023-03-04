#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorTools
{
    /// <summary>
    /// "UvMapCreationWizard" class was developed by Umutcan Bagci, it aims to create single and multiple uv maps on unity quickly and easily.
    /// </summary>
    public class UvMapCreationWizard : ScriptableWizard
    {
        public List<MeshFilter> targetUVs;

        #region Unity Event Funcitons

        /// <summary>
        /// When the wizard opens, it finds the MeshFilter components in the selection.
        /// </summary>
        private void Awake() => targetUVs = GetSelectedMeshFilters();
        
        /// <summary>
        /// When the selection changes in Unity, it finds the MeshFilter components in the selection.
        /// </summary>
        private void OnSelectionChange() => targetUVs = GetSelectedMeshFilters();

        /// <summary>
        /// Creates uv maps for the meshes found when the "Create UV Map" button is pressed.
        /// </summary>
        private void OnWizardCreate()
        {
            //Null check for targetUVs.
            if (targetUVs == null || targetUVs.Count == 0)
            {
                if (EditorUtility.DisplayDialog("Failed ✗", "The operation failed because no mesh selection"
                +"was found.", "Select & Try Again", "Cancel"))
                {
                    UVMapCreationWizard();
                }
                return;
            }

            // Try generate UV map for selected/founded meshes.
            foreach (var uvMeshFilter in targetUVs)
                TryGenerateUV(uvMeshFilter);

            EditorUtility.DisplayDialog("Completed ✔", $@" UV mapping process(s) completed successfully. 
UV map(s) of '{targetUVs.Count}' mesh(s) were created.
You can support the Uv Map Creation Wizard tool by liking, sharing, or nice comments. 
Thanks for your choice. Game development is not a job is a mission.  ", "Close");
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Adds item to the menu to create the class's wizard
        /// </summary>
        [MenuItem("Tools/UV Map Creation Wizard")]
        private static void UVMapCreationWizard() =>
            DisplayWizard<UvMapCreationWizard>("UV Map Creation Wizard", "Create UV Map");

        /// <summary>
        /// If there are selections and a MeshFilter has components, it returns its components
        /// </summary>
        /// <returns></returns>
        private List<MeshFilter> GetSelectedMeshFilters()
        {
            // Get selection objects.
            var selectionObjects = Selection.objects;
            var meshFilters = new List<MeshFilter>();
            if (selectionObjects == null)
                return meshFilters;

            //Find meshFilters
            foreach (var selection in selectionObjects)
            {
                var sl = selection as GameObject;
                if (sl != null && sl.TryGetComponent(out MeshFilter mesh))
                    meshFilters.Add(mesh);
            }

            return meshFilters;
        }

        /// <summary>
        /// Try generates uv map for target mesh
        /// </summary>
        /// <param name="mesh">Target mesh</param>
        private static void TryGenerateUV(MeshFilter mesh)
        {
            try
            {
                GenerateUV(mesh.sharedMesh);
            }
            catch (Exception e)
            {
                if (EditorUtility.DisplayDialog("Failed ✗", $@"An unexpected error was encountered. 
Details of the error: {e} 
error caught in the {mesh.name} object", "Try Again", "Skip"))
                {
                    TryGenerateUV(mesh);
                }
                else
                {
                    Debug.LogWarning( mesh.name + " object UV map creation skipped!");
                }
            }   
        }

        /// <summary>
        ///  generates uv map for target mesh
        /// </summary>
        /// <param name="mesh"> Target Mesh</param>
        private static void GenerateUV(Mesh mesh)
        {
            UnwrapParam.SetDefaults(out UnwrapParam unwrapParam);
            Unwrapping.GenerateSecondaryUVSet(mesh, unwrapParam);
        }

        #endregion
    }
}
#endif