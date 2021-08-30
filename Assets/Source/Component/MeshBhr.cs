using UnityEngine;

[ComponentAttribute(true)]
public struct MeshCom : IComponent {
    public MeshFilter filter;
    public MeshRenderer renderer;
}

public class MeshBhr : MonoBehaviour, IConverted {
    protected void Start() {
        this.Convert(this.gameObject, true);
        Destroy(this);
    }

    public void Convert(GameObject gameObject, bool self) {
        var entity = Manager.GetEntityWithGameObject(gameObject);
        var filter = this.GetComponent<MeshFilter>();
        var renderer = this.GetComponent<MeshRenderer>();

        if (!self) {
            var filter2 = gameObject.AddComponent<MeshFilter>();
            var renderer2 = gameObject.AddComponent<MeshRenderer>();
            filter2.sharedMesh = filter.sharedMesh;
            renderer2.sharedMaterials = renderer.sharedMaterials;
            renderer2.receiveShadows = renderer.receiveShadows;
            renderer2.shadowCastingMode = renderer.shadowCastingMode;

            filter = filter2;
            renderer = renderer2;
        }

        Database.MeshCom[entity] = new MeshCom() {
            filter = filter,
            renderer = renderer
        };
    }
}