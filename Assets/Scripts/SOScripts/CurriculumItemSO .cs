using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurriculumItemSO", menuName = "Scriptable Objects/CurriculumItemSO")]
public class CurriculumItemSO : ScriptableObject
{
    public string displayWord;
    public Sprite displayImage;
    public List<GemSO> requiredGems;
}
