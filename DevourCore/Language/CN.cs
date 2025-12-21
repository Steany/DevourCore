using UnityEngine;

namespace DevourCore
{
	public static class CN
	{
		public static class GUI
		{
			public const string Tab_Optimize = "优化";
			public const string Tab_HSV = "外观";
			public const string Tab_Speedrun = "速通";
			public const string Tab_FOV = "视野";
			public const string Tab_Anticheat = "反作弊";
			public const string Tab_Menu = "菜单";
			public const string Tab_Settings = "设置";

			public const string PressAnyKey = "按任意键...";
			public const string ToggleKeyFormat = "切换键: {0}";
			public const string InteractKeyFormat = "交互键: {0}";
			public const string MenuKeyFormat = "打开客户端: {0}";
			public const string CurrentClipFormat = "当前: {0}";
			public const string AreYouSure = "确定吗？";
			public const string ResetClientSettings = "重置客户端设置";

			public const string Header_Optimize = "性能优化";
			public const string Desc_Optimize = "自定义渲染距离并禁用天气效果。";
			public const string Header_Cull = "渲染距离";
			public const string Toggle_CullEnabled = "在游戏中启用";
			public const string Toggle_CullInMenu = "在菜单中启用";
			public const string CullDistanceFormat = "距离: {0} 米";
			public const string Header_Weather = "天气";
			public const string Toggle_DisableWeather = "禁用天气效果";
			public const string Toggle_MuteWeather = "静音天气音效";

			public const string Header_OutfitColor = "服装颜色";
			public const string Header_IconColor = "图标颜色";
			public const string Desc_OutfitColor = "更改所有玩家的服装颜色。";
			public const string Desc_IconColor = "自定义 666 图标的颜色。";
			public const string Toggle_IconEnabled = "启用图标";
			public const string Toggle_OutfitEnabled = "启用服装";
			public const string Swap_ToOutfit = "服装 ↔";
			public const string Swap_ToIcon = "图标 ↔";
			public const string Header_Preview = "预览";

			public const string Header_Speedrun = "速通模组";
			public const string Desc_Speedrun = "适用于速通类别的实用功能。";
			public const string Toggle_InstantInteract = "瞬时互动";
			public const string Toggle_AtticSpawn = "阁楼出生点";
			public const string Header_AutoStart = "自动开始";
			public const string Desc_AutoStart = "从大厅返回时自动开始对局。";
			public const string Toggle_ForceStart = "启用";
			public const string Toggle_UseArm = "准备时间窗口";
			public const string ForceStartDelayFormat = "开始延迟: {0:F1} 秒";
			public const string ForceStartArmFormat = "准备时间: {0:F1} 分钟";
			public const string SpeedrunPopupBody =
				"请合理使用这些修改功能。避免在与不认识的玩家进行的休闲对局中使用，否则可能会被视为作弊。利用这些功能刷等级或获得不公平优势也被视为作弊。这些功能仅用于娱乐和速通目的，而不是用来伤害或干扰其他玩家。请谨慎使用。";
			public const string SpeedrunPopupConfirm = "已了解";

			public const string Header_FOV = "自定义视野";
			public const string Desc_FOV = "将视野调整到高于或低于游戏默认限制。";
			public const string Toggle_FOVEnabled = "启用";
			public const string FOVValueFormat = "视野: {0}°";

			public const string Header_Anticheat = "速度反作弊";
			public const string Desc_Anticheat = "监控玩家是否存在可疑的移动速度。";
			public const string Toggle_AnticheatEnabled = "启用检测";
			public const string AlertDurationFormat = "警报持续时间: {0:F1} 秒";
			public const string Button_ClearAlerts = "清除警报";
			public const string AnticheatStatusFormat = "正在监控 {0} 名玩家 | {1} 个活动警报";
			public const string Button_SaveAlertPosition = "保存警报位置";
			public const string Button_EditAlertPosition = "编辑警报位置";
			public const string Button_ResetAlertPosition = "重置位置";

			public const string Header_Menu = "菜单自定义";
			public const string Desc_Menu = "自定义菜单背景和音乐设置。";
			public const string Header_MenuBackground = "菜单背景";
			public const string Toggle_CustomBackground = "自定义背景";
			public const string Header_MusicSettings = "音乐设置";
			public const string Toggle_DisableIngameMusic = "在游戏中禁用音乐";
			public const string Toggle_MuteTunnel = "静音嘉年华隧道";
			public const string Toggle_RememberMusic = "记住菜单音乐";

