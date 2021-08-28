using UnityEngine;
using Unity.Burst;

[BurstCompile]
public class World : MonoBehaviour {
    public SystemBase[] systems;

    protected void Awake() {
        this.systems = new SystemBase[] {
            new TestSystem(),
            new FuckSystem()
        };

        Database.Init();
        Manager.Init(this.systems);

        foreach (var system in this.systems) {
            system.Init();
        }
    }

    protected void OnDestroy() {
        foreach (var system in this.systems) {
            system.Dispose();
        }

        Database.Clean();
        Manager.Clean();
    }

    protected void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            for (int i = 0; i < this.systems.Length; i++) {
                this.systems[i].Update();
            }
        }

        Manager.DoRemove();
    }
}