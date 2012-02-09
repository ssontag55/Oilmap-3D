using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OilSpill : MonoBehaviour {
	
	/*
	 * This is our script for our oil spill.
	 * 
	 * We develop the track line here
	 * Draw UI elements for handling playback for the spill
	 * And set up the information for the trackline so that our main cameras
	 * can render it using OpenGL.
	 */ 
	
	public GameObject spillSubEmitter;		// A template emitter for if the current number of particles exceeds the maximum for one emitter.
	public string ptrFile;			// Path of the pointer file
	public string particleFile;		// Path of the particle file
	
	public Texture2D playButton;
	public Texture2D stopButton;
	
	private OilID[] recs;			// The list of all records for the spill
	private SiSpillets[] siRecs;
	private Particle3D[] spillets;	// The current collection of spillets for the current record.
	
	private int idIndex = 0;		// The index of the current record.
	
	private float timelineSlider = 0.0f;	// The value of the slider for playback
	private float stepInterval;// = Time.fixedDeltaTime*2.0f;	// The slider step interval when automaticall playing back
	private bool playback = false;			// Are we playing back?
	
	
	private bool tracklineMade = false;
	private float tracklineProgress = 0.0f;
	// Use this for initialization
	IEnumerator Start () {
		stepInterval = Time.fixedDeltaTime*2.0f;
		// Yield for two frames
		yield return 0;
		yield return 0;
		// Determine where the spill should move itself to.
		
		Vector2 spillLoc = GeographicCoords.GeographicDistance(GeographicCoords.GetBase(),new Vector2(GeographicCoords.SpillLoc.x,GeographicCoords.SpillLoc.z)) * GeographicCoords.Scaling();
		if(GeographicCoords.SpillLoc == Vector3.zero)
			spillLoc = Vector3.zero;
		transform.position = new Vector3(spillLoc.x,-GeographicCoords.SpillLoc.y*GeographicCoords.VerticalScaling(),spillLoc.y);
		// Read from the pointer file, populate the records.
		if(ptrFile.Contains(".LU3"))
			OilData.ReadLU3(ptrFile,ref siRecs);
		else
			OilData.ReadOMP(ptrFile,ref recs);
		
		// Develop the track line
		StartCoroutine(DevelopTrackLine());
	}
	
	
	void Update()
	{
		// If we press "p," toggle the playback state.
		if(Input.GetKeyDown("`"))
		{
			TogglePlayback();
		}
	}
	
	// Handles all UI related stuff for the spill
	void OnGUI()
	{
		UIScalingCS.ScaleUI();
		
		
		if(!tracklineMade)
		{
			GUI.Box(new Rect(412,Screen.height*0.5f,200,45),"");
			GUI.Box(new Rect(412,Screen.height*0.5f,200,30),"Building Trackline:");
			float readOnly = GUI.HorizontalSlider(new Rect(412,Screen.height*0.5f+30,200,30),tracklineProgress,0.0f,1.0f);
			return;
		}
		// As long as we've actually gotten something useful from the OMP file
		// We can draw things.
		if(recs != null || siRecs != null)
		{
			// Playback controls.
			if(playback)
			{
				// Stop button
				//if(GUI.Button(new Rect(5,743,20,20),"[]"))
				if(GUI.Button(new Rect(5,728,30,30),new GUIContent(stopButton,"Stop Spill Playback")))
				{
					playback = false;
					CancelInvoke("ChangeParticles");
				}
			}
			else
			{
				// If playback is stopped then allow us to alter the interval of time between each record.
				
				stepInterval = GUI.HorizontalSlider(new Rect(240,708,50,30),stepInterval,Time.fixedDeltaTime*2.0f,1.0f);
				GUI.Box(new Rect(290,708,300,30),"Playback speed: 1 step every " + stepInterval + " second(s)");
				// Play button.
				//if(GUI.Button(new Rect(5,743,20,20),">"))
				if(GUI.Button(new Rect(5,728,30,30),new GUIContent(playButton,"Play through spill timeline")))
				{
					playback = true;
					InvokeRepeating("ChangeParticles",stepInterval,stepInterval);
				}
				
			}
			// Additionally, allow us to click and dragon on the slider to alter it.
			// And display the timestep information.
			int tStepLength = 0;
			if(recs != null)
				tStepLength = recs.Length;
			else
				tStepLength = siRecs.Length;
			timelineSlider = GUI.HorizontalSlider(new Rect(40,738,950,30),timelineSlider,0.0f,1.0f * tStepLength);
			GUI.Box(new Rect(40,708,200,30),"Timestep: " + Mathf.Round(timelineSlider));
			if(GUI.changed && !playback)
			{
				// If we are in fact scrubbing the slider, then change the state of all particles based on the timeslider's value.
				GetSpillState((int)Mathf.Round(timelineSlider));
			}
		}
		if(GUI.tooltip != "")
			GUI.Box(new Rect((Input.mousePosition.x * (UIScalingCS.width/Screen.width)),(768-Input.mousePosition.y)-50, 100, 50), GUI.tooltip);
	}
	
	
	// Handles the toggling of playback
	// InvokeRepeating takes 3 arguments.  The function we're calling
	// How long from now to call it for the first time
	// and how often we should call it after that in seconds.
	void TogglePlayback()
	{
		playback = !playback;
		if(playback)
			InvokeRepeating("ChangeParticles",stepInterval,stepInterval);
		else
			CancelInvoke("ChangeParticles");
	}
	
	// Get the spill state of the particles at time stamp tStamp.
	
	
	void GetSpillState(int tStamp)
	{
		// Read the particle data from record tStamp.
		idIndex = tStamp;
		if(particleFile.Contains(".TR3"))
			Debug.Log(OilData.ReadTR3(particleFile,siRecs,ref spillets,tStamp));
		else
			OilData.ReadOML(particleFile,recs,ref spillets,0,tStamp);
		
		// Clear all particles currently out in the world
		particleEmitter.particles = null;
		
		foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
		
		
		int theCount = 0;
		// Get the count of relevant particles
		for(int j = 0; j < spillets.Length; j++)
		{
			
			if(spillets[j].GetNWhere() != 21)
			{
				theCount++;
				
			}
		}
		// Figure out the total number of emitters we need to display the particles in this step.
		// The maximum particles we can have per emitter is 16250, so anything below that only needs one emitter.
		int totalEmitters = (int)Mathf.Floor(theCount/16250) + 1;
		int remainingCount = theCount;
		int spilletIndex = 0;
		
		
		
		
		if(totalEmitters > 1)
		{
			for(int j = 0; j < totalEmitters; j++)
			{
				GameObject emitClone = (Instantiate(spillSubEmitter,transform.position,Quaternion.identity) as GameObject);
				emitClone.transform.parent = transform;
				emitClone.particleEmitter.particles = null;
				emitClone.particleEmitter.minEnergy = Mathf.Infinity;
				emitClone.particleEmitter.maxEnergy = Mathf.Infinity;
				emitClone.particleEmitter.minEmission = Mathf.Min(16250,remainingCount);
				emitClone.particleEmitter.maxEmission = Mathf.Min(16250,remainingCount);
			
				emitClone.particleEmitter.Emit();
				Particle[] theParticles = emitClone.particleEmitter.particles;
				int particleIndex = 0;
				
				while(particleIndex < emitClone.particleEmitter.minEmission && spilletIndex < theCount)
				{
					if(spillets[spilletIndex].GetNWhere() != 21)
					{
						theParticles[particleIndex].position = spillets[spilletIndex].GetXYZ();
						
						theParticles[particleIndex].size += spillets[spilletIndex].GetRadius() * GeographicCoords.Scaling();
						theParticles[particleIndex].color = spillets[spilletIndex].GetPColor();
						theParticles[particleIndex].color =new Color(1.0f,1.0f,1.0f,0.5f+spillets[spilletIndex].GetThickness());
						
						
						particleIndex++;
						//Debug.Log("Particle Index: " + particleIndex);
					}
					spilletIndex++;
					//Debug.Log("Spillet Index: " + spilletIndex);
				}
				remainingCount -= (int)emitClone.particleEmitter.minEmission;
				emitClone.particleEmitter.particles = theParticles;
			}
		}
		else
		{
			particleEmitter.particles = null;
			particleEmitter.minEnergy = Mathf.Infinity;
			particleEmitter.maxEnergy = Mathf.Infinity;
			particleEmitter.minEmission = theCount;
			particleEmitter.maxEmission = theCount;
			particleEmitter.Emit();
			Particle[] theParticles = particleEmitter.particles;
			
			
			int particleIndex = 0;
			
			for(int i = 0; i < spillets.Length; i++)
			{
				if(spillets[i].GetNWhere() != 21)
				{
					
					theParticles[particleIndex].position = spillets[i].GetXYZ();
					
					theParticles[particleIndex].size += spillets[i].GetRadius() * GeographicCoords.Scaling();
					theParticles[particleIndex].color = spillets[i].GetPColor();
					theParticles[particleIndex].color =new Color(1.0f,1.0f,1.0f,0.5f+spillets[i].GetThickness());
					
					particleIndex++;
					
				}
				
			}
	
			
			particleEmitter.particles = theParticles;
		
		}
		
		
	}
	
	
	// This is similar to the function above
	// Only it uses idIndex and increments it each time the function is called.
	// Used only for playing back the spill data over time.
	
	
	void ChangeParticles()
	{
		
		if(particleFile.Contains(".TR3"))
			OilData.ReadTR3(particleFile,siRecs,ref spillets,idIndex);
		else
			OilData.ReadOML(particleFile,recs,ref spillets,0,idIndex);
		timelineSlider = 1.0f * idIndex;
		
		particleEmitter.particles = null;
		
		foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
		
		
		int theCount = 0;
		// Get the count of relevant particles
		for(int j = 0; j < spillets.Length; j++)
		{
			if(spillets[j].GetNWhere() != 21)
			{
				theCount++;
				
			}
		}
		// Figure out the total number of emitters we need to display the particles in this step.
		// The maximum particles we can have per emitter is 16250, so anything below that only needs one emitter.
		int totalEmitters = (int)Mathf.Floor(theCount/16250) + 1;
		int remainingCount = theCount;
		int spilletIndex = 0;
		
		
		
		
		if(totalEmitters > 1)
		{
			for(int j = 0; j < totalEmitters; j++)
			{
				GameObject emitClone = (Instantiate(spillSubEmitter,transform.position,Quaternion.identity) as GameObject);
				emitClone.transform.parent = transform;
				emitClone.particleEmitter.particles = null;
				emitClone.particleEmitter.minEnergy = Mathf.Infinity;
				emitClone.particleEmitter.maxEnergy = Mathf.Infinity;
				emitClone.particleEmitter.minEmission = Mathf.Min(16250,remainingCount);
				emitClone.particleEmitter.maxEmission = Mathf.Min(16250,remainingCount);
			
				emitClone.particleEmitter.Emit();
				Particle[] theParticles = emitClone.particleEmitter.particles;
				int particleIndex = 0;
				
				while(particleIndex < emitClone.particleEmitter.minEmission && spilletIndex < theCount)
				{
					if(spillets[spilletIndex].GetNWhere() != 21)
					{
						theParticles[particleIndex].position = spillets[spilletIndex].GetXYZ();
						
						theParticles[particleIndex].size += spillets[spilletIndex].GetRadius() * GeographicCoords.Scaling();
						theParticles[particleIndex].color = spillets[spilletIndex].GetPColor();
						theParticles[particleIndex].color =new Color(1.0f,1.0f,1.0f,0.5f+spillets[spilletIndex].GetThickness());
						
						
						particleIndex++;
						//Debug.Log("Particle Index: " + particleIndex);
					}
					spilletIndex++;
					//Debug.Log("Spillet Index: " + spilletIndex);
				}
			
				emitClone.particleEmitter.particles = theParticles;
			}
		}
		else
		{
			particleEmitter.particles = null;
			particleEmitter.minEnergy = Mathf.Infinity;
			particleEmitter.maxEnergy = Mathf.Infinity;
			particleEmitter.minEmission = theCount;
			particleEmitter.maxEmission = theCount;
			particleEmitter.Emit();
			Particle[] theParticles = particleEmitter.particles;
			
			
			int particleIndex = 0;
			
			for(int i = 0; i < spillets.Length; i++)
			{
				if(spillets[i].GetNWhere() != 21)
				{
					
					theParticles[particleIndex].position = spillets[i].GetXYZ();
					//if(transform.position == Vector3.zero)
					//	transform.position = spillets[i].GetXYZ();
					theParticles[particleIndex].size += spillets[i].GetRadius() * GeographicCoords.Scaling();
					theParticles[particleIndex].color = spillets[i].GetPColor();
					theParticles[particleIndex].color =new Color(1.0f,1.0f,1.0f,0.5f+spillets[i].GetThickness());
					
					particleIndex++;
					
				}
				
			}
	
			
			particleEmitter.particles = theParticles;
		
		}
		//if(particleEmitter.particles.Length > 0 && transform.position == Vector3.zero)
		//	transform.position = particleEmitter.particles[0].position;
		idIndex++;
		if(particleFile.Contains(".TR3"))
		{
			if(idIndex >= siRecs.Length)
				idIndex = 0;
		}
		else
		{
			if(idIndex >= recs.Length)
				idIndex = 0;
		}
		
	}
	
	/// <summary>
	/// Develops the track line.
	/// </summary>
	IEnumerator DevelopTrackLine()
	{
		// We need to determine the total number of verts to go into the trackline.
		// Not necessarily the total number of timestamps.
		
		// Since we don't know the total vert count, we'll just make a dynamic list.
		List<Vector3> positions = new List<Vector3>();
		if(particleFile.Contains(".TR3"))
		{
			for(int i = 0; i < siRecs.Length; i++)
			{
				tracklineProgress = (1.0f*i) / (1.0f*siRecs.Length);
				// We want the highest particle in the entire record.
				// Since our lowest depth is MaxCoords.y, we set our max to that
				// and then any particle that is higher up we change the value of max.
				Vector3 max = GeographicCoords.MaxCoords;
				OilData.ReadTR3(particleFile,siRecs,ref spillets,i);
				for(int j = 0; j < spillets.Length;j++)
				{
					if(spillets[j].GetNWhere() != 21)
					{
						if(spillets[j].GetXYZ().y > max.y)
						{
							// Run through the list of spillets and set a new max
							// if this spillet is indeed closer to the surface than the current max.
							max = spillets[j].GetXYZ();
						}
						
						
					}
					
				}
				// If we made a new assignment for the maximum then add that to the list of positions.
				if(max != GeographicCoords.MaxCoords)
					positions.Add(max);
				
				
				yield return 0;
				
			}
			tracklineMade = true;
			// Set the trackline components of the main camera and the ortho camera to use the verts we just defined.
			Camera.main.GetComponent<Trackline>().verts = positions.ToArray();
			GameObject.FindWithTag("OrthoCam").GetComponent<Trackline>().verts = positions.ToArray();
		}
		else
		{
			for(int i = 0; i < recs.Length; i++)
			{
				tracklineProgress = (1.0f*i) / (1.0f*recs.Length);
				// We want the highest particle in the entire record.
				// Since our lowest depth is MaxCoords.y, we set our max to that
				// and then any particle that is higher up we change the value of max.
				Vector3 max = GeographicCoords.MaxCoords;
				OilData.ReadOML(particleFile,recs,ref spillets,0,i);
				for(int j = 0; j < spillets.Length;j++)
				{
					if(spillets[j].GetNWhere() != 21)
					{
						if(spillets[j].GetXYZ().y > max.y)
						{
							// Run through the list of spillets and set a new max
							// if this spillet is indeed closer to the surface than the current max.
							max = spillets[j].GetXYZ();
						}
						
						
					}
					
				}
				// If we made a new assignment for the maximum then add that to the list of positions.
				if(max != GeographicCoords.MaxCoords)
					positions.Add(max);
				
				
				yield return 0;
				
			}
			tracklineMade = true;
			// Set the trackline components of the main camera and the ortho camera to use the verts we just defined.
			Camera.main.GetComponent<Trackline>().verts = positions.ToArray();
			GameObject.FindWithTag("OrthoCam").GetComponent<Trackline>().verts = positions.ToArray();
		}
		
	}
	
}
