# Our game development in 2024

# 游戏内已开发的特性

- [x] 对Unity的Timeline进行扩展，实现了后处理轨道、BlendShape轨道，以及动态替换Timeline资产的功能（通过TimelineController实现）；
- [x] 在Timeline中应用Navmesh系统，使用的是Unity的官方timeline扩展插件；
- [x] 后处理效果现在支持径向模糊、以及Unity URP自带的一些Global Volume的后处理效果（其实还实现了高斯模糊，但是项目里暂时还不能调用）
- [x] 实现了一个简易的扫描线效果；
- [x] 在角色移动方便，实现Hireachical的有限状态机，目前支持Idle，移动和跳跃的基本逻辑转换；
- [x] 
- [x] 在选择角色的界面，增加点击做动作，可以提起角色的交互行为；
- [x] 
- [x] 导入了DoTween包，实现了部分UI的跳动显示；
- [x] 增加一些宝箱，暂定是放置10个宝箱，宝箱分为普通，精致和华丽，存储的内容也不同，开出来之后会先显示在屏幕的UI上，
- [x] 增加新角色的护盾特效和激光特效
- [x] 【1】策划表读取方案：
  - [x] （1）老系统的策划表读取方案（**目前新的策划表不再用了，但向下兼容**）：使用csv格式的表格作为策划表，直接用C#的`split`函数读取字段，并存在类的对象里；
  - [x] **（2）新的策划表读取方案：根据参考资料自制转表工具，可以直接自动把csv表格转成对应的类，代码可以直接访问。（大概思路：从csv中读出字段，根据字段类型等信息—>json文件—>生成C#类代码）**
    - [ ] 目前已知仍存在问题：csv策划表原始文件只支持简单数据结构类型（如string，int等），并且不能有空行（空行无法正确解析）

- [x] 【2】音频管理：存于`HAudioManager.cs`文件当中，从策划表中读取，目前支持的功能：Play，Stop，淡出某段策划表中指定音频，接口封装的比较简单；
- [x] 【3】UI管理：UI有一个基本的框架，可以Pop和Push新的UI进来，进行统一的管理；
  - [ ] **局限性：可以实现的优化方向：（1）目前UI仍是在Unity当中手动拖拽布局，可以改为json读取，并且虽然有简单考虑不同分辨率设备造成的影响，但并没有严格测试，理论上设备分辨率支持应该在底层考虑清楚，这里没有过多考虑。**
  - [ ] 考虑对框架进行优化，以适配更加复杂的情况，但暂时未提上日程；

- [x] 【4】整体资产管理：使用Unity的`Addressables`系统，==**关于该系统现在了解的还不是特别深入，并且暂时项目还未结合热更。**==
- [x] 【5】关于Message信息相关：
  - [x] （1）添加`HMessageShowMgr.cs`类，用策划表读取所有的Message信息，目前游戏内支持8种左右的消息类型；
  - [x] （2）消息可以配置是否渐入渐出、二次确认窗口、指定回调函数等。需要更多的消息类型再往里加；
    - [ ] todo：UI其实是一个难点，怎么让不同的panel不互相遮挡，这个未来如果感兴趣可以好好优化一下，现在的做法是控制例如子节点的排布顺序，估计挺费的；

- [x] 【6】



## 肉鸽（Roguelike）游戏中开发的特性（客户端向）

- [x] 【1】登录界面，帕姆在车上被甩飞（用Timeline制作），加载进度条（加载进度条保留读取策划表的时间）。
- [x] 【2】在选择角色的界面，支持ChatGPT3.5的对话系统，可以与其进行对话。关于GPT相关的功能，详见`HChatGPTManager.cs`文件，目前由于Key的限制暂时关闭了聊天功能，如果您有自己的key可以按照这个链接中的文档（https://github.com/srcnalt/OpenAI-Unity）进行配置，理论上就可以使用项目中的对话功能了。



【1】2024.6.24

- [x] todo：在肉鸽游戏中加入设置界面，在该界面可以控制音量大小、鼠标灵敏度等；
- [x] todo：在肉鸽游戏中加入ESC键，游戏暂停，可以继续游戏，重新开始，打开设置界面，游戏退出等；
- [x] todo：在音游玩法和拼图玩法中都加入按下ESC键可以退出游戏的选项，并且都有二次确认窗口；



【2】2024.6.25

