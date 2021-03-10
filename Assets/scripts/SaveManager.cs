//used to store data as json string, was going to be used to create save files

using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{

    private List<GameObject> plantCollection;

    [SerializeField]
    public Button saveButton;
    private string filePath;


    public class PlantObject {
        public string Name { get; set; }
        public List<float> Attributes { get; set; }
    }

    public class SaveObject {
        public List<PlantObject> plantObjects { get; set; }
    }

    void Start(){
        filePath = Application.persistentDataPath + "/save.json";
    }

    //get all data to be saved
    void Update() {
        plantCollection = GetComponent<PlantManager>().plantCollection;
    }

    float getVal(GameObject plant, string dep) {
        return plant.transform.Find("Dependencies/" + dep).GetComponent<DependenceAttribute>().currValue;
    }

    string SaveData(){
        List<PlantObject> plants = new List<PlantObject>();

        //get all attributes of all plants in list 
        for(int i=0; i < plantCollection.Count; i++) {
            string name = plantCollection[i].name;

            //get plant's' attributes
            float lighting = getVal(plantCollection[i], "Lighting");
            float temp = getVal(plantCollection[i], "Temperature");
            float water = getVal(plantCollection[i], "Water");
            float fertiliser = getVal(plantCollection[i], "Fertiliser");

            List<float> depVals = new List<float>();
            depVals.Add(lighting);
            depVals.Add(temp);
            depVals.Add(water);
            depVals.Add(fertiliser);

            PlantObject plantObject = new PlantObject{
                Name = name,
                Attributes = depVals,
            };

            plants.Add(plantObject);
        };

        SaveObject saveObject = new SaveObject {
            plantObjects = plants,
        };

        return JsonConvert.SerializeObject(saveObject,Formatting.Indented);
    }

    public void Save() {
        string json = SaveData();
        File.WriteAllText(filePath,json);       //will create a file if it doesnt exist
        // LoadFile();
    }

    public void LoadFile(){

        string json = "";

        // Open the stream and read it back.
        using (StreamReader sr = File.OpenText(filePath))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                json += s;
            }
        }        

        // Debug.Log(json);

        //deserialize the object
        SaveObject saveObject = JsonConvert.DeserializeObject<SaveObject>(json);

        for(int i=0; i < saveObject.plantObjects.Count; i++) {
            Debug.Log(saveObject.plantObjects[i].Name);
        }
        

    }


}
