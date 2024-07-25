///                                                                        ///
/// Dev Note: This component must placed on transform that you wanna flip! ///
///                                                                        ///

using UnityEngine;

namespace Feature.Flip
{
    public class FlipController : MonoBehaviour
    {
        public bool IsFlipped { get; private set; }

        public void Flip(int value)
        {
            IsFlipped = value > 0 ? false : true;
            transform.localScale = new Vector3(value * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}