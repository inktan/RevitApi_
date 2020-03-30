goa-tools-wt
=====
goa 大象设计 插件源代码 王坦

#### CommentDoorWindowL_H_Custom04（外部命令）-明细表数据注释
*说明-need by 王彦韬：  
读取族参数添加族参数；*

#### COLR_SWIT（外部命令）-颜色切换
*说明：  
将视图图元替换为黑色，目前采取的策略为：通过viewId创建过滤器，过滤出视图可见图元，进行图元一键替换；*

#### GeometrySurfaceArea（外部命令）-读取物体面的面积，单选
*说明：  
单选任何物体的几何面，读取几何面面积；*

#### GeometrySurfacesArea（外部命令）-读取物体面的面积，多选
*说明：  
多选任何物体的几何面，读取几何面面积之和；*

#### GRID_Number（外部命令）-划线编轴号
*说明：  
通过两点连线进行轴网重新标号；预设三种轴号样式，设计师和自主选择是否加分区号*

#### ModelessFormExternalEvent（外部应用）-测试request
*说明：  
通过非模窗口点击按钮触发revit主线程命令，该程序通过一个外部事件接口，设置多个命令；*

#### New Command_addin（外部应用）-注册使用图标
*说明：  
使用外部应用接口，将外部命令注册为可点击图标；*

#### SynchronizewithcentralTimer（外部应用）-操作停止后，倒计时同步中心模型

*说明：  
解决问题：  
在对模型进行最后一次修改后，倒计一段时间后自动与中心模型同步；
解决方案：
使用DocumentChanged事件进行全局监控，通过DocumentChanged事件触发timer时间机制，在timer时间线程调用外部事件（ExternalEventHandler）回到revit主线程；*

#### VIEW_INTF（外部命令）-提资各视图-待完成
*说明：  
功能1-复制或更新PLOT视图为INTF视图；  
功能2-复制或更新PLOT视图为建筑方案图；  
功能2-复制或更新PLOT视图为建筑方案图；*

#### WorkSetSharedLevelsGrids（外部应用）-自动将标高轴网移动到共享工作集
*说明：  
使用DMU（Dynamic Model Update)元素联动机制，自动将轴网标高移动到"共享标高和轴网" 或者 "Shared Levels and Grids"工作集；  
需要注意，DMU回调函数已经在transaction里面，因此，在该函数执行事务动作对模型进行修改时，不需要额外创建transaction*
