## Thinking

* 暂定Entity为一个int值
* Component为一个纯struct
* 使用Manager创建Entity与Component
* System监听Manager，当有Entity更新时就会被通知，进行判断是否符合条件，符合条件则加入列表，删除时同理
* System在每个业务点遍历列表，根据列表的id获取所需的Component，执行业务
* World作为MonoBehavior，执行一切System

* 为了性能，Database存储Component的容器必须代码生成
* 同样也是为了性能，获取Component的操作只能直接从Database操作所属容器了
* Entity销毁时，会通知各System
    * System顺手去Component容器把东西给销毁了