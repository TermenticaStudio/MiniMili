using UnityEditor;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaAutoLogin : MonoBehaviour
    {
        #region FIELDS
        public bool test;
        [SerializeField] private float retryTime = 5f;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            NakamaManager.Instance.onLoginFail += LoginFailed;
            TryLogin();
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onLoginFail -= LoginFailed;
        }

        private void TryLogin()
        {
            if (test)
            {
                NakamaManager.Instance.LoginWithCustomId(System.Guid.NewGuid().ToString());
            }
            else
            {
                NakamaManager.Instance.LoginWithDevice();
            }
        }

        private void LoginFailed()
        {
            Invoke(nameof(TryLogin), retryTime);
        }

        #endregion
    }
}
