using UnityEngine;
using System.IO;

public class ScenarioLoader : MonoBehaviour {
    public GameObject terrainBuilder;
    public GameObject spillObject;
    public GameObject cam;
    public GameObject robot;
    
    public GUISkin theSkin;
    protected string scenarioPath;
    protected string zmpPath;
    protected string zmlPath;
    
    protected FileBrowser m_fileBrowser;
    
    protected string[] selectPatterns = {null,"*.ZNP"};
    protected string[] hardDirectories = {"","/OUTDATA"};
    
	protected string[] scenarioList;
	
	protected bool docHistory = false;
    protected bool selection = false;
    protected Vector2 scrollPos = Vector2.zero;
    protected int currentPattern = 0;
	protected bool badDirectory = false;
    [SerializeField]
    protected Texture2D m_directoryImage,
                        m_fileImage;
    
    
    protected void OnGUI () {
		
    	UIScalingCS.ScaleUI();
    	if(theSkin)
        	GUI.skin = theSkin;
        
		if(badDirectory)
		{
			DirectoryDoesntExist();
			return;
		}
		
		if(!selection)
			GUI.Box(new Rect(412,200,200,20), "Oilmap Scenario Loader");
		
        if(!selection && GUI.Button(new Rect(412,384,200,50), "Set New Working Directory"))
        {
        	selection = true;
        }
        if(!selection && GUI.Button(new Rect(412,434,200,50), "Load Previous Working Directory"))
        {
        	selection = true;
			docHistory = true;
        	if(File.Exists(Application.persistentDataPath+"/config.cfg"))
        	{
				GetScenarioList();
				
        		/*scenarioPath = GetMostRecentScenario();
        		m_fileBrowser = new FileBrowser(
                    new Rect(100, 100, 600, 500),
                    "Choose Text File",
                    FileSelectedCallback
                );
                currentPattern++;
                m_fileBrowser.CurrentDirectory = scenarioPath+hardDirectories[currentPattern];
               	m_fileBrowser.BrowserType = FileBrowserType.File;
               	m_fileBrowser.SelectionPattern = selectPatterns[currentPattern];
               	*/
        	}
			else
			{
				docHistory = false;
			}
        }
        if(selection)
        {
        	if(docHistory)
			{
				if(scenarioList != null && scenarioList.Length > 0)
				{
					scrollPos = GUI.BeginScrollView(new Rect(256,64,512,768),scrollPos,new Rect(0,0,512,50.0f*scenarioList.Length));
					float yIndex = 0.0f;
					for(int i = 0; i < scenarioList.Length; i++)
					{
						if(GUI.Button(new Rect(0,yIndex,512,50),scenarioList[i]))
						{
							docHistory = false;
							scenarioPath = scenarioList[i];
							m_fileBrowser = new FileBrowser(
                    		new Rect(100, 100, 600, 500),
                    		"Choose ZNP File",
                   			FileSelectedCallback
                			);
                			currentPattern++;
							if(!Directory.Exists(scenarioPath+hardDirectories[currentPattern]))
								badDirectory=true;
							else
							{
                				m_fileBrowser.CurrentDirectory = scenarioPath+hardDirectories[currentPattern];
               					m_fileBrowser.BrowserType = FileBrowserType.File;
               					m_fileBrowser.SelectionPattern = selectPatterns[currentPattern];
								RegisterScenarioDirectory();
							}
						}
						yIndex+= 50.0f;
					}
					GUI.EndScrollView();
				}
			}
			else
			{
        		if (m_fileBrowser != null) {
        		    m_fileBrowser.OnGUI();
        		} else {
        	   	 OnGUIMain();
        		}
			}
        }
    }
	
