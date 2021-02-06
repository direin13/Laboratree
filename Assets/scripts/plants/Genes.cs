using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Gene
{
    public string name;
    //2 chars, uppercase == dominant, lowercase == recessive
    public string allelePair;
    public string dominantVal;
    public string recessiveVal;

    public Gene(string name, string allelePair, string dominantVal, string recessiveVal)
    {
        this.name = name;
        this.allelePair = allelePair;
        this.dominantVal = dominantVal;
        this.recessiveVal = recessiveVal;
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

    public void SetGene(string name, string allelePair, string dominantVal, string recessiveVal)
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

        Gene newGene = new Gene(name, allelePair, dominantVal, recessiveVal);

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
        foreach (Gene pair in genes)
        {
            if (pair.name == name)
            {
                out_ = pair;
            }
        }

        if (out_ == null)
        {
            throw new Exception(String.Format("the gene '{0}' could not be found in gene script", name));
        }

        CheckGene(out_);

        return out_;
    }

    public string GetValue(string name)
    {
        Gene gene = GetGene(name);
        //check if name found, if not error f
        if (gene.allelePair.Any(ch => char.IsUpper(ch)))
        {
            //dominant
            return gene.dominantVal;
        }
        else
        {
            return gene.recessiveVal;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}