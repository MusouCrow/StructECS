using UnityEngine;
using Unity.Burst;
using Entity = System.Int32;

[BurstCompile]
public class RotateSystem : SystemBase {
    private int value;

    public override bool Filter(in Entity entity) {
        return Database.TransformCom.ContainsKey(entity);
    }

    public override void OnExit(in Entity entity) {
        Database.TransformCom.Remove(entity);
    }

    public override void Update() {
        foreach (var e in this.entities) {
            var transform = Database.TransformCom[e];
            transform.transform.Rotate(new Vector3(0, 1, 0));
        }
    }
}

