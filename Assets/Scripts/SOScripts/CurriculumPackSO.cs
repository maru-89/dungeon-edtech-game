using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurriculumPackSO", menuName = "Scriptable Objects/CurriculumPackSO")]
public class CurriculumPackSO : ScriptableObject
{
    public string curriculumName;
    public int gradeLevel;
    public enum DifficultyLevel { Easy, Medium, Hard }
    public DifficultyLevel difficulty;
    public List<GemSO> fullGemList;
    public List<CurriculumItemSO> curriculumItemList;
}
