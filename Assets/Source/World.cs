using UnityEngine;
using Unity.Burst;

[BurstCompile]
public class World : MonoBehaviour {
    protected void Awake() {
        Database.Init();
        Manager.Init();
    }

    protected void Start() {
        foreach (var system in Manager.systems) {
            system.Init();
        }
    }

    protected void OnDestroy() {
        Database.Clean();
        Manager.Clean();
    }

    protected void FixedUpdate() {
        for (int i = 0; i < Manager.systems.Count; i++) {
            Manager.systems[i].Update();
        }

        Manager.DoRemove();
        Manager.DoTask();
    }
}