using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurriculumPackSO", menuName = "Scriptable Objects/CurriculumPackSO")]
public class CurriculumPackSO : ScriptableObject
{
    public string curriculumName;
    public int gradeLevel;
    public DifficultyLevel difficulty;
    public List<RetrievalType> supportedRetrievalTypes;
    public List<GemSO> fullGemList;
    public List<CurriculumItemSO> curriculumItemList;
}
