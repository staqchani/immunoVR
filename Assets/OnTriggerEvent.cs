using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBeats;
using VRBeats.ScriptableEvents;

public class OnTriggerEvent : MonoBehaviour
{
    [SerializeField] GameEvent onLeftCubeEnter;
    [SerializeField] GameEvent onRightCubeEnter;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<VR_BeatCube>(out VR_BeatCube cube))
        {
            if (cube.getColorSide() == ColorSide.Left)
                onLeftCubeEnter?.Invoke();
            else onRightCubeEnter?.Invoke();
        }
    }
}
