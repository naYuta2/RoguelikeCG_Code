using UnityEngine;
using UnityEngine.Events;
public class UnityEventCTR : MonoBehaviour
{
    [SerializeField]
    UnityEvent unityEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Test()
    {
        Debug.Log("UnityEvent実行！");
    }
}
