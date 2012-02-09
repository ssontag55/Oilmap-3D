var fogColor = Color (0, 0.4, 0.7, 0.6);
var fogDensity = 0.04;
var skyboxMat : Material;
var bubbleEffect : GameObject;
var bubbleSpawnPoint : Transform;


function Start()
{
	GetComponent(Camera).backgroundColor = fogColor;
	RenderSettings.skybox = skyboxMat;
}
function Update () {
	if(RenderSettings.fog && transform.position.y >= 0.0)
	{
		RenderSettings.fog = false;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.skybox = skyboxMat;
		GetComponent("Fisheye").enabled = false;
		GetComponent("MotionBlur").enabled = false;

	}
	else if(!RenderSettings.fog && transform.position.y < 0.0)
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.skybox = null;
		GetComponent("Fisheye").enabled = true;
		GetComponent("MotionBlur").enabled = true;
		
	}
}