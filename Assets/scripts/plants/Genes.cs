using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using MiscFunctions;

[Serializable]
public class Gene
{
    public string name;
    //2 chars, uppercase == dominant, lowercase == recessive
    public string allelePair;
    public string valueType;
    public string dominantVal;
    public string recessiveVal;

    private static readonly string[] acceptedTypes = { "float", "string", "bool", "int", "Color", "Vector2" };


    public Gene(string name, string allelePair, string valueType, string dominantVal, string recessiveVal)
    {
        this.name = name;
        if (allelePair.Length != 2)
        {
            throw new ArgumentException(String.Format("The gene '{0}' allele pair must have only 2 characters. Got '{1}'", name, allelePair));
        }

        if (allelePair.Any(char.IsDigit))
        {
            throw new ArgumentException(String.Format("The gene '{0}' allele pair must only have alphabetical characters. Got '{1}'", name, allelePair));
        }

        this.allelePair = allelePair;

        if (!acceptedTypes.Contains(valueType))
        {
            throw new ArgumentException(String.Format("'{0}' has an unsupported type '{1}'!", name, valueType));
        }

        this.valueType = valueType;

        this.dominantVal = dominantVal;
        this.recessiveVal = recessiveVal;
    }

    public T GetValue<T>()
    {
        //get the value of the gene. If any allele is dominant then
        //dominant val is returned, else the recessive val is returned

        if (allelePair.Any(ch => char.IsUpper(ch)))
        {
            return (T)Convert.ChangeType(dominantVal, typeof(T));
        }
        else
        {
            return (T)Convert.ChangeType(recessiveVal, typeof(T));
        }
    }

    public T AlleleToValue<T>(char allele)
    {
        //get the value of specific allele. If any allele is dominant then
        //dominant val is returned, else the recessive val is returned
        if (!allelePair.Contains(allele))
        {
            throw new ArgumentException(String.Format("'{0}' is not an allele of {1}", allele, name));
        }

        if (char.IsUpper(allele))
        {
            return (T)Convert.ChangeType(dominantVal, typeof(T));
        }
        else
        {
            return (T)Convert.ChangeType(recessiveVal, typeof(T));
        }
    }

    public static bool Homozygous(string allelePair)
    {
        char allele1 = allelePair.ElementAt(0);
        char allele2 = allelePair.ElementAt(1);
        return (char.IsUpper(allele1) && char.IsUpper(allele2) || char.IsLower(allele1) && char.IsLower(allele2));
    }

    //GetValueMix mixes the 2 values into one, depending on the specified type
    public static float GetValueMix(float val1, float val2, float offsetPercentage)
    {
        return val1 + ((val2 - val1) * offsetPercentage);
    }

    public static int GetValueMix(int val1, int val2, int offset)
    {
        int out_;
        if (Math.Abs(val2 - val1) <= 1)
        {
            //choose 50 50 since there's no inbetween
            int rInt = Random.Range(0, 2);
            if (rInt == 0)
                out_ = val1;
            else
                out_ = val2;
        }
        else
        {
            out_ = val1 + ((val2 - val1) / offset);
        }

        return out_;
    }

    public static Color GetValueMix(Color val1, Color val2, float offsetPercentage)
    {
        return NumOp.GetColorBlend(val1, val2, offsetPercentage);
    }

    public static Vector2 GetValueMix(Vector2 val1, Vector2 val2, float offsetPercentage)
    {
        return new Vector2(GetValueMix(val1[0], val2[0], offsetPercentage), GetValueMix(val1[1], val2[1], offsetPercentage));
    }

    public static bool GetValueMix(bool val1, bool val2)
    {
        int rint = Random.Range(0, 2);
        if (rint == 0)
        {
            return val1;
        }
        else
        {
            return val2;
        }
    }

    public static string GetValueMix(string val1, string val2)
    {
        int rint = Random.Range(0, 2);
        if (rint == 0)
        {
            return val1;
        }
        else
        {
            return val2;
        }
    }

