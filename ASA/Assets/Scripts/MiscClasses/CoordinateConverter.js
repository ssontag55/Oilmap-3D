
class CoordinateConverter
{

	static var coordScaling : Vector3 = Vector3(0.001f,0.01f,0.001f);
	//static var coordScaling : Vector3 = Vector3(1,1,1);

	
	static function Scaling() : float
	{
		//1 Unity Meter = 100 real meters
		
		return coordScaling.x;
	}
	static function VerticalScaling() : float
	{
		return coordScaling.y;
	}
	
	
	

}