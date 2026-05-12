using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguagePackSO", menuName = "Scriptable Objects/LanguagePackSO")]
public class LanguagePackSO : ScriptableObject
{
    public string languageName;
    public int gradeLevel;
    public enum DifficultyLevel { Easy, Medium, Hard }
    public DifficultyLevel difficulty;
    public List<GemSO> fullGemList;
    public List<VocabWordSO> vocabWordList;
}
