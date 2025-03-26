/*using UnityEngine;

public static class CustomizationData
{
    public static string PlayerName = "Player";
    public static Color PlayerColor = Color.white;
    public static Material PlayerMaterial; 
}*/

using UnityEngine;

public static class CustomizationData
{
    public static string PlayerName { get; set; } = "Player";
    public static Color PlayerColor { get; set; } = Color.white;
    public static int ColorIndex { get; set; } = 0; // Store index for material selection
}