using UnityEngine;
using Unity.Burst;
using Entity = System.Int32;

[BurstCompile]
public class TestSystem : SystemBase {
    private int value;

    public override bool Filter(in Entity entity) {
        return Database.TestCom.ContainsKey(entity);
    }

    public override void OnEnter(in Entity entity) {
        var test = Database.TestCom[entity];
        test.value = this.value++;
        Database.TestCom[entity] = test;
    }

    public override void OnExit(in Entity entity) {
        Database.TestCom.Remove(entity);
    }

    public override void Update() {
        foreach (var e in this.entities) {
            var test = Database.TestCom[e];
            Debug.Log(e + ", " + test.value);
        }
    }
}