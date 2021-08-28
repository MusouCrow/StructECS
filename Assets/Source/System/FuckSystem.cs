using UnityEngine;
using Unity.Burst;
using Entity = System.Int32;

[BurstCompile]
public class FuckSystem : SystemBase {
    private int value;

    public override bool Filter(in Entity entity) {
        return Database.FuckComponent.ContainsKey(entity);
    }

    public override void OnEnter(in Entity entity) {
        var fuck = Database.FuckComponent[entity];
        fuck.value = 123;
        Database.FuckComponent[entity] = fuck;
    }

    public override void OnExit(in Entity entity) {
        Database.FuckComponent.Remove(entity);
    }
}