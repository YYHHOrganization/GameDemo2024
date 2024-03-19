# Our game development in 2024

# 游戏内已开发的特性

- [x] 对Unity的Timeline进行扩展，实现了后处理轨道、BlendShape轨道，以及动态替换Timeline资产的功能（通过TimelineController实现）；
- [x] 在Timeline中应用Navmesh系统，使用的是Unity的官方timeline扩展插件；
- [x] 后处理效果现在支持径向模糊、以及Unity URP自带的一些Global Volume的后处理效果（其实还实现了高斯模糊，但是项目里暂时还不能调用）
- [x] 实现了一个简易的扫描线效果；
- [x] 在角色移动方便，实现Hireachical的有限状态机，目前支持Idle，移动和跳跃的基本逻辑转换；
- [x] UI有一个基本的框架，可以Pop和Push新的UI进来，进行统一的管理；
- [x] 在选择角色的界面，增加点击做动作，可以提起角色的交互行为；
- [x] 在选择角色的界面，支持ChatGPT3.5的对话系统，可以与其进行对话；
- [ ] 



# todo list

- [ ] 制作完成第一个可玩的关卡
- [ ] 增加一些宝箱，暂定是放置10个宝箱，宝箱分为普通，精致和华丽，存储的内容也不同，开出来之后会先显示在屏幕的UI上，



# bug fix

