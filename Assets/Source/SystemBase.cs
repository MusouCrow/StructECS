using Unity.Collections;
using Entity = System.Int32;

public class SystemBase {
    protected NativeHashSet<Entity> entities;

    public SystemBase() {
        this.entities = new NativeHashSet<Entity>(16, Allocator.Persistent);
    }

    public void Dispose() {
        this.entities.Dispose();
    }

    public void AddEntity(in Entity entity) {
        if (this.Filter(entity)) {
            this.entities.Add(entity);
            this.OnEnter(entity);
        }
    }

    public void DelEntity(in Entity entity) {
        bool ok = this.entities.Remove(entity);

        if (ok) {
            this.OnExit(entity);
        }
    }

    public virtual void Init() {}
    public virtual void Update() {}
    public virtual bool Filter(in Entity entity) {return true;}
    public virtual void OnEnter(in Entity entity) {}
    public virtual void OnExit(in Entity entity) {}
}