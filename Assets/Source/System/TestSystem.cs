using UnityEngine;
using Unity.Burst;
using Entity = System.Int32;

[BurstCompile]
public class TestSystem : SystemBase {
    private int value;

    public override bool Filter(in Entity entity) {
        return Database.TestComponent.ContainsKey(entity) && Database.FuckComponent.ContainsKey(entity);
    }

    public override void OnEnter(in Entity entity) {
        var test = Database.TestComponent[entity];
        test.value = this.value++;
        Database.TestComponent[entity] = test;
    }

    public override void OnExit(in Entity entity) {
        Database.TestComponent.Remove(entity);
    }

    public override void Update() {
        foreach (var e in this.entities) {
            var test = Database.TestComponent[e];
            var fuck = Database.FuckComponent[e];
            Debug.Log(e + ", " + test.value + ", " + fuck.value);
        }
    }
}