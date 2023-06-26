using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelGenerator : MonoBehaviour {
    [SerializeField] bool generate;
    [SerializeField] int sectionsToGenerate = 4;
    [SerializeField] GameObject startingSection, endingSection;
    [SerializeField] List<GameObject> sections = new List<GameObject>();
    Vector2 lastPos;
    Transform player => FindObjectOfType<PlayerController>().transform;

    private void Update()
    {
        if (generate) {
            generate = false;
            PlaceLevel();
        }
    }

    public void GenerateLevel(int levelNum)
    {
        transform.position = player.position + (Vector3) Vector2.down * 10;
        PlaceLevel(levelNum);
    }

    void PlaceLevel(int levelNum = -1)
    {
        lastPos = transform.position;
        DeleteChildren();
        
        lastPos = PlaceSection(startingSection);
        for (int i = 0; i < sectionsToGenerate; i++) {
            var chosenSection = ChoseNextSection();
            lastPos = PlaceSection(chosenSection);
        }
        lastPos = PlaceSection(endingSection, levelNum);
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
        return newSection.GetComponent<LevelSection>().endPoint.position;
    }
}