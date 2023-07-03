using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class LootTable
    {
        [System.Serializable] 
        public class Entry
        {
            public int ItemID;
            [Range(0, 1)] public float Rarity;
            [HideInInspector] public float rarityThreshold;
        }

        [HideInInspector] public string name;
        public string tableName;
        [HideInInspector ] public int ID;
        [SerializeField] List<Entry> Entries = new List<Entry>();

        public int GetItem()
        {
            float totalRarity = 0;
            foreach (var e in Entries) {
                totalRarity += e.Rarity;
                e.rarityThreshold = totalRarity;
            }
            float roll = Random.Range(0, totalRarity);

            for (int i = 0; i < Entries.Count; i++) {
                if (roll < Entries[i].rarityThreshold) return Entries[i].ItemID;
            }
            return -1;
        }
    }

    [System.Serializable]
    public class ItemID
    {
        [HideInInspector] public string name;
        [HideInInspector] public int ID;
        public GameObject prefab;
    }

    public static GameManager i;
    private void Awake() {i = this; }

    [SerializeField] List<LootTable> lootTables = new List<LootTable>();
    [SerializeField] List<ItemID> lootPrefabs = new List<ItemID>();
    [SerializeField] GameObject gunPickupPrefab;

    [Header("XP")]
    [SerializeField] GameObject xpPrefab;
    [SerializeField] Vector2 xOrbSpeedRange, yOrbSpeedRange;

    [Space()]
    [SerializeField] float difficultIncreasePeriod;
    float difficultIncreaseCooldown;
    public int maxEnemies = 3;

    [Header("restart")]
    [SerializeField] int sceneNum = 2;
    [SerializeField] int victorySceneID = 5;

    [Space()]
    [SerializeField] GameObject pauseMenu;
    private bool paused = true;
    public bool isPaused(){ return paused; }
    
    [Header("Level and gameOver")]
    public bool gameOver = false;
    [SerializeField] GameObject gameOverCanvas, openingStoryCanvas;
    [SerializeField] int numLevels;
    public int currentLevel;

    LevelGenerator levelgen => FindObjectOfType<LevelGenerator>();
    PlayerCombat pCombat => FindAnyObjectByType<PlayerCombat>();
    PlayerXP pXP => FindAnyObjectByType<PlayerXP>();


    public List<GameObject> enemies;

    public void SpawnGunPickup(Gun gun, Vector3 pos)
    {
        var newGun = Instantiate(gunPickupPrefab, pos, Quaternion.identity);
        newGun.GetComponentInChildren<Pickup>().Init(gun, 2);
    }

    private void OnValidate()
    {
        for (int i = 0; i < lootPrefabs.Count; i++) {
            lootPrefabs[i].ID = i;
            if (lootPrefabs[i].prefab != null) lootPrefabs[i].name = i + ": " + lootPrefabs[i].prefab.name;
        }
        for (int i = 0; i < lootTables.Count; i++) {
            lootTables[i].ID = i;
            lootTables[i].name = i + ": " + lootTables[i].tableName;
        }
    }

    public void SpawnXP(float amount, Vector2 pos)
    {
        while (amount > 1) {
            float _amount = Random.Range(1, amount);
            amount -= _amount;
            SpawnXPorb(_amount, pos);
        }
        SpawnXPorb(1, pos);
    }

    void SpawnXPorb(float amount, Vector2 pos)
    {
        var newOrb = Instantiate(xpPrefab, pos, Quaternion.identity, transform);
        newOrb.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(xOrbSpeedRange.x, xOrbSpeedRange.y), Random.Range(yOrbSpeedRange.x, yOrbSpeedRange.y)));
        newOrb.GetComponent<xpPickup>().Init(pXP, amount);
    }

    public void SpawnLoot(int lootTableID, Vector3 position)
    {
        GameObject obj = null;
        for (int i = 0; i < lootTables.Count; i++) {
            if (lootTables[i].ID == lootTableID) {
                int itemID = lootTables[i].GetItem();
                for (int j = 0; j < lootPrefabs.Count; j++) {
                    if (lootPrefabs[j].ID == itemID) obj = lootPrefabs[j].prefab;
                }
            }
        }
        if (obj != null) Instantiate(obj, position, Quaternion.identity);
    }
 
    private void Start(){
        enemies = new List<GameObject>();

        if (FindObjectOfType<LevelGenerator>() == null) {
            Resume();
            return;
        }

        openingStoryCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (pCombat.isDead){
            //fill in later if necessary            
        }


        if(Input.GetKeyDown(KeyCode.Escape) && !gameOver) {
            if (isPaused()){
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void CompleteLevel(int levelNum)
    {
        currentLevel = levelNum;
        if (levelNum == numLevels) {
            SceneManager.LoadScene(victorySceneID);
            return;
        }
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        Pause();
        levelgen.GenerateLevel(currentLevel + 1);
        pCombat.FullHeal();
        Resume();
    }

    public void GameOver(){
        gameOver = true;
        gameOverCanvas.SetActive(true);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(1);
        SceneManager.LoadScene(sceneNum, LoadSceneMode.Additive);
    }

    public void Pause(bool showPauseMenu = true)
    {
        print("game paused");
        paused = true;
        Time.timeScale = 0f;
        if (showPauseMenu) pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        paused = false;
        pauseMenu.SetActive(false);
        openingStoryCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene(0);
        paused = false;
        Time.timeScale = 1f;
    }
}
