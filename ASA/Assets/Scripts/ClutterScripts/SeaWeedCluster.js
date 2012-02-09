var seaWeedPrefab : GameObject;
var rockPrefab : GameObject;

private var clutter : GameObject[];

private var clutterMade : boolean = false;
function CreateSeaWeed(rad : float, maximumHeight : float)
{
	GetComponent(CapsuleCollider).radius *= CoordinateConverter.Scaling() * rad;
	GetComponent(CapsuleCollider).height *= CoordinateConverter.VerticalScaling();
	var numClones : int = Random.Range(2,5);
	clutter = new GameObject[numClones];
	for(var i = 0; i < numClones; i++)
	{
		var seaWeed : boolean = false;
		if(Random.Range(0,100) > 30)
		{
			seaWeed = true;
		}
		
		if(seaWeed)
		{
			clutter[i] = Instantiate(seaWeedPrefab,transform.position + Random.insideUnitSphere*GetComponent(CapsuleCollider).radius,Quaternion.identity);
			clutter[i].transform.localScale.y *= Random.Range(0.5, maximumHeight/clutter[i].transform.localScale.y);
			//clutter[i].transform.localScale.x *= CoordinateConverter.Scaling()*10.0;
			//clutter[i].transform.localScale.z *= CoordinateConverter.Scaling()*10.0;
		}
		else
		{
			clutter[i] = Instantiate(rockPrefab,transform.position + Random.insideUnitSphere*rad,Quaternion.identity);
		}
		clutter[i].transform.parent = transform;
		clutter[i].transform.localPosition.y = 0.0;
		
		var depthHit : RaycastHit;
		if(Physics.Raycast(clutter[i].transform.position,-Vector3.up,depthHit,10.0))
		{
			clutter[i].transform.position.y = depthHit.point.y;
		}
		clutter[i].GetComponent("Clutter").rendererRef.enabled = false;
		yield;
	}
	clutterMade = true;
}

function OnTriggerExit(col : Collider)
{
	if(!clutterMade || clutter == null)
		return;
	for(var i = 0; i < clutter.length; i++)
	{
		clutter[i].GetComponent("Clutter").rendererRef.enabled = false;
	}
}

function OnTriggerEnter(col : Collider)
{
	if(!clutterMade || clutter == null)
		return;
	for(var i = 0; i < clutter.length; i++)
	{
		clutter[i].GetComponent("Clutter").rendererRef.enabled = true;
	}
}