using UnityEngine;

// Dette er en enkel klasse for � representere en ressurs med en type (string) og mengde (int).
public class Resource
{
    public string type;
    public int amount;

    public Resource(string type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
    public Sprite LoadResourceImage() {
        switch (type.ToLower()) { 
            case "food" : { return Resources.Load<GameObject>("Corn").GetComponent<SpriteRenderer>().sprite; }
            case "wood" : { return Resources.Load<GameObject>("Wood").GetComponent<SpriteRenderer>().sprite; }
            case "stone" : { return Resources.Load<GameObject>("Stone").GetComponent<SpriteRenderer>().sprite; }
            case "iron" : { return Resources.Load<GameObject>("Iron").GetComponent<SpriteRenderer>().sprite; }
            case "gold" : { return Resources.Load<GameObject>("Gold").GetComponent<SpriteRenderer>().sprite; }
            case "oil" : { return Resources.Load<GameObject>("Oil").GetComponent<SpriteRenderer>().sprite; }
        }
        return null;
    }
}

