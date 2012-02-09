var movementSpeed : float = 1000.0;
var mouseSensitivity : float = 500.0;

var fogColor = Color (0, 0.4, 0.7, 0.6);
var fogDensity = 0.04;

private var establishDist : float = 1000.0;
private var establishPoint : Vector3;

private var snapping : boolean = false;
private var refVelocity : Vector3 = Vector3.zero;
function Update () 
{
	if(!snapping)
	{
		if(Input.GetKey("left alt"))
		{
			if(Input.GetMouseButtonDown(0))
			{
				establishPoint = transform.position + (establishDist*transform.forward*CoordinateConverter.Scaling());
			}
			else if(Input.GetMouseButton(0))
			{
				transform.Rotate(Vector3(-Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"),0) * Time.deltaTime*mouseSensitivity);
				
				transform.position = establishPoint - (establishDist*transform.forward*CoordinateConverter.Scaling());
			}
			else if(Input.GetMouseButton(1))
			{
				transform.Translate(Vector3(0,0,Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y")) * Time.deltaTime * movementSpeed*CoordinateConverter.Scaling()*100.0);
			}
			else if(Input.GetMouseButton(2))
			{
				transform.Translate(Vector3(-Input.GetAxis("Mouse X"),-Input.GetAxis("Mouse Y"),0) * Time.deltaTime*movementSpeed*CoordinateConverter.Scaling()*100.0);
			}
		}
	}
	transform.eulerAngles.z = 0;
	if(RenderSettings.fog && transform.position.y > -0.1)
	{
		RenderSettings.fog = false;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
	}
	else if(!RenderSettings.fog && transform.position.y <= -0.1)
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
	}
}

function Snap(pos : Vector3)
{
	if(snapping)
		return;
	var targetPos : Vector3 = pos - (transform.forward * establishDist*CoordinateConverter.Scaling()*10.0);
	snapping = true;
	while(Vector3.Distance(transform.position,targetPos) > establishDist*CoordinateConverter.Scaling()*10.0)
	{
		transform.position = Vector3.SmoothDamp(transform.position,targetPos,refVelocity,1.0);
		yield;
	}
	snapping = false;
}