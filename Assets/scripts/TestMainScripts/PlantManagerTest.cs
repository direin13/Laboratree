using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class PlantManagerTest
{
    PlantManager pmanager;

    [SetUp] //called at the end of each test
    public void Setup()
    {
        pmanager = GameObject.Find("GameManager").GetComponent<PlantManager>();
        pmanager.allPrefabs.Add("Aloe");
        pmanager.allPrefabs.Add("Jade");
        pmanager.allPrefabs.Add("Echeveria");
        pmanager.allPrefabs.Add("Basic Plant");
        foreach (string prefab in pmanager.allPrefabs)
        {
            if (pmanager.prefabMappings.ContainsKey(prefab))
            {
                pmanager.prefabMappings.Remove(prefab);
            }
            GameObject plant = (GameObject)Resources.Load(PlantManager.prefabPath + prefab);
            pmanager.prefabMappings.Add(prefab, plant);
        }

        pmanager.activePlants = new GameObject[pmanager.maxActivePlants];
    }


    [TearDown] //called at the end of each test
    public void Teardown()
    {
        foreach(GameObject obj in pmanager.plantCollection)
        {
            GameObject.DestroyImmediate(obj);
        }

        pmanager.plantCollection.Clear();
        Array.Clear(pmanager.activePlants, 0, pmanager.activePlants.Length);

    }

    // A Test behaves as an ordinary method
    [Test]
    public void PlantManagerStartUpTest()
    {
        // Use the Assert class to test condition
        Assert.IsTrue(pmanager.plantCollection.Count == 0, "Collection should be empty at the beginning");
        Assert.IsFalse(pmanager.plantCollection.Count > 0, "Collection should be empty at the beginning");
        for (int i = 0; i< pmanager.activePlants.Length; i++)
        {
            if (pmanager.activePlants[i])
            {
                Assert.Fail("Active plant array should be empty at the beginning");
            }
        }
    }

    [Test]
    public void MakePlantTest1()
    {
        string name = "TestPlant1";
        GameObject newPlant = pmanager.MakePlant(name, "Aloe");
        Assert.IsFalse(pmanager.plantCollection.Count == 0, String.Format("Plant wass added, the length should be 1. Got {0}", pmanager.plantCollection.Count) );
        Assert.IsFalse(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was added. It should not be active but it was", newPlant.name) );
        Assert.IsTrue(newPlant.name == name, String.Format("Plant '{0}' was added. It should be called {0} but it was called {1}", name, newPlant.name));
    }

    [Test]
    public void MakePlantTest2()
    {
        string name = "TestPlant2";
        GameObject newPlant = pmanager.MakePlant(name, "Echeveria");
        GameObject newPlant2 = pmanager.MakePlant(name + "(new)", "Jade");

        Assert.IsTrue(pmanager.plantCollection.Count == 2, String.Format("2 plants were added, the length should be 2. Got {0}", pmanager.plantCollection.Count));
        Assert.IsFalse(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was added. It should not be active but it was", newPlant.name));
        Assert.IsTrue(!pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was added. It should not be active but it was", newPlant2.name));

        Assert.IsTrue(newPlant.name == name, String.Format("Plant '{0}' was added. It should be called {0} but it was called {1}", name, newPlant.name));
        Assert.IsTrue(newPlant2.name == name + "(new)", String.Format("Plant '{0}' was added. It should be called {0} but it was called {1}", name+"(new)", newPlant2.name));
    }

    [Test]
    public void MakePlantTest3()
    {
        string name = "TestPlant";
        try
        {
            GameObject newPlant = pmanager.MakePlant(name, "PlantThatDoesntExist");
            Assert.Fail("This plant type doesn't exist but exception wasnt thrown");
        }
        catch (KeyNotFoundException) { }

        try
        {
            GameObject newPlant = pmanager.MakePlant(name, "Aloe");
            GameObject newPlant2 = pmanager.MakePlant(name, "Aloe");
            Assert.Fail("The plant '" + name + "' was already created. Shouldn't be able to make another one");
        }
        catch (ArgumentException) { }
    }

    [Test]
    public void MakePlantTest4()
    {
        string name = "AnotherTestPlant";
        try
        {
            GameObject newPlant = pmanager.MakePlant(name, "");
            Assert.Fail("This plant type doesn't exist but exception wasnt thrown");
        }
        catch (KeyNotFoundException) { }

        try
        {
            GameObject newPlant = pmanager.MakePlant(name, "Aloe");
            GameObject newPlant2 = pmanager.MakePlant(name, "Echeveria");
            Assert.Fail("The plant '" + name + "' was already created. Shouldn't be able to make another one");
        }
        catch (ArgumentException) { }
    }

    [Test]
    public void ActivatePlantTest()
    {
        string name = "TestPlant";
        GameObject newPlant = pmanager.MakePlant(name, "Echeveria");
        GameObject newPlant2 = pmanager.MakePlant(name+"(2)", "Aloe");
        MonoBehaviour.print(pmanager.activePlants.Length);

        pmanager.SetPlantStatus(newPlant2, true);

        Assert.IsTrue(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant2.name));
        Assert.IsFalse(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' wasn't activated. Should be false but got true", newPlant.name));
    }


    [Test]
    public void ActivatePlantTest2()
    {
        string name = "Max's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");

        pmanager.SetPlantStatus(newPlant, true);

        Assert.IsTrue(pmanager.PlantActive(newPlant2) == false, String.Format("Plant '{0}' wasn't activated. Should be false but got true", newPlant2.name));
        Assert.IsFalse(pmanager.PlantActive(newPlant) == false, String.Format("Plant '{0}' was activated. Should be false but got true", newPlant.name));

        pmanager.SetPlantStatus(newPlant, false);

        Assert.IsFalse(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was deactivated. Should be false but got true", newPlant.name));
    }



    [Test]
    public void ActivatePlantTest3()
    {
        string name = "Max's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Echeveria");

        pmanager.SetPlantStatus(newPlant, true);
        pmanager.SetPlantStatus(newPlant2, true);
        pmanager.SetPlantStatus(newPlant3, true);

        Assert.IsTrue(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant.name));
        Assert.IsTrue(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant2.name));
        Assert.IsTrue(pmanager.PlantActive(newPlant3), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant3.name));

        Assert.AreEqual(pmanager.activePlants.Length, pmanager.maxActivePlants);
    }


    [Test]
    public void ActivatePlantTest4()
    {
        string name = "Joe's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Echeveria");

        pmanager.SetPlantStatus(newPlant, true);
        pmanager.SetPlantStatus(newPlant2, true);
        pmanager.SetPlantStatus(newPlant3, true);

        Assert.IsTrue(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant.name));
        Assert.IsTrue(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant2.name));
        Assert.IsTrue(pmanager.PlantActive(newPlant3), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant3.name));

        Assert.AreEqual(pmanager.activePlants.Length, pmanager.maxActivePlants);

        pmanager.SetPlantStatus(newPlant2, false);
        Assert.IsFalse(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was deactivated. Should be fale but got true", newPlant2.name));

    }


    [Test]
    public void ActivatePlantTest5()
    {
        string name = "Joe's plant";
        for (int i = 0; i < pmanager.maxActivePlants; i++)
        {
            GameObject newPlant = pmanager.MakePlant(name + "(" + i.ToString() + ")", "Aloe");
            pmanager.SetPlantStatus(newPlant, true);
        }

        GameObject newPlant4 = pmanager.MakePlant(name + "(" + pmanager.maxActivePlants.ToString() + ")", "Jade");
        try
        {
            pmanager.SetPlantStatus(newPlant4, true);
            Assert.Fail("Max Amount of plant's were added. Should not be able to make a new one");
        }
        catch (ArgumentException) { }
    }


    [Test]
    public void ActivatePlantTest6()
    {
        string name = "Joe's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");

        pmanager.SetPlantStatus(newPlant, true);
        pmanager.SetPlantStatus(newPlant2, true);

        Assert.IsTrue(pmanager.PlantActive(newPlant), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant.name));
        Assert.IsTrue(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant2.name));

        try
        {
            pmanager.SetPlantStatus(newPlant, true);
            Assert.Fail(newPlant.name + " was already activated. Should not be able to activate it again");
        }
        catch (ArgumentException) { }

        Assert.IsTrue(pmanager.PlantActive(newPlant2), String.Format("Plant '{0}' was activated. Should be true but got false", newPlant2.name));
    }


    [Test]
    public void ActivatePlantTest7()
    {
        string name = "Max's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Echeveria");

        pmanager.SetPlantStatus(newPlant, true);
        pmanager.SetPlantStatus(newPlant2, true);

        Assert.IsTrue(newPlant.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was activated. Timer should be active but it isnt", newPlant.name));
        Assert.IsTrue(newPlant2.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was activated. Timer should be active but it isnt", newPlant2.name));
        Assert.IsFalse(newPlant3.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was not activated. Timer should not be active but it is", newPlant3.name));
    }


    [Test]
    public void ActivatePlantTest8()
    {
        string name = "Max's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Echeveria");

        pmanager.SetPlantStatus(newPlant, true);
        pmanager.SetPlantStatus(newPlant3, true);

        Assert.IsTrue(newPlant3.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was activated. Timer should be active but it isnt", newPlant3.name));
        Assert.IsFalse(newPlant2.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was not activated. Timer should be active but it is", newPlant2.name));

        pmanager.SetPlantStatus(newPlant3, false);

        Assert.IsFalse(newPlant3.GetComponent<Timer>().getTicks, String.Format("Plant '{0}' was not activated. Timer should be active but it is", newPlant3.name));

    }



    [Test]
    public void RemovePlantTest1()
    {
        string name = "Erika's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        int count = pmanager.plantCollection.Count;

        pmanager.RemovePlant(newPlant);
        Assert.AreEqual(pmanager.plantCollection.Count, count - 1, String.Format("Plant was removed collection is still the same length"));
    }


    [Test]
    public void RemovePlantTest2()
    {
        string name = "Erika's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");

        pmanager.RemovePlant(newPlant);
        Assert.IsTrue(pmanager.plantCollection.Contains(newPlant2), String.Format("Plant '{0}' should still be in the collection", newPlant2.name));
        Assert.AreEqual(pmanager.plantCollection.Count, 1);
    }


    [Test]
    public void GetPlantIndexesTest1()
    {
        string name = "daffy's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Aloe");
        GameObject newPlant4 = pmanager.MakePlant(name + "(4)", "Aloe");


        pmanager.SetPlantStatus(newPlant3, true);
        pmanager.SetPlantStatus(newPlant2, true);
        pmanager.SetPlantStatus(newPlant4, true);

        int[] arr = pmanager.GetActivePlantIndexes();

        Assert.IsTrue( (arr[0] == 2 && arr[1] == 1 && arr[2] == 3) ,"3rd, 2nd and 4th plant were activated. They should come up in the that order");
    }


    [Test]
    public void GetPlantIndexesTest2()
    {
        string name = "daffy's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Aloe");
        GameObject newPlant4 = pmanager.MakePlant(name + "(4)", "Aloe");


        pmanager.SetPlantStatus(newPlant3, true);
        pmanager.SetPlantStatus(newPlant4, true);

        int[] arr = pmanager.GetActivePlantIndexes();

        Assert.IsTrue((arr[0] == 2 && arr[1] == 3 && arr[2] == -1), "3rd, 4th plant were activated. They should come up in the that order. The last should be -1");
    }

    [Test]
    public void GetPlantIndexesTest3()
    {
        string name = "daffy's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");
        GameObject newPlant3 = pmanager.MakePlant(name + "(3)", "Aloe");
        GameObject newPlant4 = pmanager.MakePlant(name + "(4)", "Aloe");


        pmanager.SetPlantStatus(newPlant3, true);
        pmanager.SetPlantStatus(newPlant4, true);
        pmanager.SetPlantStatus(newPlant, true);

        pmanager.RemovePlant(newPlant3);
        GameObject.DestroyImmediate(newPlant3);


        int[] arr = pmanager.GetActivePlantIndexes();

        //MonoBehaviour.print(arr[0].ToString() + arr[1].ToString() + arr[2].ToString());

        Assert.IsTrue((arr[0] == -1 && arr[1] == 2 && arr[2] == 0), String.Format("plant3 '{0}' was removed. The first index should be -1", newPlant.name));
    }


    [Test]
    public void BreedTest1()
    {
        string name = "John's plant";
        GameObject newPlant = pmanager.MakePlant(name, "Jade");
        GameObject newPlant2 = pmanager.MakePlant(name + "(2)", "Jade");

        GameObject child = pmanager.Breed(newPlant, newPlant2, "child");

        Assert.IsTrue(pmanager.plantCollection.Contains(child), String.Format("New child '{0}' was made but wasnt present in the collection", child.name));
        Assert.AreEqual(pmanager.plantCollection.Count, 3, String.Format("New child '{0}' was made but wasnt present in the collection", child.name));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlantManagerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
