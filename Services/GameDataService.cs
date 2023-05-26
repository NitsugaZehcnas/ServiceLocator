using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Save the data of the entire game
/// </summary>
public class GameDataService
{
    public GameDataService(GameService _service) => gameService = _service;

    #region Variables

    private GameService gameService;

    public GameData[] SlotData { get; private set; }
    public GameData CurrentData => SlotData[CurrentSlotIndex];
    public int CurrentSlotIndex { get; private set; }

    private string folderDataPath;

    #endregion

    #region Base Methods

    public void Initialize()
    {
        folderDataPath = $"{Application.persistentDataPath}/Saves/";

        SlotData = new GameData[3];

        for (int i = 0; i < 3; i++) LoadGameData(i);
    }

    #endregion

    #region GameDataService Methods

    public void SaveCurrentGameData()
    {
        string fileName = string.Format("/{0}.dat", "GameData_" + CurrentSlotIndex);

        if (!Directory.Exists(folderDataPath)) Directory.CreateDirectory(folderDataPath);

        BinaryFormatter bf = new();
        FileStream file = File.Create(folderDataPath + fileName);

        bf.Serialize(file, CurrentData);
        file.Close();
    }

    public void DeleteGameData(int gameSlot)
    {
        string fileName = string.Format("/{0}.dat", "GameData_" + gameSlot);

        if (!CheckGameData(gameSlot)) return;

        File.Delete(folderDataPath + fileName);
        SlotData[gameSlot] = new();
    }

    private void LoadGameData(int _slotIndex)
    {
        string fileName = string.Format("/{0}.dat", "GameData_" + CurrentSlotIndex);
        string path = folderDataPath + fileName;

        if (!CheckGameData(_slotIndex))
        {
            SlotData[_slotIndex] = new();
            return;
        }

        BinaryFormatter _bf = new();
        FileStream _file = File.Open(path, FileMode.Open);
        SlotData[_slotIndex] = (GameData)_bf.Deserialize(_file);
        _file.Close();
    }

    public void SetCurrentSlotIndex(int index) => CurrentSlotIndex = index;

    public bool CheckGameData(int gameSlot)
    {
        string fileName = string.Format("/{0}.dat", "GameData_" + gameSlot);
        return Directory.Exists(folderDataPath) && File.Exists(folderDataPath + fileName);
    }

    #endregion
}

//TODO: Crear un script solamente para esta clase
[Serializable]
public class GameData
{
    //Player info
    public int playerLife;
    public float playerStamina;
    public int amountOfJumps = 1;
    public int amountOfThrows = 1;

    //Spawn info
    public string spawnScene = "01_01";
    public float xSpawnPos = -6f;
    public float ySpawnPos = -4.485f;

    //Room info
    public Dictionary<string, RoomSaveInfo> roomDictionary = new();
}
