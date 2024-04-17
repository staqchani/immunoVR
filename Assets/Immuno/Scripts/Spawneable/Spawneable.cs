using UnityEngine;
using VRBeats.ScriptableEvents;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VRBeats 
{
    public class Spawneable : MonoBehaviour
    {
        [SerializeField] private float speed = 2.0f;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        public float Speed { get { return speed; } }

        private bool updatePositionX = false;
        private bool updatePositionY = false;
        private bool updatePositionZ = false;
        private bool updateSpeed = false;
        private bool updateRotation = false;

        public System.Action onSpawnComplete;

#if UNITY_EDITOR
        public virtual void CustomInspector(SpawnEventInfo info, Object[] targets)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 lastPosition = info.position;
            info.position = EditorGUILayout.Vector3Field("Position", info.position);
            if (EditorGUI.EndChangeCheck())
            {
                if (lastPosition.x != info.position.x)
                    updatePositionX = true;
                else if (lastPosition.y != info.position.y)
                    updatePositionY = true;
                else if (lastPosition.z != info.position.z)
                    updatePositionZ = true;
            }

            EditorGUI.BeginChangeCheck();
            info.speed = EditorGUILayout.FloatField("Speed", info.speed);
            if (EditorGUI.EndChangeCheck())
            {
                updateSpeed = true;
            }

            EditorGUI.BeginChangeCheck();
            info.rotation = EditorGUILayout.Vector3Field("Rotation" , info.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                updateRotation = true;
            }

            if (info.speed < 0.0001f)
            {
                info.speed = 0.0001f;
            }

            foreach (Object obj in targets)
            {
                if (obj is VR_BeatSpawnMarker spawnMarker)
                {
                    if (updateSpeed)
                    {
                        spawnMarker.spawInfo.speed = info.speed;
                    }
                        

                    if (updatePositionX)
                        spawnMarker.spawInfo.position.x = info.position.x;
                    if (updatePositionY)
                        spawnMarker.spawInfo.position.y = info.position.y;
                    if (updatePositionZ)
                        spawnMarker.spawInfo.position.z = info.position.z;

                    if (updateRotation)
                        spawnMarker.spawInfo.rotation = info.rotation;
                }
            }

            updatePositionX = false;
            updatePositionY = false;
            updatePositionZ = false;
            updateRotation = false;
            updateSpeed = false;

        }
#endif

        public virtual Quaternion GetSpawnRotation()
        {
            return Quaternion.Euler(rotation);
        }

        public virtual void Construct(SpawnEventInfo info)
        {
            int complexity = PlayerPrefs.GetInt("Complexity");
            switch (complexity)
            {
                case 0: info.speed = 4f; break;
                case 1: info.speed = 8f; break;
                case 2: info.speed = 12f; break;
            }
            
            speed = info.speed * info.speedMultiplier;
        }

        public virtual void OnSpawn()
        {
            if (onSpawnComplete != null)
                onSpawnComplete.Invoke();
        }

        public void SetSpeedDirection(int dir)
        {            
            speed = Mathf.Abs(speed) * dir;           
        }


    }
}

