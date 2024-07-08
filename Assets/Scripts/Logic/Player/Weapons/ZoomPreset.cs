using UnityEngine;

namespace Logic.Player.WeaponsSystem
{
    [CreateAssetMenu(fileName = "Zoom Preset", menuName = "Presets/Zoom")]
    public class ZoomPreset : ScriptableObject
    {
        public int Zoom;
        public float LensSize;
    }
}