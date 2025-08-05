using UnityEngine;
using System;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    // Inventory lists
    private List<string> taskList = new List<string>();
    private List<string> petFoodList = new List<string>();
    private List<string> accessoryList = new List<string>();

    // Item pools for random rewards
    private string[] foodItems = { "Apple", "Fish", "Bone", "Carrot", "Cake" };
    private string[] accessoryItems = { "Hat", "Glasses", "Scarf", "Bowtie", "Collar" };

    public event Action OnInventoryUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTask(string taskName)
    {
        taskList.Add(taskName);
        SaveInventory();
        OnInventoryUpdated?.Invoke();
    }

    public void AddPetFood(string foodName)
    {
        petFoodList.Add(foodName);
        SaveInventory();
        OnInventoryUpdated?.Invoke();
    }

    public void AddAccessory(string accessoryName)
    {
        accessoryList.Add(accessoryName);
        SaveInventory();
        OnInventoryUpdated?.Invoke();
    }

    public void AddRandomItem()
    {
        int rewardType = UnityEngine.Random.Range(0, 2);

        if (rewardType == 0 && foodItems.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, foodItems.Length);
            AddPetFood(foodItems[index]);
        }
        else if (accessoryItems.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, accessoryItems.Length);
            AddAccessory(accessoryItems[index]);
        }
    }

    // Public accessors
    public List<string> TaskList => taskList;
    public List<string> PetFoodList => petFoodList;
    public List<string> AccessoryList => accessoryList;

    // Save and load using PlayerPrefs
    private void SaveInventory()
    {
        PlayerPrefs.SetString("Tasks", string.Join(",", taskList.ToArray()));
        PlayerPrefs.SetString("PetFood", string.Join(",", petFoodList.ToArray()));
        PlayerPrefs.SetString("Accessories", string.Join(",", accessoryList.ToArray()));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("Tasks"))
        {
            string tasks = PlayerPrefs.GetString("Tasks");
            if (!string.IsNullOrEmpty(tasks))
            {
                taskList = new List<string>(tasks.Split(','));
            }
        }

        if (PlayerPrefs.HasKey("PetFood"))
        {
            string food = PlayerPrefs.GetString("PetFood");
            if (!string.IsNullOrEmpty(food))
            {
                petFoodList = new List<string>(food.Split(','));
            }
        }

        if (PlayerPrefs.HasKey("Accessories"))
        {
            string accessories = PlayerPrefs.GetString("Accessories");
            if (!string.IsNullOrEmpty(accessories))
            {
                accessoryList = new List<string>(accessories.Split(','));
            }
        }
    }

    public void ClearInventory()
    {
        taskList.Clear();
        petFoodList.Clear();
        accessoryList.Clear();
        SaveInventory();
        OnInventoryUpdated?.Invoke();
    }
}


