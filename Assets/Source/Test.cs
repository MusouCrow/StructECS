using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour {
    public GameObject prefab;

    protected async void Start() {
        // Manager.Instantiate(this.prefab);
        var handle = Manager.InstantiateAsync(this.prefab);

        while (!handle.completed) {
            await Task.Yield();
        }

        Debug.Log("Complete! " + handle.gameObject + ", " + handle.entity);
    }
}