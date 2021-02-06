using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class Gene
{
    public string name;
    //2 chars, uppercase == dominant, lowercase == recessive
    public string allelePair;
    public string valueType;
    public string dominantVal;
    public string recessiveVal;

    public Gene(string name, string allelePair, string valueType, string dominantVal, string recessiveVal)
    {
        this.name = name;
        this.allelePair = allelePair;
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
        

    public Gene CrossGene(Gene gene2, string newGeneName)
    {
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

        if (char.IsUpper(allele1) && char.IsUpper(allele2))
        {
            //set dominant as allele 1 val, and recessive as allele 2 val
            newDominantVal = AlleleToValue<string>(allele1);
            newRecessiveVal = AlleleToValue<string>(allele2);
        }
        else
        {
            //set dominant allele as dominant val and vice versa
            if (char.IsUpper(allele1))
            {
                newDominantVal = AlleleToValue<string>(allele1);
                newRecessiveVal = AlleleToValue<string>(allele2);
            }
            else
            {
                newRecessiveVal = AlleleToValue<string>(allele1);
                newDominantVal = AlleleToValue<string>(allele2);
            }
        }

        return new Gene(newGeneName, newAllelePair, valueType, newDominantVal, newRecessiveVal);
    }

}

public class Genes : MonoBehaviour
{
    //holds all the information (variables) for other components in string format
    public List<Gene> genes;

    void Start()
    {
        for (int i=0; i < genes.Count; i++)
        {
            try
            {
                CheckGene(genes[i]);
            }
            catch (Exception e)
            {
                print(e);
                genes.RemoveAt(i);
            }
        }
    }
    
    void CheckGene(Gene gene)
    {
        if (gene.allelePair.Length != 2)
        {
            throw new Exception(String.Format("The gene '{0}' allele pair must have only 2 characters. Got '{1}'", gene.name, gene.allelePair));
        }

        if (gene.allelePair.Any(char.IsDigit))
        {
            throw new Exception(String.Format("The gene '{0}' allele pair must only have alphabetical characters. Got '{1}'", gene.name, gene.allelePair));
        }
    }

    public void SetGene(Gene newGene)
    {
        CheckGene(newGene);

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

        CheckGene(newGene);

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

        CheckGene(out_);

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
        out_.genes.Clear();
        foreach (Gene gene in genes)
        {
            try
            {
                Gene matching = other.GetGene(gene.name);
                out_.SetGene( gene.CrossGene(matching, gene.name) );
            }
            catch (ArgumentException e)
            {
                print(e);
                Debug.LogWarning(String.Format("'{0}' Could not find a matching gene for '{1}' in '{2}', skipping...", name, gene.name, other.gameObject.name), gameObject);
            }
        }
        return out_;
    }

    // Update is called once per frame
    void Update()
    {
    }
}