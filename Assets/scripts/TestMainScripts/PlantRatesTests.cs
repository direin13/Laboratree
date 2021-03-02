using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class PlantRatesTests
{
    PlantManager pmanager;
    public readonly List<PlantRates> plantRates = new List<PlantRates>();

    [SetUp] //called at the start of each test
    public void Setup()
    {

        pmanager = GameObject.Find("GameManager").GetComponent<PlantManager>();

        pmanager.activePlants = new GameObject[pmanager.maxActivePlants];

        pmanager.allPrefabs.Add("Aloe");
        pmanager.allPrefabs.Add("Jade");
        pmanager.allPrefabs.Add("Echeveria");
        pmanager.allPrefabs.Add("Basic Plant");

        int i = 0;
        foreach (string prefab in pmanager.allPrefabs)
        {
            if (pmanager.prefabMappings.ContainsKey(prefab))
            {
                pmanager.prefabMappings.Remove(prefab);
            }

            GameObject plant = (GameObject)Resources.Load(PlantManager.prefabPath + prefab);
            pmanager.prefabMappings.Add(prefab, plant);

            GameObject testPlant = pmanager.MakePlant("tester"+i.ToString(), prefab);

            if (i < pmanager.maxActivePlants)
                pmanager.SetPlantStatus(testPlant, true);
            plantRates.Add(testPlant.GetComponent<PlantRates>());

            i++;
        }
    }

    [TearDown] //called at the end of each test
    public void Teardown()
    {
        foreach (GameObject obj in pmanager.plantCollection)
        {
            GameObject.DestroyImmediate(obj);
        }

        pmanager.plantCollection.Clear();
        Array.Clear(pmanager.activePlants, 0, pmanager.activePlants.Length);
        plantRates.Clear();
    }


    // A Test behaves as an ordinary method
    [Test]
    public void DepenedenceAttrGetTest1()
    {
        string [] dependencies = { "Lighting", "Water", "Temperature", "Fertiliser"};
        foreach(PlantRates pr in plantRates)
        {
            foreach (string dep in dependencies)
            {
                Assert.IsTrue(pr.GetDepComp(dep) != null, String.Format("Could not find '{0}' dependency in '{1}'", dep, pr.gameObject.name) );
            }
        }
    }

    [Test]
    public void DepenedenceAttrGetTest2()
    {
        string[] dependencies = { "Lighting", "Water", "Temperature", "Fertiliser" };
        foreach (PlantRates pr in plantRates)
        {
            foreach (string dep in dependencies)
            {
                GameObject depObj = pr.GetDepComp(dep).gameObject;
                Assert.IsTrue(pr.GetDepComp(depObj) != null, String.Format("Could not find '{0}' dependency in '{1}'", dep, pr.gameObject.name));
            }
        }
    }


    [Test]
    public void DepenedenceAttrGetTest4()
    {
        string[] dependencies = { "notfound", "nouExist", "Hello", "Structure" };
        foreach (PlantRates pr in plantRates)
        {
            foreach (string dep in dependencies)
            {
                Assert.IsTrue(!pr.GetDepComp(dep), String.Format("'{0}' dependency should not be in '{1}'", dep, pr.gameObject.name));
            }
        }
    }


    [Test]
    public void DepenedenceAttrGetTest5()
    {
        string[] dependencies = { "notfound", "nouExist", "Hello", "Structure" };
        foreach (PlantRates pr in plantRates)
        {
            foreach (string dep in dependencies)
            {
                Assert.IsTrue(!pr.GetDepComp(dep), String.Format("'{0}' dependency should not be in '{1}'", dep, pr.gameObject.name));
            }
        }
    }


    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlantRatesTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