- [ ] 在游戏中引入成就系统，加一些简单的成就；
- [ ] todo：简易的存档功能，这个再说；
- [x] 在游戏中加入GM窗口，提供一些简单的Button可以选择，或者是召唤宠物之类的。



**【3】2024.6.27~2024.7.2**

- [x] 猛然发现PlayerCamera居然没有开启反走样，赶紧把PlayerCamera设置成了TAA反走样；



【4】2024.7.3

- [x] 添加了3种道具，以及对应的客户端逻辑；



## TA相关的内容（渲染向、PCG等）

- [x] 最近学习了流体模拟的效果，使用Compute shader，现在在boss房当中会有一个2D可交互的流体效果，不过只是对贴图进行修改，没有真正的流体体积；
- [ ] 



# todo list

- [x] 制作完成第一个可玩的关卡
- [x] ==音乐和音效（H）==
- [x] ==电梯的方案优化（比如人上去之后让人作为电梯的子节点，电梯到达楼顶之后再分开）（Y）==
- [x] 木偶分屏选择的时候鼠标要点左边时 旋转鼠标可以控制人物移动旋转，点右边的时候锁住视角（Y）-现在改为可以快捷键Lock
- [x] 钥匙的逻辑+钥匙在探测的时候的显示（H）；
- [x] 呼出木偶的时候，界面关闭+格子撤回（Y）；
- [x] ==开门 完成第一关（Y）==
- [x] ==LevelManager的完善，策划表的集体完善（Y），==
- [ ] ==存档功能（存档收集的东西，拥有的角色。。。。。）==（H）
- [ ] ==第二关：激光。木偶和角色的释放技能功能，角色死亡状态，释放技能状态（写进有限状态机里）（一起，后面再说）==
- [ ] 一个Message的Panel+策划表（H）
- [x] 加一个开盾音效
- [x] 加绀海组彩蛋
- [x] 加开盾激光
- [x] 拿宝箱
- [x] 改给的角色：黄泉
- [ ] 加弹球
- [ ] 加瑞早六和
- [ ] 加背包
- [ ] 加开头shader
- [ ] 加死亡状态
- [ ] 加入一个不同玩家的存档体系



# bug fix

**【1】2024.6.23**

- [x] （1）修复了猫猫糕和角色解锁进入肉鸽游戏的条件：要同时解锁才能进入肉鸽游戏。
- [x] （2）修复了刚进入肉鸽游戏的时候点击确定会出现一个射击瞄准的图标的问题；
- [x] （3）修复了闪电劈中角色可能会多次扣血的问题；



**【2】2024.6.24**

- [x] （1）修复了三月七猫猫糕有概率不出现的问题；
- [x] （2）修复了落雷的碰撞箱问题：原来的碰撞体积有点大，角色容易被雷劈死；
- [ ] （3）todo：在运行的时候出了一个报错，大概是foreach的时候数组中的元素发生了改变，但是由于忘记保存log信息了，所以暂时无法复现，有需要再找一下。
- [ ] （4）todo：（**已知可能出现的问题，暂未修复**）音游玩法捡到道具effect可能会丢失，该问题将在下一个版本得到修复。
- [ ] （5）todo：（**已知可能存在的问题**）在初始的时候可能没有出生在初始房间，有概率出生在其他的房间。
- [x] （6）修复了部分UI在叠加在一起之后再退出顶层UI，可能会出现射击瞄准图案的问题。（解决方案是在Resume中进行处理）



**【3】2024.6.25**

- [x] 修复了重新加载游戏，单例模式上绑定的(`+=`)监听事件会出现报错现象，解决方案是在重新加载场景之前取消单例模式上的所有事件监听，见坑【1】；
- [x] 发现触发对话系统可能会导致接下来不打雷的问题，因为不小心把Weather系统挂载的gameObject的active设置成false了。
- [ ] 有可能出现对话奖励不触发的问题？不确定有没有，有可能是因为选项点太快了。







##  已经讲完的特性





# 坑

【1】2024.6.25遇到的问题：https://blog.csdn.net/mo_qi_qi/article/details/106806988，也就是说在使用`SceneManager.LoadSceneAsync(currentSceneName);`重新加载场景之前，要记得把所有单例模式上绑定的监听函数取消监听，否则会报奇怪的错误，比如MissingReference等；

【2】
