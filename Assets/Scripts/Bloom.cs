using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System;

[RequireComponent(typeof(MeshRenderer))]
public class Bloom : MonoBehaviour
{
    public enum WeatherStates
    {
        Sunny,
        Rain,
        Night,
        Current,
        PingPong
    };
    public WeatherStates weatherState;
    bool forwards = true;
    Mesh[] meshes;
    MeshFilter[] meshFilters;
    GameObject[] flowers;
    GameObject[] objects;
    int positionIndex;

    void Start()
    {
        positionIndex = 0;
        objects = GameObject.FindGameObjectsWithTag("mesh");
        var MyList = GameObject.FindGameObjectsWithTag("mesh").OrderBy(go => Convert.ToInt32(go.name.Substring(4))).ToList();

        meshes = new Mesh[objects.Length];
        int i = 0;
        foreach (GameObject obj in MyList)
        {
            meshes[i] = obj.GetComponent<MeshFilter>().mesh;
            i++;
        }
        i = 0;
        flowers = GameObject.FindGameObjectsWithTag("Flower");
        print(flowers.Length);

        meshFilters = new MeshFilter[flowers.Length];
        foreach (GameObject flower in flowers)
        {
            meshFilters[i] = flower.GetComponent<MeshFilter>();
            i++;
        }
    }

    void Update()
    {
        if (weatherState == WeatherStates.Night)
        {
            bloomNight();
        }
        else if (weatherState == WeatherStates.Sunny)
        {
            bloomSunny();
        }
        else if (weatherState == WeatherStates.Rain)
        {
            bloomRain();
        }
        else if (weatherState == WeatherStates.PingPong)
        {
            pingPong();
        }
        else
        {
            getCurrentWeather();
        }
    }

    private void bloomRain()
    {
        if (positionIndex < 122)
        {
            positionIndex++;
            foreach (MeshFilter meshFilter in meshFilters)
            {
                meshFilter.mesh = meshes[positionIndex];
            }
        }
    }

    private void bloomSunny()
    {
        if (positionIndex < 61)
        {
            positionIndex++;
            foreach (MeshFilter meshFilter in meshFilters)
            {
                meshFilter.mesh = meshes[positionIndex];
            }
        }
        else if (positionIndex > 61)
        {
            positionIndex--;
            foreach (MeshFilter meshFilter in meshFilters)
            {
                meshFilter.mesh = meshes[positionIndex];
            }
        }
    }

    private void bloomNight()
    {
        if (positionIndex > 0)
        {
            positionIndex--;
            foreach (MeshFilter meshFilter in meshFilters)
            {
                meshFilter.mesh = meshes[positionIndex];
            }
        }
    }

    public void pingPong()
    {

        if (positionIndex == 122)
        {
            forwards = false;
        }
        if (positionIndex == 0)
        {
            forwards = true;
        }
        if (forwards == true)
        {
            positionIndex++;
        }
        else
        {
            positionIndex--;
        }
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.mesh = meshes[positionIndex];
        }
    }

    public void getCurrentWeather()
    {
        string url = "http://apidev.accuweather.com/currentconditions/v1/348735.json?language=en&apikey=hoArfRosT1215";
        WWW www = new WWW(url);

        StartCoroutine(WaitForRequest(www));
    }

    public IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        Debug.Log(www.text);
        int index = www.text.IndexOf("WeatherIcon", StringComparison.OrdinalIgnoreCase);
        char ic = www.text.ElementAt(index + 13);
        double icon = Char.GetNumericValue(ic);
        bool isDayTime = www.text.Contains("true");
        Debug.Log(isDayTime);
		Debug.Log("ICON: " + icon);

		// check for errors
		if (www.error == null)
        {
            //RootObject json = new RootObject();
            //json = JsonUtility.FromJson<RootObject>(www.text);
            if ((icon > 7 && icon < 30) || icon > 38)
			{
				bloomRain();
			}
            if (!isDayTime){
                bloomNight();
            }
            else{
                bloomSunny();
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}

//[System.Serializable]
//public class Metric
//{
//	public double Value { get; set; }
//	public string Unit { get; set; }
//	public int UnitType { get; set; }
//}
//[System.Serializable]
//public class Imperial
//{
//	public double Value { get; set; }
//	public string Unit { get; set; }
//	public int UnitType { get; set; }
//}
//[System.Serializable]
//public class Temperature
//{
//	public Metric Metric { get; set; }
//	public Imperial Imperial { get; set; }
//}
//[System.Serializable]
//public class RootObject
//{
//	//public string LocalObservationDateTime { get; set; }
//	//public int EpochTime { get; set; }
//	public string WeatherText { get; set; }
//	public int WeatherIcon { get; set; }
//	public bool IsDayTime { get; set; }
//	//public Temperature Temperature { get; set; }
//	//public string MobileLink { get; set; }
//	//public string Link { get; set; }
//	public static RootObject CreateFromJSON(string jsonString)
//	{
//		return JsonUtility.FromJson<RootObject>(jsonString);
//	}
//    public RootObject(){
//        WeatherText = "";
//        WeatherIcon = new int();
//        IsDayTime = new bool();
//    }
//}