using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VocabWordSO", menuName = "Scriptable Objects/VocabWordSO")]
public class VocabWordSO : ScriptableObject
{
    public string displayWord;
    public Sprite displayImage;
    public List<GemSO> requiredGems;
}
