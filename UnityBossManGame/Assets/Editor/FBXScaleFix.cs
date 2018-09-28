//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEditor;

public class FBXScaleFix : AssetPostprocessor {

	public void OnPreprocessModel()
	{
        ModelImporter modelImporter = (ModelImporter) assetImporter;
        modelImporter.globalScale = 1;
	}
}
