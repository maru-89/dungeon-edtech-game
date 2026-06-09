using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MethodOfLociSO", menuName = "Scriptable Objects/MethodOfLociSO")]
public class MethodOfLociSO : ScriptableObject
{
    public GameObject torchPrefab;
    public List<RoomThemeSO> roomThemes; // List of room themes to randomly assign to rooms for visual variety
    public float wallDistance = 50f; // Room size 50
    public float torchHeight = 8f; // Room height 10
    public float landmarkOffset = 5f; // Distance from wall for landmarks
}
