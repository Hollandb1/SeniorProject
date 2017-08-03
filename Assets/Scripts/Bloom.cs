using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Bloom : MonoBehaviour
{
	public enum WeatherStates
	{
		Sunny,
		Rain,
		Night,
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
		else
		{
			pingPong();
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

}