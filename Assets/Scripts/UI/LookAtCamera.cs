using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}