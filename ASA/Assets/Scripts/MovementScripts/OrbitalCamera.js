var subject : Transform;

var camDist : float = 5.0;
var rotationSpeed : float = 60.0;
var compass : Texture2D;
private var lastMoveTimer : float = 0.0;
private var overheadView : boolean = false;
private var overheadOffset : float = 50.0f;
function Start()
{
	camDist = Mathf.Max(CoordinateConverter.VerticalScaling()*camDist,Camera.main.nearClipPlane*1.5);
	if(subject == null)
		subject = GameObject.FindWithTag("Player").transform;
	Screen.lockCursor = true;
}
function LateUpdate () {
	
	
	//if(subject.position.y > 0)
	//{
	//	Snapping();
	//}
	if(Input.GetKeyDown("v"))
	{
		Screen.lockCursor = !Screen.lockCursor;
	}
	
	if(Input.GetKeyDown("o"))
	{
		overheadView = !overheadView;
		if(overheadView)
			subject.gameObject.BroadcastMessage("OverheadNavigation", SendMessageOptions.DontRequireReceiver);
		else
		{
			subject.gameObject.BroadcastMessage("NormalNavigation", SendMessageOptions.DontRequireReceiver);
			Screen.lockCursor = true;
		}
	}
	if(!overheadView)
	{
		if(Screen.lockCursor)
		{
			if(Input.GetAxis("Mouse Y") == 0.0 && Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && subject.position.y <= 0.0)
			{
				if(Time.time > lastMoveTimer + 2.0)
					Snapping();
			}
		
			else
			{
				lastMoveTimer = Time.time;
				transform.Rotate(Vector3(-rotationSpeed * Input.GetAxis("Mouse Y"),rotationSpeed * Input.GetAxis("Mouse X"),0)*Time.deltaTime);
				transform.eulerAngles.z = 0.0;
				transform.position = subject.position - (transform.forward * camDist);
			}
		}
	}
	else
	{
		if(Input.GetAxis("Vertical") || Input.GetAxis("Horizontal"))
		{
			transform.position = Vector3(subject.position.x,camDist*overheadOffset,subject.position.z);
			transform.LookAt(subject);
			transform.eulerAngles.y = 0;
		}
		else
			OverheadSnap();
	}
	
	overheadOffset -= Input.GetAxis("Mouse ScrollWheel");
	overheadOffset = Mathf.Clamp(overheadOffset,50.0f,200.0f);
}

function OverheadSnap()
{
	var tarPosition = Vector3(subject.position.x,camDist*overheadOffset,subject.position.z);
	transform.position = Vector3.Slerp(transform.position,tarPosition,Time.deltaTime);
	transform.LookAt(subject);
	transform.eulerAngles.y = 0;
}
function Snapping()
{
	
	transform.eulerAngles.z = 0.0;
	var tarPosition = subject.position - (subject.forward * camDist);
	transform.position = Vector3.Slerp(transform.position,tarPosition,Time.deltaTime);
	transform.LookAt(subject);
	
}
function Teleport()
{
	transform.position = subject.position - (transform.forward * camDist);
}
function OnGUI()
{
	UIScaling.ScaleUI();
	var compassRect = Rect(0,512,64,64);
	GUIUtility.RotateAroundPivot(-transform.eulerAngles.y,Vector2((compassRect.x+0.5*compassRect.width) * (Screen.width/UIScaling.width),(compassRect.y+0.5*compassRect.height)* (Screen.height/UIScaling.height)));
	
	GUI.DrawTexture(compassRect,compass);
}