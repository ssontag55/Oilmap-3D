class UIScaling
{
	static var width : float = 1024;
	static var height : float = 768;
	
	static function ScaleUI()
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3(Screen.width/width, Screen.height/height,1));
	}
}