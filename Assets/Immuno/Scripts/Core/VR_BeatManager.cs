using UnityEngine;
using Platinio;
using UnityEngine.Playables;
using VRBeats.ScriptableEvents;
using VRSDK;
using System.Collections;

namespace VRBeats
{
    public class VR_BeatManager : Singleton<VR_BeatManager>
    {
        [SerializeField] private BoxCollider playZone = null;
        [SerializeField] private Transform player = null;
        [SerializeField] private VR_BeatSettings settings = null;
        [SerializeField] private GameEvent onGameOver = null;
        [SerializeField] private AudioClip[] cutSounds;
        private AudioManager audioManager = null;
        private EnviromentController enviromentController = null;
        private PlayableDirector playableDirector = null;
        private bool isGameRunning = true;

        public Color RightColor { get { return settings.RightColor * settings.GlowIntensity; } }
        public Color LeftColor { get { return settings.LeftColor * settings.GlowIntensity; } }
        public VR_BeatSettings GameSettings { get { return settings; } }

        public Transform Player { get { return player; } }

        private int playerConsecutiveMiss = 0;

        protected override void Awake()
        {
            base.Awake();
            audioManager = FindObjectOfType<AudioManager>();
            enviromentController = FindObjectOfType<EnviromentController>();

            playableDirector = FindObjectOfType<PlayableDirector>();
        }

        IEnumerator GameCoroutine()
        {
            float duration = (float)playableDirector.playableAsset.duration;
            float t = 0;
            int complexity = PlayerPrefs.GetInt("Complexity");
            settings.SetTargetTravelTime(complexity == 0 ? 0.5f : complexity == 1 ? 0.4f : 0.3f);
            ModeSetting mode = settings.Mode(complexity);
            float steps = duration / mode.totalSpawnable;
            float s = 0;
            float spawnTime = 0;
            float lastSpawnTime = 0;
            bool spawn = false;
            while(t < duration && isGameRunning)
            {
                t += Time.deltaTime;
                s += Time.deltaTime;
                if(s > steps)
                {
                    spawnTime = Random.Range(0, steps/3f);
                    s = 0;
                    spawn = false;
                    
                }

                if(s > spawnTime && !spawn)
                {
                    spawn = true;
                    lastSpawnTime = t;
                    SpawnEventInfo info = new SpawnEventInfo()
                    {
                        colorSide = Random.Range(0, 2) == 1 ? ColorSide.Left : ColorSide.Right
                    };

                    Spawneable sp = Random.Range(0, mode.greenCellChance) == 0 ? settings.GreenCell : settings.RedCell;
                    Spawn(sp, info);
                }
                yield return null;
            }

            //if (isGameRunning) GameOver();
        }
        protected override void Start()
        {
            base.Start();
            playerConsecutiveMiss = 0;
            StartCoroutine(GameCoroutine());
        }
        
        public void PlayCutSound(int clip)
        {
            audioManager.PlayShortClip(cutSounds[clip]);
        }
        public Color GetColorFromColorSide(ColorSide side)
        {
            return side == ColorSide.None ? RightColor : LeftColor;
        }

        public Color GetColorFromControllerType(VR_ControllerType controller)
        {
            return controller == VR_ControllerType.Right ? RightColor : LeftColor;
        }

        private int frame = 0;
        private void Update()
        {
            frame++;                       
        }

        public void Spawn(Spawneable spawneable , SpawnEventInfo info)
        {
            if (!isGameRunning)
                return;
            info.position.z = 0;
            info.position.y = Random.Range(0.2f, 1f);
            info.position.x = Random.Range(-0.6f, 0.6f);
            Vector3 finalPosition = CalculateSpawnPosition( info.position);
            Vector3 travelOffset = Vector3.forward * -settings.TargetTravelDistance;
            Vector3 spawnPosition = finalPosition - travelOffset;

            //Spawneable clone = Instantiate( spawneable , spawnPosition , Quaternion.Euler( info.rotation ) );
            Spawneable clone = Instantiate( spawneable , spawnPosition , Quaternion.identity );
            SetSpeedRelativeToPlayZone(info);
            clone.Construct(info);
            
            Vector3 finalScale = clone.transform.localScale;
            clone.transform.localScale = Vector3.zero;

            
            clone.transform.Move(finalPosition, settings.TargetTravelTime).SetEase(settings.TargetTravelEase).SetOnComplete(delegate
            {
                clone.OnSpawn();
            }).SetUpdateMode(Platinio.TweenEngine.UpdateMode.Update);     

            
            clone.transform.ScaleTween(finalScale, settings.TargetTravelTime).SetEase(settings.TargetTravelEase);

        }

        private void SetSpeedRelativeToPlayZone(SpawnEventInfo info)
        {
            info.speedMultiplier = (int) Mathf.Sign(playZone.transform.forward.z * -1.0f);
        }
       
        private Vector3 CalculateSpawnPosition(Vector3 relativePosition)
        {
            Vector3 pos = CalculatePlayZoneCenter();

            pos += Vector3.right * relativePosition.x * playZone.size.x;
            pos += Vector3.up * relativePosition.y * playZone.size.y;
            pos += Vector3.forward * relativePosition.z * playZone.size.z;

            return pos;
        }

        private Vector3 CalculatePlayZoneCenter()
        {
            return playZone.transform.position + playZone.center;
        }

        public void GameOver()
        {
            //the game is already stopped
            if (!isGameRunning)
            {
                return;
            }

            isGameRunning = false;
            //slowdown the music to 0 and stop the playabledirector
            audioManager.BlendAudioMixerPitch(1.0f , 0.0f).SetOnComplete( delegate {
                if (playableDirector != null)
                    playableDirector.Stop();
            }
            ).SetOwner(gameObject);
            enviromentController?.TurnLightsOff();
            
        }

        public void RestartLevel()
        {
            gameObject.CancelAllTweens();

            isGameRunning = true;
            audioManager.SetAudioMixerPitch(1.0f);
            enviromentController?.TurnLightsOn();
            StartCoroutine(GameCoroutine());
            //playableDirector.time = 0.0f;
            //playableDirector.Play();
        }

    }

}
