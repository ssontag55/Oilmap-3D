class SeaWeedBob extends Clutter
{
	var bones : Transform[];
	private var baseRots : Vector3[];
	private var swayRotation : Vector3[];
	
	
	
	function Start()
	{
		var hit : RaycastHit;
		if(Physics.Raycast(transform.position,-Vector3.up,hit))
		{
			transform.position.y = hit.point.y;
		}
		transform.localScale.y = Random.Range(0.5,10.0);
		swayRotation = new Vector3[bones.length];
		InvokeRepeating("Sway",3.0,1.0);
	}
	function Update () {
		for(var i = 0; i < bones.length; i++)
		{
			bones[i].eulerAngles = Vector3.Slerp(bones[i].eulerAngles,bones[i].eulerAngles+swayRotation[i],Time.deltaTime);
		}
		
	}
	
	function Sway()
	{
		for(var i = 0; i < bones.length; i++)
		{
			swayRotation[i] = Random.insideUnitSphere*2.0;
		}
		
	}
	
	function HaltAmbience()
	{
		rendererRef.enabled = false;
		CancelInvoke("Sway");
	}
	function StartAmbience()
	{
		rendererRef.enabled = true;
		InvokeRepeating("Sway",3.0,1.0);
	}
	
}