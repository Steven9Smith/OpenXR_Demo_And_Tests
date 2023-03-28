using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public float maxHeight = 0f;
    public GameplayGroup[] groups;
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool allTrue = true;
        foreach (GameplayGroup group in groups)
        {
            if (!group.CriteriaMet(maxHeight))
                allTrue = false;
            group.Update(maxHeight);
        }
        if (allTrue)
        {
            var things = GameObject.FindGameObjectsWithTag("Dynamic_Body");
            foreach(var t in things)
            {
                var buo = t.GetComponent<BuoyancyComponent>();
                if(buo != null)
                {
                    buo.waterHeight = 10;
                }
            }
            foreach(var g in groups)
            {
                g.minObj = 15;
            }
        }
    }
}
[System.Serializable]
public class GameplayGroup
{
    public Transform[] obj;
    public TextMeshProUGUI text;
    public int minObj;
    public string name;
    public int GetGroupsUnderMaxHeight(float maxHeight)
    {
        int a = 0;
        for(int i = 0; i < obj.Length; i++)
            a += obj[i].position.y < maxHeight ? 1 : 0;
        return a;
    }
    public bool CriteriaMet(float maxHeight)
    {
        return GetGroupsUnderMaxHeight(maxHeight) >= minObj;
    }
    public void Update(float maxHeight)
    {
        if (text != null) text.text = Mathf.Clamp((minObj - GetGroupsUnderMaxHeight(maxHeight)),0,100) + " " + name + " remaining";
    }
}