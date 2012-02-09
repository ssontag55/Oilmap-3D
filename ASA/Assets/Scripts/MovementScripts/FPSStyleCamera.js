var movementSpeed : float = 10.0;
var mouseSensitivity : float = 100.0;

var parentRig : Transform;

function Update()
{
	if(parentRig == null)
		parentRig = transform.parent;
		
	parentRig.Rotate(Vector3(0,Input.GetAxis("Mouse X"),0) * mouseSensitivity * Time.deltaTime);
	
	
	transform.Rotate(Vector3(-Input.GetAxis("Mouse Y"),0,0) * mouseSensitivity * Time.deltaTime);
	
	var input = Input.GetAxis("Horizontal") * parentRig.right + Input.GetAxis("Vertical")*transform.forward;
	
	parentRig.position += input*movementSpeed*Time.deltaTime;
	
	//parentRig.Translate(input*movementSpeed*Time.deltaTime);
}