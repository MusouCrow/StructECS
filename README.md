## 新游戏对象框架

### GameObject-Component的问题
* 实例化时会一口气将所有Component初始化，导致卡顿
* Component的各种回调通过很古老的方式调度，存在性能问题
* MonoBehavior的封装过厚

### 改进点
* 杜绝一口气实例化
    * 分帧创建对象？
    * 多线程创建对象？
* 浅层封装
    * 数据行为分离？
* Cache Friendly?
    * Component使用struct？
    * 采用ECS？

### ECS的问题
* 某些玩意会出现A Component, A System的问题
* 解决方案: 不必苛求，做成MonoBehavior也是可以的

### Struct的问题
* 作为返回值的时候会拷贝
    * 可通过返回索引之类的方式去间接获取
    * 数据量不是很大的话其实也可以接受
* 在连连看之类的场合可能ref会失效
    * 通过索引的方式进行？
* 不便存放在某个对象里
    * 对util不利，要考虑不持有或间接获取的方式了

### Class的问题
* 不利于Cache Friendly
* 增加GC负担

### Cache Friendly？
* 内存上紧密连续
* Entity层面：将Entity的Component紧密排布
    * 除了挨个构建聚合struct Component的entity，没啥好办法
* Component层面：成员尽量使用值类型
    * struct就完事了
* System层面：连续运行相同结构的业务
    * ECS搞起来
* 函数层面：ref参数，少了指针访问，参数一并入缓存
    * struct搞起来

### Job & Brust
* 得是不含托管资源、纯struct资源
* 不含绝大多数的Unity引擎API，用DOTS API替代
* 符合条件的Component可通过Job异步创建
* 符合条件，可异步执行的System可调用Job
* 符合条件的函数和struct可声明Brust

### Unity内置组件的兼容
* 通过一层新的Component包装它们？
* 直接使用？

### Entity
* 以struct的形式存储各种Component？
    * 最利于cache friendly
    * 但种类太多，扩展性差
* 以Dictionary的形式存储Component？
    * 过于冗杂
    * Component得是Class
* 以数组作为容器，Component枚举代表数组的下标存储？
    * 空间浪费
    * Component得是Class
* Entity作为一个id，各Component存储在各自的Map？
    * 比较健壮的实现
    * 添加新Component时需要添加相应措施
    * 可考虑自动生成代码

### Component
* 只拥有必要的、不依赖其他Component的函数
* Component作为Class？
    * 拉了
* Component作为Struct？
    * 好

### System
* 筛选出符合Component组合的Entity
* 方案1？
    * 构建Entity-Component集合的Struct，将符合条件的单位组成List
    * 遍历List，执行业务
    * 业务执行完毕后要将修改过的Component写回Map
    * 比较啰嗦，有利于cache friendly，性能更好
* 方案2？
    * 存储Entity List
    * 遍历List，从Map提取Component，执行业务
    * 每帧都要获取Component，不利于性能

### Service
* 写一堆功能函数，提供它处调用
* 以Component为主要参数，功能组合产生火花

### Util
* 为了cache friendly，也尽量使用struct
* 不保存Component，通过外部参数获得

### 连连看
* 考虑封装一个Actor对象，囊括相关Component？
    * 最后写回Map的量太大，顶不住
    * 除非能做脏标记
    * 手动在函数标记Component脏了