using System;

public class SystemAttribute : Attribute {
    public int order = 0;

    public SystemAttribute(int order=0) {
        this.order = order;
    }
}