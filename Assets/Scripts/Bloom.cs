﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(MeshRenderer))]
public class Bloom : MonoBehaviour
{
    static int framecount;
    bool forwards = true;
    public Mesh[] meshes;
    MeshFilter[] meshFilters;
    GameObject[] flowers;
    GameObject[] objects;
    int index;
    void Start()
    {
        framecount = 0;
        index = 0;
        objects = GameObject.FindGameObjectsWithTag("mesh");
        var MyList = GameObject.FindGameObjectsWithTag("mesh").OrderBy(go => Convert.ToInt32(go.name.Substring(4))).ToList();

        meshes = new Mesh[objects.Length];
        int i = 0;
        foreach (GameObject obj in MyList){
            meshes[i] = obj.GetComponent<MeshFilter>().mesh;
            i++;
        }
        i = 0;
        flowers = GameObject.FindGameObjectsWithTag("Flower");
        print(flowers.Length);

        meshFilters = new MeshFilter[flowers.Length];
        foreach (GameObject flower in flowers){
			meshFilters[i] = flower.GetComponent<MeshFilter>();
            i++;
		}
    }

    void Update()
    {
        foreach(MeshFilter meshFilter in meshFilters)
        {
            meshFilter.mesh = meshes[index];
        }
            nextPosition(ref index, ref forwards);
    }

    public static void nextPosition(ref int index, ref bool forwards)
    {
        if (index == 121)
        {
            forwards = false;
            framecount = 0;
        }
        if (index == 0){
            forwards = true;
        }
        if (forwards == true){
            index++;
        }
        else{
            index--;
        }
    }

}