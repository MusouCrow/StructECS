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

* 如何收集System
    * 手动记录？
    * 利用Attribute，还带自动排序？
* System的Entity容器从何而来
    * 自己管理
    * System得考虑交给Manager持有
        * 为了更好的跟API联动

* GameObject实例化
    * 要做到在场景的对象可以直接实例化
    * 代码创建不可以直接GameObject.Instantiate，太卡了
    * 直接新建GameObject，复制名字、layer、tag相关
    * 对于MonoBehavior，采取遍历Prefab含有IConvert的组件，调用Convert函数直接生成Entity与Component
    * 遍历Prefab的子对象，如法炮制
* 分帧创建
    * 对于不追求一口气直接创建的对象，将以Component为颗粒度加入到任务列表中
    * 任务内容包括: 创建GameObject，创建Component，激活Entity
    * 将以栈的形式存放任务，每帧会定量消费任务
    * 对于渲染组件，在激活前将会隐藏之
* 创建时获取父对象的问题
    * 创建一个对象栈，将创建的GameObject放进去
    * 相关Component的创建就根据栈的尾部获取GameObject
    * 触发激活时，尾部GameObject出栈
* 有必要直接返回对象么？
    * 对于一口气创建的，有必要
    * 对于分帧异步的，可提供个回调参数作为通知
    * 也可以考虑返回个Handle，便于协程使用，缺点是会产生GC
* 对于一点IConvert不存的无逻辑对象
    * 直接返回GameObject，不视为Entity？
    * 哪怕是无逻辑也得挂载转换组件？