			public const string Header_Settings = "客户端设置";
			public const string Desc_Settings = "自定义菜单快捷键与界面外观。";
			public const string Header_Hotkeys = "客户端快捷键";
			public const string Header_ThemeColor = "主题颜色";
			public const string Desc_ThemeColor = "调整色相以更改全部界面颜色。";
			public const string Header_Miscellaneous = "高级";

			public const string ThemeTabsHueFormat = "标签: {0}°";
			public const string ThemeBackgroundHueFormat = "背景: {0}°";
			public const string Toggle_DarkMode = "深色模式";
			public const string Toggle_NoBackground = "无背景";
			public const string LanguageLabel = "语言: 中文";

			public const string InfoOverlayTitle = "说明";

			public const string Header_VisibleCategories = "可见标签";
			public const string Desc_VisibleCategories = "选择要在客户端中显示的功能标签。";
		}

		public static class MenuText
		{
			public const string Town = "小镇";
			public const string Manor = "庄园";
			public const string Farmhouse = "农舍";
			public const string Asylum = "精神病院";
			public const string Inn = "旅馆";
			public const string Slaughterhouse = "屠宰场";
			public const string Carnival = "嘉年华";
		}

		public static class Anti
		{
			public const string UnknownName = "未知";
			public const string SuspiciousSpeedFormat = "{0} 可疑速度! 平均={1:F2} 米/秒";
			public const string AlertsTitle = "⚠ 速度监控 可疑玩家";
		}

		public static class Tabs
		{
			public const string Optimize = "优化";
			public const string HSV = "外观";
			public const string Speedrun = "速通";
			public const string FOV = "视野";
			public const string Anticheat = "反作弊";
			public const string Menu = "菜单";
		}

		public static class InfoText
		{
			public const string Optimize = @"
{B}<b><color={ACCENT}>渲染距离:</color></b> 调整地图物体的渲染距离, 可在游戏内和菜单中生效。

<b><color={WARN}>警告:</color></b> 使用高亮道具的天赋 (例如 <b>Inspired</b>) 时, 只有当你足够靠近物体时, 高亮才会出现。
建议为<b>渲染距离</b>绑定一个<b>快捷键</b>, 需要时可以快速切换, 在几秒内暂时恢复完整视野。

{B}<b><color={ACCENT}>禁用天气效果:</color></b> 移除所有天气效果 (雨, 雪, 风), 并可选择静音相关音效。只能在主菜单或大厅中启用。

<b><color={WARN}>警告:</color></b> 如果在过场动画中停留时间过长, 天气可能无法被正常关闭。";

			public const string HSV = @"
<b>你可以使用预览上方的按钮, 在 {B}<b><color={ACCENT}>服装</color></b> 和 {B}<b><color={ACCENT}>图标</color></b> 的 HSV 之间切换。</b>

{B}<b><color={ACCENT}>图标:</color></b> 根据你的等级在 70 到 666 之间选择图标样式, 并自定义图标的颜色。使用交换按钮上方的小按钮循环可用图标。

{B}<b><color={ACCENT}>服装:</color></b> 一次性调整所有玩家服装的 <b>HSV</b> (色相, 饱和度, 明度)。

<b><color={WARN}>警告:</color></b> 为显著提升性能, 某些地图上的服装扫描可能会花费更长时间。

{B}这些是基于色相叠加的视觉效果, 而不是重新着色的贴图, 因此某些颜色看起来可能与预期略有差异。

{B}<b>所有外观更改仅对本地可见, 其他玩家无法看到。</b>";

