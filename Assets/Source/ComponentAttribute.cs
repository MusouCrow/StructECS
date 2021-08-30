using System;

public class ComponentAttribute : Attribute {
    public bool managed;

    public ComponentAttribute(bool managed=false) {
        this.managed = managed;
    }
}