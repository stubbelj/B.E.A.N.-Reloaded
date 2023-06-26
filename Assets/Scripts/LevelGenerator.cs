using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelGenerator : MonoBehaviour {
    [SerializeField] bool generate;
    [SerializeField] int sectionsToGenerate = 4;

    GameObject startingSection, endingSection;

    [SerializeField] List<GameObject> sections = new List<GameObject>();
    Vector2 lastPos;

    private void Update()
    {
        if (generate) {
            generate = false;
            GenerateLevel();
        }
    }

    void GenerateLevel()
    {
        lastPos = transform.position;
        DeleteChildren();
        PlaceSection(startingSection);
        for (int i = 0; i < sectionsToGenerate; i++) {
            var chosenSection = ChoseNextSection();
            lastPos = PlaceSection(chosenSection);
        }
        PlaceSection(endingSection);
    }

    void DeleteChildren()
    {
        for (int i = 0; i < transform.childCount; i++) {
            if (Application.isPlaying) Destroy(transform.GetChild(i).gameObject);
            else DestroyImmediate(transform.GetChild(i).gameObject);
            i -= 1;
        }
    }

    GameObject ChoseNextSection()
    {
        return sections[Random.Range(0, sections.Count)];
    }

    Vector3 PlaceSection(GameObject prefab)
    {
        var newSection = Instantiate(prefab, lastPos, Quaternion.identity, transform);
        return newSection.GetComponent<LevelSection>().endPoint.position;
    }
}