			public const string Speedrun = @"
<b><color={DANGER}>免责声明</color></b>: 请合理使用这些改动。不要在与陌生人的休闲对局中使用, 否则可能会被视为作弊。利用本模块刷等级或获取不公平优势属于作弊行为。这些功能仅为娱乐和正规速通而设计, 而不是用来伤害或压制其他玩家。

{B}<b><color={ACCENT}>瞬时互动:</color></b> 移除所有长按交互 (复活, 仪式, 牢笼等)。

{B}请确保你设置的<b>交互按键</b>与游戏内的交互键完全一致, 否则功能可能无法正常工作。

{B}<b><color={ACCENT}>阁楼出生:</color></b> 复现旧版农舍中使用安娜时从阁楼出生的漏洞。仅影响农舍地图, 专为速通设计。

{B}<b><color={ACCENT}>自动开始:</color></b> 在返回大厅时自动开始对局。只能在菜单或大厅中启用, 以避免导致模组失效。

{B}<b><color={ACCENT}>开始延迟</color></b>: 在大厅中等待多久再开始, 可根据你电脑的加载速度调整。
{B}<b><color={ACCENT}>准备时间</color></b>: 一局结束后, 自动开始功能保持激活的时间长度。

<b><color={WARN}>警告:</color></b> 如果大厅加载时间过长 (取决于性能), <b><color={ACCENT}>自动开始</color></b> 可能会在大厅完全加载前触发, 从而导致模块失效。如果发生这种情况, 只需重新进入大厅, 或在单人时重新载入游戏即可。该模块在单人模式以及你作为房主时有效; 如果你不是房主, 它会失效并需要按上述方法重新恢复。";

			public const string FOV = @"
{B}启用后, 自定义视野会覆盖游戏内默认视野设置, 而不会更改游戏内视野滑块的数值。

{B}允许使用低于 <b><color={ACCENT}>60</color></b> 以及高于 <b><color={ACCENT}>95</color></b> 的视野值。

{B}当模块开启时, 惊吓演出与过场动画也会使用你的自定义视野。

{B}使用 UV 手电时, 不会引入额外的摄像机副作用。";

			public const string Settings = @"
{B}你可以设置一个<b><color={ACCENT}>自定义快捷键</color></b>来打开客户端, 该按键会自动保存。

<b><color={WARN}>提示:</color></b> 如果不小心将其绑定为<b>鼠标左键</b>, 可以通过<b>鼠标右键</b>重新打开界面并再次修改。

{B}<b><color={ACCENT}>主题颜色:</color></b> 自定义客户端在<b>标签</b>和<b>背景</b>上的强调色, 包括可启用的<b><color={ACCENT}>深色模式</color></b>以获得更暗的主题, 以及<b><color={ACCENT}>无背景</color></b>选项, 可以几乎完全移除窗口背景。

{B}在<b><color={ACCENT}>可见标签</color></b>中, 你可以启用或隐藏整页标签, 只保留自己关心的部分。

{B}<b><color={ACCENT}>语言</color></b>可以在英文和中文界面之间切换。

{B}<b><color={ACCENT}>重置客户端设置</color></b>会将所有设置恢复为默认值。

{B}如果界面不小心移出屏幕, 可以按<b>F1</b>将窗口重新居中。";

			public const string Anticheat = @"
{B}<b><color={ACCENT}>速度反作弊:</color></b> 持续监控所有玩家的<b>平均移动速度</b>, 当其在较长时间内明显高于原版阈值时, 会将其标记为可疑, 并在屏幕上显示带有名字和平均速度的警报。

{B}<b><color={ACCENT}>清除警报</color></b>会移除当前所有警告, 并重置每位玩家的警报标记。

{B}<b><color={ACCENT}>警报持续时间:</color></b> 设置每条警报在屏幕上显示的时间, 以及可疑玩家保持高亮的时长。

{B}<b><color={ACCENT}>编辑警报位置:</color></b> 允许你将警报框拖动到屏幕上的任意位置, 然后保存新位置。

{B}<b><color={ACCENT}>重置位置:</color></b> 将警报框恢复到默认位置。

<b><color={WARN}>说明:</color></b> 此模块仅提供<b>信息提示</b>, <b>不会踢出</b>任何玩家; 请将其作为判断可疑行为的辅助工具, 而不是直接证据。在场景切换或使用<b>清除警报</b>时, 警报与跟踪数据会被重置。";

			public const string Menu = @"
{B}<b><color={ACCENT}>菜单背景:</color></b> 将默认菜单背景替换为任意大厅环境。

<b><color={WARN}>说明:</color></b> 当你处于大厅中时, 虽然背景按钮依然可见, 但必须返回主菜单才能切换背景。

{B}<b><color={ACCENT}>记住菜单音乐:</color></b> 重启游戏后, 将再次使用你上次选择的<b>菜单音乐</b>。

{B}<b><color={ACCENT}>在游戏中禁用音乐:</color></b> 关闭所有地图的背景音乐, 但保留菜单音乐。

{B}<b><color={ACCENT}>静音嘉年华隧道:</color></b> 关闭<b>嘉年华</b>地图中蓝色旋转隧道的音乐。";
		}
	}
}