    public Gene CrossGene(Gene gene2, string newGeneName)
    {
        //Takes this and another gene and crosses them to get a mix of the 2 genes
        //alleles form this and gene2 are passed down randomly.

        if (valueType != gene2.valueType)
        {
            throw new ArgumentException(String.Format("The value type of '{0}' and  '{1}' do not match", valueType, gene2.valueType));
        }

        //Get one allele from both genes
        int rInt = Random.Range(0, 2);
        char allele1 = allelePair.ElementAt(rInt);

        rInt = Random.Range(0, 2);
        char allele2 = gene2.allelePair.ElementAt(rInt);

        char[] alleles = { allele1, allele2 };

        string newAllelePair = new string(alleles);
        string newDominantVal;
        string newRecessiveVal;


        //mix the dominant of the 2 genes, and the recessives.
        //get the halfway points depending on the type of value
        //set new dominant and recessive values as these 2 halfway points
        if (valueType == "float")
        {
            float dom1 = float.Parse(dominantVal);
            float dom2 = float.Parse(gene2.dominantVal);
            float rec1 = float.Parse(recessiveVal);
            float rec2 = float.Parse(gene2.recessiveVal);

            newDominantVal = GetValueMix(dom1, dom2, 0.5f).ToString();
            newRecessiveVal = GetValueMix(rec1, rec2, 0.5f).ToString();
        }
        else if (valueType == "int")
        {
            int dom1 = Int32.Parse(dominantVal);
            int dom2 = Int32.Parse(gene2.dominantVal);
            int rec1 = Int32.Parse(recessiveVal);
            int rec2 = Int32.Parse(gene2.recessiveVal);

            newDominantVal = GetValueMix(dom1, dom2, 2).ToString();
            newRecessiveVal = GetValueMix(rec1, rec2, 2).ToString();
        }

        else if (valueType == "string")
        {
            newDominantVal = GetValueMix(dominantVal, gene2.dominantVal);
            newRecessiveVal = GetValueMix(recessiveVal, gene2.recessiveVal);
        }

        else if (valueType == "bool")
        {
            bool dom1 = bool.Parse(dominantVal);
            bool dom2 = bool.Parse(gene2.dominantVal);
            bool rec1 = bool.Parse(recessiveVal);
            bool rec2 = bool.Parse(gene2.recessiveVal);

            newDominantVal = GetValueMix(dom1, dom2).ToString();
            newRecessiveVal = GetValueMix(rec1, rec2).ToString();
        }

        else if (valueType == "Color")
        {
            //get halfway color between the genes
            Color dom1;
            ColorUtility.TryParseHtmlString(dominantVal, out dom1);
            Color dom2;
            ColorUtility.TryParseHtmlString(gene2.dominantVal, out dom2);

            Color rec1;
            ColorUtility.TryParseHtmlString(recessiveVal, out rec1);
            Color rec2;
            ColorUtility.TryParseHtmlString(gene2.recessiveVal, out rec2);

            newDominantVal = "#" + ColorUtility.ToHtmlStringRGB( GetValueMix(dom1, dom2, 0.5f) );

            newRecessiveVal = "#" + ColorUtility.ToHtmlStringRGB( GetValueMix(rec1, rec2, 0.5f) );
        }

        else if (valueType == "Vector2")
        {
            Vector2 dom1 = Parse.Vec2(dominantVal);
            Vector2 dom2 = Parse.Vec2(gene2.dominantVal);
            Vector2 rec1 = Parse.Vec2(recessiveVal);
            Vector2 rec2 = Parse.Vec2(gene2.recessiveVal);

            //Vec.ToString() rounds up each value to 1 decimal place so I have to make the string directly
            Vector2 tmpDom = GetValueMix(dom1, dom2, 0.5f);
            newDominantVal = String.Format("({0}, {1})", tmpDom[0], tmpDom[1]);

            Vector2 tmpRec = GetValueMix(rec1, rec2, 0.5f);
            newRecessiveVal = String.Format("({0}, {1})", tmpRec[0], tmpRec[1]);

            if (name == "leafScale")
            {
                Debug.Log("mixing leafscale");
                Debug.Log(newDominantVal);
                Debug.Log(dom1);
                Debug.Log(dom2);

            }
        }

        else
        {
            throw new ArgumentException( String.Format("'{0}' has an unsupported type '{1}'!", name, valueType) );
        }

        //if (Homozygous(newAllelePair))
        //{
        //    Debug.Log("mixing " + name);
        //    Debug.Log(String.Format("p1 - Dom: {0}, Rec: {1} | p2 - Dom: {2}, Rec: {3}", dominantVal, recessiveVal, gene2.dominantVal, gene2.recessiveVal));
        //    Debug.Log("newDom: " + newDominantVal + ", newRec: " + newRecessiveVal);
        //}

        return new Gene(newGeneName, newAllelePair, valueType, newDominantVal, newRecessiveVal);
    }

}

