﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Procution of water (well)
 Produces: water
    
 Production of food (farm)
 Consumes: water
 Produces: food
 
 Production of population (house)
 Consumes: water and food
 Produces: people
 
 

 Production of building materials (WIP)
 Consumes: steel, concrete
 Produces: building materials*/

public enum Resource { Food, Water, People }


public class ResourceStockpile
{
    // Current amount of resources currently in stockpile
    public float m_amount { get; private set; }
    // Maximum amount of resources in stockpile
    public float m_max { get; private set; }

    public ResourceStockpile(float max)
    {
        m_amount = 0;
        m_max = max;
    }
    ///* Updates amount of a resource and returns whatever was left over
    // * if stockpile was e*/
    public float M_ChangeAmount(float change)
    {
        float leftOver = 0;
        float newAmount = m_amount + change;
        if (newAmount < 0)
        {
            leftOver = -1 * newAmount;
            newAmount = 0;
        }
        else if (newAmount > m_max)
        {
            leftOver = newAmount - m_max;
            newAmount = m_max;
        }
        m_amount = newAmount;
        return leftOver;
    }
};
public class ResourceProcess
{
    public ResourceProcess(Dictionary<Resource, float> resourcesConsumed, Dictionary<Resource, float> resourcesProduced)
    {
        m_resourcesConsumed = resourcesConsumed;
        m_resourcesProduced = resourcesProduced;
    }
    // Resource mapped to amount of resource consumed by this process to produce
    protected Dictionary<Resource, float> m_resourcesConsumed;
    // Resource mapped to amount of resource produced by this process
    protected Dictionary<Resource, float> m_resourcesProduced;
    // Performs the process, consuming goods and producing
    public void Execute(ref Dictionary<Resource, ResourceStockpile> cityStockpiles)
    {
        bool failed = false;
        // First make sure that the resources needed exist
        foreach (var item in m_resourcesConsumed)
        {
            if (cityStockpiles[item.Key].m_amount - item.Value * Time.deltaTime >= 0)
            {
                // Check passed
                continue;
            }
            else
            {
                // Check didn't pass, don't produce
                failed = true;
            }
        }
        // Then make sure that produced goods actually fits
        foreach (var item in m_resourcesProduced)
        {
            if (cityStockpiles[item.Key].m_amount + item.Value * Time.deltaTime <= cityStockpiles[item.Key].m_max)
            {
                // Check passed
                continue;
            }
            else
            {
                // Check didn't pass, don't produce
                failed = true;
            }
        }
        if (failed)
        {
            return;
        }
        // Then consume the resources
        foreach (var item in m_resourcesConsumed)
        {
            cityStockpiles[item.Key].M_ChangeAmount(-item.Value * Time.deltaTime);
        }
        // Finally add the produced resources
        foreach (var item in m_resourcesProduced)
        {
            cityStockpiles[item.Key].M_ChangeAmount(item.Value * Time.deltaTime);
        }
    }
}

public class City : MonoBehaviour
{
    // This will have to be put in some sort of list or something
    public GameObject m_wellPrefab;
    public GameObject m_farmPrefab;

    // List of all buildings in this city
    private List<GameObject> m_buildings = new List<GameObject>();
    // Resource type mapped to amount of said resource currently present in city
    private Dictionary<Resource, ResourceStockpile> m_resourceStockpiles = new Dictionary<Resource, ResourceStockpile>();


    // DEBUG stuff
    public float food = 0;
    public float water = 0;
    public float people = 0;

    // Use this for initialization
    void Start()
    {
        // Create all stockpiles with 0 value
        foreach (Resource res in System.Enum.GetValues(typeof(Resource)))
        {
            m_resourceStockpiles[res] = new ResourceStockpile(4);
        }

        // Create some buildings
        m_buildings.Add(Instantiate(m_wellPrefab, transform.position + new Vector3(0, 0, 1), transform.rotation, transform));
        m_buildings.Add(Instantiate(m_farmPrefab, transform.position + new Vector3(0, 0, -1), transform.rotation, transform));
    }

    // Update is called once per frame
    void Update()
    {
        // Update city stockpile with building consumption and production
        foreach (GameObject buildingObj in m_buildings)
        {
            Building building = buildingObj.GetComponent<Building>();
            if (building)
            {
                building.Produce(ref m_resourceStockpiles);
            }
            else
            {
                Debug.LogError("building object in city doesn't have building script");
            }
        }

        food = m_resourceStockpiles[Resource.Food].m_amount;
        water = m_resourceStockpiles[Resource.Water].m_amount;
        people = m_resourceStockpiles[Resource.People].m_amount;
    }
}



//public class ResourceCache
//{
//    Resource m_resource;
//    float m_currentAmount;
//    float m_maxAmount;

//    public ResourceCache(Resource resource, float m_maxAmount)
//    {
//        m_resource = resource;
//        m_currentAmount = 0;
//    }

//    /* Updates amount of a resource and returns whatever was left over
//     * if stockpile was e*/
//    public float M_changeAmount(float change)
//    {
//        float leftOver = 0;
//        float newAmount = m_currentAmount - change;
//        if(newAmount < 0)
//        {
//            leftOver = -1 * newAmount;
//            newAmount = 0;
//        }
//        else if (newAmount > m_maxAmount)
//        {
//            leftOver = newAmount - m_maxAmount;
//            newAmount = m_maxAmount;
//        }
//        return leftOver;
//    }
//};