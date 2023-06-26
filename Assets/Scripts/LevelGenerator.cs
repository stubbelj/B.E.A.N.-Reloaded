using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelGenerator : MonoBehaviour {
    [SerializeField] bool generate;
    [SerializeField] int sectionsToGenerate = 4;
    int sectionsPlaced = 0;
    [SerializeField] GameObject startingSection, endingSection;
    [SerializeField] List<GameObject> sections = new List<GameObject>();
    Vector2 lastPos;
    Transform player => FindObjectOfType<PlayerController>().transform;
    int levelNum;    

    private void Update()
    {
        if (generate) {
            generate = false;
            PlaceLevel();
        }
    }

    private void Start()
    {
        PlaceLevel(0);
    }

    public void GenerateLevel(int levelNum)
    {
        var pos = lastPos;
        pos.x = player.transform.position.x;
        transform.position = pos;
        this.levelNum = levelNum;
        PlaceLevel();
    }

    void PlaceLevel(int levelNum = -1)
    {
        lastPos = transform.position;
        DeleteChildren();
        sectionsPlaced = 0;
            
        lastPos = PlaceSection(startingSection);
        PlaceNextSection();        
    }

    public void PlaceNextSection()
    {
        if (sectionsPlaced > sectionsToGenerate) return;

        var chosenSection = ChoseNextSection();
        lastPos = PlaceSection(chosenSection);
        sectionsPlaced += 1;

        if (sectionsPlaced == sectionsToGenerate) {
            lastPos = PlaceSection(endingSection, levelNum);
        }
    }

    void DeleteChildren()
    {
        if (Application.isPlaying) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
        else {
            for (int i = 0; i < transform.childCount; i++) {
                DestroyImmediate(transform.GetChild(i).gameObject);
                i -= 1;
            }
        }   
    }

    GameObject ChoseNextSection()
    {
        return sections[Random.Range(0, sections.Count)];
    }

    Vector3 PlaceSection(GameObject prefab, int levelNum = -1)
    {
        var newSection = Instantiate(prefab, lastPos, Quaternion.identity, transform);
        if (levelNum != -1) newSection.GetComponentInChildren<LevelEnd>().levelNum = levelNum;
        newSection.GetComponent<LevelSection>().levelGen = this;
        newSection.GetComponent<LevelSection>().player = player;

        return newSection.GetComponent<LevelSection>().endPoint.position;
    }
}