	void DirectoryDoesntExist()
	{
		GUI.Box(new Rect(306,300,412,50), scenarioPath+hardDirectories[currentPattern] + " Doesn't exist");
		if(GUI.Button(new Rect(306,400,206,20), "Choose different working directory"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		if(GUI.Button(new Rect(512,400,206,20), "Back to start"))
		{
			Application.LoadLevel(0);
		}
	}
    
	protected void GetScenarioList()
	{
		StreamReader fileReader = new StreamReader(Application.persistentDataPath+"/config.cfg");
		string fileContents = fileReader.ReadToEnd();
		fileReader.Close();
		scenarioList = fileContents.Split("\n"[0]);
	}
	
    protected void OnGUIMain() {
        
        GUILayout.BeginHorizontal();
            GUILayout.Label("Select Working Directory", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.Label(scenarioPath ?? "none selected");
            if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
                m_fileBrowser = new FileBrowser(
                    new Rect(100, 100, 600, 500),
                    "Click Select to set the Working Directory",
                    FileSelectedCallback
                );
                if(currentPattern == 0)
               		m_fileBrowser.BrowserType = FileBrowserType.Directory;
               	else
               	{
               		m_fileBrowser.CurrentDirectory = scenarioPath+hardDirectories[currentPattern];
               		m_fileBrowser.BrowserType = FileBrowserType.File;
               		//m_fileBrowser.SetNewDirectory(scenarioPath);
               	}
                m_fileBrowser.SelectionPattern = selectPatterns[currentPattern];
                m_fileBrowser.DirectoryImage = m_directoryImage;
                m_fileBrowser.FileImage = m_fileImage;
            }
        GUILayout.EndHorizontal();
    }
    
    protected void FileSelectedCallback(string path) {
		if(path == null || path == "")
		{
			Application.LoadLevel(0);
			return;
		}
        m_fileBrowser = null;
        switch(currentPattern)
        {
        	case 0:
        		scenarioPath = path;
        		 m_fileBrowser = new FileBrowser(
                    new Rect(100, 100, 600, 500),
                    "Choose ZNP File",
                    FileSelectedCallback
                );
                currentPattern++;
                m_fileBrowser.CurrentDirectory = scenarioPath+hardDirectories[currentPattern];
               	m_fileBrowser.BrowserType = FileBrowserType.File;
               	m_fileBrowser.SelectionPattern = selectPatterns[currentPattern];
               	RegisterScenarioDirectory();
        	break;
        	case 1:
        		currentPattern++;
				
					
        		string gridFile = ZNPRead.ReadZNP(path);
        		LoadBathymetry(scenarioPath + "/GRIDS/"+gridFile.Replace(".GRD",".DEP"));
        		zmpPath = path.Replace(".ZNP",".ZMP");
        		zmlPath = path.Replace(".ZNP",".ZML");
        		LoadSpill();
        	break;
        	
        	
        	default:
        	break;
        }
        
        	
        
        if(currentPattern >= selectPatterns.Length)
        	Destroy(gameObject);
        Debug.Log("Selected Directory: " +path);
    }
    
    protected void RegisterScenarioDirectory()
    {
		if(File.Exists(Application.persistentDataPath+"/config.cfg"))
		{
			GetScenarioList();
		}
    	StreamWriter fileWriter = File.CreateText(Application.persistentDataPath+"/config.cfg");
		
		
    	fileWriter.Write(scenarioPath);
		if(scenarioList != null && scenarioList.Length > 0)
		{
			
			for(int i = 0; i < scenarioList.Length; i++)
			{
				if(!scenarioList[i].Equals(scenarioPath))
				{
					fileWriter.Write("\n");
					fileWriter.Write(scenarioList[i]);
				}
			}
		}
    	fileWriter.Close();
    }
    
    protected string GetMostRecentScenario()
    {
    	StreamReader fileReader = new StreamReader(Application.persistentDataPath+"/config.cfg");
    	string retString = fileReader.ReadLine();
    	fileReader.Close();
    	return retString;
    }
    
    
    protected void LoadBathymetry(string depPath)
    {
    	GameObject clone = (Instantiate(terrainBuilder,Vector3.zero,Quaternion.identity) as GameObject);
    	clone.GetComponent<BuildTerrainMesh>().dataFPath = depPath;
    	//Instantiate(robot,new Vector3(300,1,300),Quaternion.identity);
    	Instantiate(cam,new Vector3(300,1,300),Quaternion.identity);
    }
    protected void LoadSpill()
    {
    	GameObject clone = (Instantiate(spillObject,Vector3.zero,Quaternion.identity) as GameObject);
    	clone.GetComponent<OilSpill>().ptrFile = zmpPath;
    	clone.GetComponent<OilSpill>().particleFile = zmlPath;
    }
}