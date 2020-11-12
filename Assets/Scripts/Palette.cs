using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{
    //Splotch Center
    public Dictionary<int, Color> palette = new Dictionary<int, Color>();

    public Palette()
    {
        palette = new Dictionary<int, Color>();
        palette.Add(0, new Color(0.1f, 0.5f, 0.0f)); // empty
        palette.Add(1, new Color(0.3f, 0.3f, 0.8f)); // blocks
        palette.Add(2, Color.red); // splotch centers
        palette.Add(3, new Color(0.8f, 0.6f, 0.3f)); // splotch rim
        palette.Add(4, new Color(0.7f, 0.1f, 0.5f)); // squares
    }
}
