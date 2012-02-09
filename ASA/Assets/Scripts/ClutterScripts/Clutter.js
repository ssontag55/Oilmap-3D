
class Clutter extends MonoBehaviour
{
	var rendererRef : Renderer;
	var envLayer : LayerMask;
	
	function Start()
	{
		transform.position.y = 0;
		var hit : RaycastHit;
		if(Physics.Raycast(transform.position,-Vector3.up,hit))
		{
			transform.position.y = hit.point.y;
			transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.forward,hit.normal));
			transform.Rotate(new Vector3(0,Random.Range(0,355),0));
		}
		transform.localScale *= Random.Range(0.5,20.0);
	}
	
	function HaltAmbience()
	{
		rendererRef.enabled = false;
	}
	function StartAmbience()
	{
		rendererRef.enabled = true;
	}
	
}