public class Genes : MonoBehaviour
{
    //holds all the information (variables) for other components in string format
    public List<Gene> genes;

    void Start()
    {
        List<Gene> new_ = new List<Gene>();
        foreach(Gene gene in genes)
        {
            try
            {
                new_.Add(new Gene(gene.name, gene.allelePair, gene.valueType, gene.dominantVal, gene.recessiveVal));
            }
            catch (ArgumentException e)
            {
                genes.Clear();
                throw e;
            }
        }

        genes.Clear();
        genes = new_;
    }
    

    public void SetGene(Gene newGene)
    {
        // add/replace specified gene
        bool found = false;
        int i = 0;
        foreach (Gene gene in genes)
        {
            if (gene.name == name)
            {
                found = true;
            }
            if (!found)
                i++;
        }

        if (found)
        {
            genes[i] = newGene;
        }
        else
        {
            genes.Add(newGene);
        }
    }

    public void SetGene(string name, string allelePair, string valueType, string dominantVal, string recessiveVal)
    {
        bool found = false;
        int i = 0;
        foreach (Gene gene in genes)
        {
            if (gene.name == name)
            {
                found = true;
            }
            if (!found)
                i++;
        }

        Gene newGene = new Gene(name, allelePair, valueType, dominantVal, recessiveVal);

        if (found)
        {
            genes[i] = newGene;
        }
        else
        {
            genes.Add(newGene);
        }
    }

    public Gene GetGene(string name)
    {
        Gene out_ = null;
        foreach (Gene gene in genes)
        {
            if (gene.name == name)
            {
                out_ = gene;
            }
        }

        if (out_ == null)
        {
            throw new ArgumentException(String.Format("the gene '{0}' could not be found in gene script", name), name);
        }

        return out_;
    }



    public T GetValue<T>(string name)
    {
        Gene gene = GetGene(name);
        //check if name found, if not will error
        return gene.GetValue<T>();
    }

    public Genes CrossGenes(Genes other, Genes out_)
    {
        //crosses all genes on object and puts result into out_
        List<Gene> newGenes = new List<Gene>();
        foreach (Gene gene in genes)
        {
            try
            {
                Gene matching = other.GetGene(gene.name);
                newGenes.Add( gene.CrossGene(matching, gene.name) );
            }
            catch (ArgumentException e)
            {
                print(e);
                Debug.LogWarning(String.Format("'{0}' Could not find a matching gene for '{1}' in '{2}', skipping...", name, gene.name, other.gameObject.name), gameObject);
            }
        }

        out_.genes.Clear();
        foreach (Gene gene in newGenes)
        {
            out_.SetGene(gene);
        }

        return out_;
    }


    // Update is called once per frame
    void Update()
    {
    }
}