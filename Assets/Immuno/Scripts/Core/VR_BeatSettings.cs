using UnityEngine;

namespace VRBeats
{
    [CreateAssetMenu(menuName = "VR Beats/Create Settings" , fileName = "Settings")]
    public class VR_BeatSettings : ScriptableObject
    {
        [Header("Core")]        
        [SerializeField] private Color rightColor = Color.green;
        [SerializeField] private Color leftColor = Color.red;
        [SerializeField] private float glowIntensity = 5.0f;
        [SerializeField] private float targetTravelDistance = 10.0f;
        [SerializeField] private float targetTravelTime = 0.2f;
        [SerializeField] private Ease targetTravelEase = Ease.EaseInOutSine;
        [SerializeField] private int errorLimit = 5;
        [Header("Score")]
        [SerializeField] private int scorePerHit = 1;
        [SerializeField] private int maxMultiplier = 10;
        [Header("Audio")]
        [SerializeField] private AudioClip leftCut;
        [SerializeField] private AudioClip rightCut;
        [Header("Spawnable")]
        [SerializeField]private Spawneable greenCell;
        [SerializeField]private Spawneable redCell;
        [SerializeField] private ModeSetting[] modes;
        public Color RightColor { get { return rightColor; } }
        public Color LeftColor { get { return leftColor; } }
        public float GlowIntensity { get { return glowIntensity; } }
        public float TargetTravelDistance { get { return Mathf.Abs( targetTravelDistance ); } }
        public float TargetTravelTime { get { return targetTravelTime; } }
        public Ease TargetTravelEase { get { return targetTravelEase; } }   
        public int ScorePerHit { get { return scorePerHit; } }
        public int MaxMultiplier { get { return maxMultiplier; } } 
        public int ErrorLimit { get { return errorLimit; } }

        public AudioClip RightCutSound { get { return rightCut; } }
        public AudioClip LeftCutSound { get { return leftCut; } }

        public Spawneable GreenCell { get { return greenCell; } }
        public Spawneable RedCell { get { return redCell; } }
        public ModeSetting Mode(int m) => modes[m];

        public void SetTargetTravelTime(float t) => targetTravelTime = t;
    }
    [System.Serializable]
    public class ModeSetting
    {
        public ModeLevel mode;
        public int totalSpawnable;
        [Range(3,6)]
        public int greenCellChance;
    }

    public enum ModeLevel
    {
        Easy,
        Medium,
        Hard
    }
}
