var rendererRef : Renderer;
function Start()
{
	if(!rendererRef)
		rendererRef = renderer;
	rendererRef.enabled = false;
	//GetComponent(CapsuleCollider).radius *= CoordinateConverter.Scaling();
	//GetComponent(CapsuleCollider).height *= CoordinateConverter.VerticalScaling();
}
function OnTriggerEnter (col : Collider) {
	if(col.tag == "MainCamera")
	{
		if(!rendererRef)
			rendererRef = renderer;
		rendererRef.enabled = true;
	}
}

function OnTriggerExit(col : Collider)
{
	if(col.tag == "MainCamera")
	{
		if(!rendererRef)
			rendererRef = renderer;
		rendererRef.enabled = false;
	}
}