using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public class Test : MonoBehaviour {
    private NativeList<Entity> entites;

    protected void Start() {
        this.entites = new NativeList<Entity>(Allocator.Persistent);

        for (int i = 0; i < 3; i++) {
            this.AddEntity();
        }
    }

    protected void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && this.entites.Length > 0) {
            Manager.DelEntity(this.entites[0]);
            this.entites.RemoveAt(0);
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            this.AddEntity();
        }
    }

    protected void OnDestroy() {
        this.entites.Dispose();
    }

    private void AddEntity() {
        var entity = Manager.NewEntity();
        Database.TestComponent[entity] = new TestComponent();
        Database.FuckComponent[entity] = new FuckComponent();
        
        Manager.ApplyEntity(entity);
        this.entites.Add(entity);
    }
}