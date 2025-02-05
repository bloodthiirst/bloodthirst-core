using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
namespace Bloodthirst.Editor.BExcelEditor
{
    public class BExcelShortcuts : MonoBehaviour
    {

        [MenuItem("Bloodthirst Tools/Shortcuts/Update Localization")]
        public static void UpdateLocalization()
        {
            IEnumerable<BExcelOutput> assets = EditorUtils.FindAssetsByType<BExcelOutput>();
            
            BExcelOutput asset = assets.FirstOrDefault();
            Assert.IsNotNull(asset);

            BExcelExport export = new BExcelExport();
            export.AssetExportPath = AssetDatabase.GetAssetPath(asset);
            export.ScriptExportPath = asset.scriptPath;
            export.Namespace = asset.scriptNamespace;            

            string excelPath = "Assets/Content/Localization/LocalizationSource.xlsx";
            Object excelAsset = AssetDatabase.LoadAssetAtPath<Object>(excelPath);

            BExcelContext ctx = BExcelContext.Create(excelAsset);
            IBExcelFilter casted = export;
            casted.Setup(ctx);
            if(!casted.IsValid(out string err))
            {
                Debug.LogError(err);
                return;
            }

            casted.Execute();
            casted.Clean();
        }

    }
}
