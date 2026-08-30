using UnityEngine;

public class Tile : MonoBehaviour
{
    public int line;
    public int column;
    public GameObject[] paths = new GameObject[6]; 
    public Tile[] neighbors = new Tile[6];
    public bool[] isPathOpen = new bool[6];

    public void Initialize()
    {
        line = 0;
        column = 0;
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i].SetActive(false);
            neighbors[i] = null;
            isPathOpen[i] = false;
        }
        
    }

    public void SetCoords(int line, int column)
    {
        this.line = line;
        this.column = column;
    }

    public void SetColor(Color color)
    {
        foreach (GameObject path in paths)
        {
            SpriteRenderer spriteRenderer = path.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }
}
