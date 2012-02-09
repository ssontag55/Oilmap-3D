var horizontalSpeed : float = 10.0;	// Meters/sec
var verticalSpeed : float = 10.0;

var gravity : float = Physics.gravity.y;

var subPos : Texture2D;

var placementIndicator : GameObject;

private var verticalVelocity : float = 0.0;
private var camRef : Transform;

private var movement : Vector3 = Vector3.zero;
private var charCtrl : CharacterController;
private var theSpeed : float = 0.0;

private var minimap : Texture2D;
private var mmapScaling : Vector2 = Vector2(256.0,256.0);

private var precisionSpeed : float = horizontalSpeed;

private var togglePrecision : boolean = false;

private var mapAspect : float = 1.0;

private var overheadNav : boolean = false;

function Start()
{
	camRef = Camera.main.transform;
	charCtrl = GetComponent(CharacterController);
	
	horizontalSpeed *= CoordinateConverter.VerticalScaling();
	precisionSpeed = horizontalSpeed * 0.1;
	verticalSpeed *= CoordinateConverter.VerticalScaling();
	gravity *= CoordinateConverter.VerticalScaling();
	
	transform.localScale = Vector3(CoordinateConverter.VerticalScaling(),CoordinateConverter.VerticalScaling(),CoordinateConverter.VerticalScaling());
	
	var clone = Instantiate(placementIndicator,transform.position,transform.rotation);
	yield;
	clone.transform.parent = transform;
	clone.transform.localPosition = Vector3.zero;
	clone.transform.localEulerAngles = Vector3.zero;
	
}

function Update () {
	if(overheadNav)
		Screen.lockCursor = false;
	if(Input.GetKeyDown("x"))
	{
		togglePrecision = !togglePrecision;
		Debug.Log("Precision movement is: " + togglePrecision);
	}
	if(transform.position.y > 0.0)
	{
		//transform.position.y = -0.1;
		theSpeed = Mathf.Lerp(theSpeed,0.0,Time.deltaTime);
		// Apply gravity.
		verticalVelocity += gravity*Time.deltaTime;
	}
	else
	{
		verticalVelocity = Mathf.Lerp(verticalVelocity,0.0,0.5);
		if(Screen.lockCursor || overheadNav)
			SmartMovement();
		else
			theSpeed = Mathf.Lerp(theSpeed,0.0,0.9);
	}
	charCtrl.Move((theSpeed*movement + Vector3(0,verticalVelocity,0)) * Time.deltaTime);
	var lookVec : Vector3 = movement + Vector3(0,verticalVelocity,0);
	if(movement != Vector3.zero)
	{
		if(transform.position.y > 0.0)
		{
			var pointVec : Vector3 = theSpeed*movement+Vector3(0,verticalVelocity,0);
			transform.rotation = Quaternion.LookRotation(pointVec.normalized);
		}
		else
		{
			transform.rotation = Quaternion.LookRotation(movement);
		}
		
		
	}
	
}

function OnGUI()
{
	UIScaling.ScaleUI();
	
	//GUI.Box(Rect(768,316,196,30), "Depth: ~" + Mathf.Round((-1*(transform.position.y)/CoordinateConverter.VerticalScaling())) + "meters");
	
	GUI.Label(Rect(0,250,200,50), "Speed: " + Mathf.Floor(theSpeed/CoordinateConverter.VerticalScaling()) + " Meters/Second");
	
	/*if(minimap != null)
	{
		GUI.Box(Rect(1020-minimap.width,0,minimap.width+4,minimap.height+4),"");
		var heightDif : float = minimap.height*mapAspect - minimap.height;
		GUI.DrawTexture(Rect(1022-minimap.width,-heightDif*0.5 + 2,minimap.width,minimap.height*mapAspect+2),minimap);
		if(subPos != null)
		{
			var offset : Vector2 = Vector2(1022-minimap.width,0);
			
			var upperLeftCoords : Vector2 = offset;
			
			upperLeftCoords.x += (transform.position.x* (minimap.width/mmapScaling.x));
			upperLeftCoords.x -= subPos.width*0.5;
			upperLeftCoords.y += minimap.height - 1.0*(transform.position.z * (minimap.height/mmapScaling.y));
			upperLeftCoords.y -= subPos.height*0.5;
			
			GUI.DrawTexture(Rect(upperLeftCoords.x,upperLeftCoords.y,subPos.width,subPos.height),subPos);
		}
		GUI.Label(Rect(1024-minimap.width ,minimap.height,minimap.width*0.5,50),"X: "+transform.position.x);
		GUI.Label(Rect(1024-minimap.width+minimap.width*0.5,minimap.height,minimap.width*0.5,50),"Y: " + transform.position.z);
	}*/
}

function GrabMapData()
{
	var theLand : GameObject=  GameObject.FindWithTag("Geo");
	var worldVert : Vector3 = theLand.GetComponent(MeshFilter).mesh.vertices[theLand.GetComponent(MeshFilter).mesh.vertices.length-1];
	mapAspect = worldVert.x/worldVert.z;
	var miniMapCam : GameObject = GameObject.FindWithTag("MiniMapper");
	var renderTex : RenderTexture = miniMapCam.GetComponent(Camera).targetTexture;
	
	GetMap(renderTex,worldVert.x,worldVert.z);
	
}
function GetMap(theMap : RenderTexture, worldWidth : float, worldLength : float)
{
	var previousActive : RenderTexture = RenderTexture.active;
	RenderTexture.active = theMap;
	minimap = Texture2D(theMap.width,theMap.height);
	minimap.ReadPixels(Rect(0,0,theMap.width,theMap.height),0,0);
	minimap.Apply();
	RenderTexture.active = previousActive;
	mmapScaling = Vector2(worldWidth,worldLength);
}

function OverheadNavigation()
{
	overheadNav = true;
}
function NormalNavigation()
{
	overheadNav = false;
}
function SmartMovement()
{
	var fwd : Vector3 = camRef.forward;
	if(overheadNav)
		fwd = camRef.up;
	//fwd = Vector3(fwd.x,0,fwd.z).normalized;
	var rgt : Vector3 = Vector3(fwd.z,0,-fwd.x);
	
	var inputDir : Vector3 = fwd * Input.GetAxis("Vertical") + rgt * Input.GetAxis("Horizontal");
	inputDir = inputDir.normalized;
	var targetSpeed : float = Mathf.Min(inputDir.magnitude,1.0);
	
	if(togglePrecision)
		targetSpeed *= precisionSpeed;
	else
		targetSpeed *= horizontalSpeed;
	
	movement = inputDir;
	theSpeed = Mathf.Lerp(theSpeed,targetSpeed,Time.deltaTime);
}
@script RequireComponent(CharacterController);