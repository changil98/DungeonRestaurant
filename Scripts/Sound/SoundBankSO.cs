using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundBank", menuName = "Audio/Sound Bank")]
public class SoundBank : ScriptableObject
{
    public List<Sound> sounds = new List<Sound>();
}
