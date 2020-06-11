using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour {
    [SerializeField] private string _mapScene;

    public void ReturnToMap() {
        SceneManager.LoadScene(_mapScene);
    }
}
