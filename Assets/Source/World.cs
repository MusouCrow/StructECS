using UnityEngine;
using Unity.Burst;

[BurstCompile]
public class World : MonoBehaviour {
    public TestSystem system;

    protected void Awake() {
        Database.Init();
        Manager.Init();

        var entity = Manager.NewEntity();
        Database.TestComponentMap[entity] = new TestComponent();
        Manager.ApplyEntity(entity);
    }

    protected void OnDestroy() {
        Database.Clean();
        Manager.Clean();
    }

    protected void FixedUpdate() {
        this.system.Update();
    }
}