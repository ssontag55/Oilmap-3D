var clutterObjs : GameObject[];

var clutterDensity : int = 100;
var variation : int = 10;

private var clutterReferences : GameObject[];


function Start()
{
	GetComponent(BoxCollider).size.y = Mathf.Abs(transform.position.y)*2.0;
	GetComponent(BoxCollider).center.y = (0.25*GetComponent(BoxCollider).size.y);
	/*var boxColRef : BoxCollider = GetComponent(BoxCollider);
	var cornerA : Vector2 = Vector2(transform.position.x-boxColRef.size.x*0.5,transform.position.z-boxColRef.size.z*0.5);
	var cornerB : Vector2 = Vector2(transform.position.x+boxColRef.size.x*0.5,transform.position.z+boxColRef.size.z*0.5);
	
	var amtClutter : int = Random.Range(clutterDensity-variation,clutterDensity+variation);
	
	clutterReferences = new GameObject[amtClutter];
	
	for(var i : int = 0; i < clutterReferences.length; i++)
	{
		clutterReferences[i] = Instantiate(clutterObjs[Random.Range(0,clutterObjs.length)],Vector3(Random.Range(cornerA.x,cornerB.x),0,Random.Range(cornerA.y,cornerB.y)),Quaternion.identity);
		clutterReferences[i].BroadcastMessage("HaltAmbience",SendMessageOptions.DontRequireReceiver);
		clutterReferences[i].transform.parent = transform;
		//yield;
	}*/
	
}

function BuildClutter()
{
	var boxColRef : BoxCollider = GetComponent(BoxCollider);
	var cornerA : Vector2 = Vector2(transform.position.x-boxColRef.size.x*0.5,transform.position.z-boxColRef.size.z*0.5);
	var cornerB : Vector2 = Vector2(transform.position.x+boxColRef.size.x*0.5,transform.position.z+boxColRef.size.z*0.5);
	
	var amtClutter : int = Random.Range(clutterDensity-variation,clutterDensity+variation);
	
	clutterReferences = new GameObject[amtClutter];
	
	for(var i : int = 0; i < clutterReferences.length; i++)
	{
		clutterReferences[i] = Instantiate(clutterObjs[Random.Range(0,clutterObjs.length)],Vector3(Random.Range(cornerA.x,cornerB.x),0,Random.Range(cornerA.y,cornerB.y)),Quaternion.Euler(new Vector3(0,Random.Range(0,355),0)));
		//clutterReferences[i].BroadcastMessage("HaltAmbience",SendMessageOptions.DontRequireReceiver);
		clutterReferences[i].transform.parent = transform;
		yield;
		if(clutterReferences == null)
			return;
	}
}

function KillClutter()
{
	if(clutterReferences != null && clutterReferences.length > 0)
	{
		for(var i : int = 0; i < clutterReferences.length; i++)
		{
			if(clutterReferences[i] != null)
				Destroy(clutterReferences[i]);
		}
	}
	clutterReferences = null;
}
function OnTriggerEnter(col : Collider)
{
	if(col.tag == "Player")
	{
		BuildClutter();
	}
}

function OnTriggerExit(col : Collider)
{
	if(col.tag == "Player")
	{
		KillClutter();
	}
}

function ChangeClutterState(nState : boolean)
{
	var msgSent : String = "HaltAmbience";
	if(nState)
	{
		msgSent = "StartAmbience";
	}
	for(var i : int = 0; i < clutterReferences.length; i++)
	{
		clutterReferences[i].BroadcastMessage(msgSent,SendMessageOptions.DontRequireReceiver);
	}
	
}