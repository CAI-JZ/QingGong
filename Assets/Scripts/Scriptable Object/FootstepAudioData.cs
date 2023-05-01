using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/AudioData")]
public class FootstepAudioData : ScriptableObject
{
    public List<FootstepAudio> footstepAudios = new List<FootstepAudio>();
    
}

[System.Serializable]
public class FootstepAudio
{
    public string Tag;

    [Range(0,1)]
    public float audioVolume;

    public List<AudioClip> audioClips = new List<AudioClip>();

    public float voiceInterval;
}
