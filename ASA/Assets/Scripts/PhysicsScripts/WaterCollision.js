var fogColor = Color (0, 0.4, 0.7, 0.6);
var fogDensity = 0.04;
/*function OnTriggerExit (col : Collider) {
	if(col.tag != "MainCamera")
		return;
	if(col.transform.position.y >= transform.position.y)
	{
		Debug.Log("Fog disabled");
		RenderSettings.fog = false;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		
	}
	else
	{
		Debug.Log("Fog enabled");
		RenderSettings.fog = true;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
	}
	
}*/

function OnTriggerEnter(col : Collider)
{
	if(col.tag != "MainCamera")
		return;
		
	
	if(col.transform.position.y >= transform.position.y)
	{
		Debug.Log("Fog enabled");
		RenderSettings.fog = true;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
	}
	else
	{
		Debug.Log("Fog disabled");
		RenderSettings.fog = false;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
	}
}
function ToggleEnabling(go : GameObject, theState : boolean)
{
	yield WaitForSeconds(0.1);
	go.GetComponent("ColorCorrectionEffect").enabled = theState;
}