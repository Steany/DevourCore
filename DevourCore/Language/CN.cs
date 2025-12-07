using UnityEngine;

namespace DevourCore
{
    public static class CN
    {
        public static class GUI
        {
            // Tab titles
            public const string Tab_Optimize = "优化";
            public const string Tab_HSV = "外观";
            public const string Tab_Speedrun = "速通";
            public const string Tab_FOV = "视野";
            public const string Tab_Anticheat = "反作弊";
            public const string Tab_Menu = "菜单";
            public const string Tab_Settings = "设置";

            // Generic / shared
            public const string PressAnyKey = "按任意键...";
            public const string ToggleKeyFormat = "切换键: {0}";
            public const string InteractKeyFormat = "交互键: {0}";
            public const string MenuKeyFormat = "打开菜单: {0}";
            public const string CurrentClipFormat = "当前: {0}";
            public const string AreYouSure = "确定吗？";
            public const string ResetClientSettings = "重置客户端设置";

            // Optimize tab
            public const string Header_Optimize = "性能优化";
            public const string Desc_Optimize = "自定义渲染距离并禁用天气效果。";
            public const string Header_Cull = "渲染距离";
            public const string Toggle_CullEnabled = "启用渲染距离";
            public const string Toggle_CullInMenu = "在菜单中启用";
            public const string CullDistanceFormat = "距离: {0} 米";
            public const string Header_Weather = "天气";
            public const string Toggle_DisableWeather = "禁用天气效果";

            // HSV / Outfit tab
            public const string Header_OutfitColor = "服装颜色";
            public const string Header_IconColor = "666 图标颜色";
            public const string Desc_OutfitColor = "更改所有玩家服装的颜色。";
            public const string Desc_IconColor = "自定义 666 等级图标的颜色。";
            public const string Toggle_IconEnabled = "启用 666 图标";
            public const string Toggle_OutfitEnabled = "启用服装";
            public const string Swap_ToOutfit = "服装 ↔";
            public const string Swap_ToIcon = "666 ↔";
            public const string Header_Preview = "预览";

            // Speedrun tab
            public const string Header_Speedrun = "速通功能";
            public const string Desc_Speedrun = "用于速通分类的实用功能。";
            public const string Toggle_InstantInteract = "瞬时互动";
            public const string Toggle_AtticSpawn = "阁楼出生点 (农舍)";
            public const string Header_AutoStart = "自动开始";
            public const string Desc_AutoStart = "返回大厅时自动开始游戏。";
            public const string Toggle_ForceStart = "启用自动开始";
            public const string Toggle_UseArm = "启用准备时间窗口";
            public const string ForceStartDelayFormat = "开始延迟: {0:F1} 秒";
            public const string ForceStartArmFormat = "准备时间: {0:F1} 分钟";
            public const string SpeedrunPopupBody =
                "请合理使用这些修改功能。避免在与不认识的玩家进行的休闲对局中使用，否则可能会被视为作弊。利用这些功能刷等级或获得不公平优势也被视为作弊。这些功能仅用于娱乐和速通目的，而非伤害或干扰其他玩家。请谨慎使用。";
            public const string SpeedrunPopupConfirm = "已了解";

            // FOV tab
            public const string Header_FOV = "自定义视野";
            public const string Desc_FOV = "将视野调整到高于正常限制。";
            public const string Toggle_FOVEnabled = "启用自定义视野";
            public const string FOVValueFormat = "视野: {0}°";

            // Anticheat tab
            public const string Header_Anticheat = "速度反作弊";
            public const string Desc_Anticheat = "监控玩家是否存在可疑移动速度。";
            public const string Toggle_AnticheatEnabled = "启用速度检测";
            public const string AlertDurationFormat = "警报持续时间: {0:F1} 秒";
            public const string Button_ClearAlerts = "清除警报";
            public const string AnticheatStatusFormat = "正在监控 {0} 名玩家 | {1} 个活动警报";

            // Menu tab
            public const string Header_Menu = "菜单自定义";
            public const string Desc_Menu = "自定义菜单背景和音乐设置。";
            public const string Header_MenuBackground = "菜单背景";
            public const string Toggle_CustomBackground = "启用自定义背景";
            public const string InLobby_NoBgChange = "在大厅中时无法更改背景。";
            public const string Header_MusicSettings = "音乐设置";
            public const string Toggle_DisableIngameMusic = "在游戏中禁用音乐";
            public const string Toggle_MuteTunnel = "静音隧道（嘉年华）";
            public const string Toggle_RememberMusic = "记住菜单音乐";

            // Settings tab
            public const string Header_Settings = "客户端设置";
            public const string Desc_Settings = "自定义菜单快捷键和外观。";
            public const string Header_Hotkeys = "菜单快捷键";
            public const string Header_ThemeColor = "主题颜色";
            public const string Desc_ThemeColor = "调整色相以改变整个界面的主题颜色。";
            public const string Header_Miscellaneous = "杂项";

            // Info overlay title
            public const string InfoOverlayTitle = "说明";

            // Visible categories (for Tabs)
            public const string Header_VisibleCategories = "可见标签";
            public const string Desc_VisibleCategories = "选择要在客户端中显示的标签。";
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
            public const string SuspiciousSpeedFormat = "{0} - 可疑速度! 平均={1:F2} 米/秒";
            public const string AlertsTitle = "⚠ 速度监控 - 可疑玩家";
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
{B}当你使用<b><color={ACCENT}>渲染距离</color></b>并同时使用高亮道具天赋时，只有在靠近物体时高亮才会出现。
建议为<b><color={ACCENT}>渲染距离</color></b>绑定一个快捷键，需要时可以暂时关闭限制，几秒内恢复完整视野。

{B}<b><color={ACCENT}>禁用天气效果</color></b>只能在主菜单或大厅中使用，以避免在地图中出现异常。

<b><color={WARN}>警告：</color></b>如果<b><color={ACCENT}>禁用天气效果</color></b>一直开启，并且在过场动画中停留时间过长（超过 12 秒），天气可能无法被正常关闭。";

            public const string HSV = @"
<b>你可以通过上方的按钮在服装颜色与 666 图标颜色之间切换。</b>

{B}<b><color={ACCENT}>666 图标：</color></b>为所有 666 等级图标（包括你自己）自定义颜色。

<b><color={WARN}>警告：</color></b>只有当你的等级为<b><color={ACCENT}>666</color></b>时，此功能才会生效，否则界面不会发生变化。

{B}<b><color={ACCENT}>服装：</color></b>一次性调整所有玩家服装的<b><color={ACCENT}>色相、饱和度与明度</color></b>。

<b><color={WARN}>警告：</color></b><b><color={ACCENT}>服装</color></b>模块在某些电脑上可能会导致明显的卡顿或帧数下降。如果感觉过于卡顿，可以随时关闭该模块。为显著提升性能，某些地图中的服装扫描可能会花费更长时间。

{B}这些修改是基于颜色叠加的视觉效果，而不是真正重做贴图，因此某些颜色看起来可能与预期略有差异。

{B}<b>所有外观更改仅对本地可见，其他玩家无法看到。</b>";

            public const string Speedrun = @"
<b><color={DANGER}>免责声明</color></b>：请合理使用这些改动。不要在与陌生人的休闲对局中使用，否则可能会被视为作弊。利用本模块加速升级或获得不公平优势属于作弊行为。这些功能仅为娱乐和正规的速通玩法而设计，不应用来伤害或压制其他玩家。

{B}<b><color={ACCENT}>瞬时互动：</color></b>移除所有长按交互，例如复活、仪式、牢笼等操作。

{B}请确保设置的<b><color={ACCENT}>交互按键</color></b>与游戏内的交互按键完全一致，否则功能可能无法正常工作。

{B}<b><color={ACCENT}>阁楼出生：</color></b>在农舍中复现旧版本使用安娜时从阁楼出生的旧漏洞。该功能仅作用于农舍地图，专门用于速通。

{B}<b><color={ACCENT}>自动开始：</color></b>在回到大厅时自动开始游戏。只能在菜单或大厅中启用，以防止模块失效。

 • <b><color={ACCENT}>开始延迟</color></b>：在大厅等待多久再开始，可根据你电脑的加载速度调整。
 
• <b><color={ACCENT}>准备时间</color></b>：一局结束后，自动开始功能保持激活的时间长度。

<b><color={WARN}>警告：</color></b>如果大厅加载时间过长（性能不足），<b><color={ACCENT}>自动开始</color></b>可能会在大厅完全加载前触发，导致模块失效。如果发生这种情况，只需重新进入大厅，或在单人时重新载入游戏即可。该模块在单人模式以及你作为房主时有效。如果你不是房主，模块会失效并需要同样的方法重新恢复。";

            public const string FOV = @"
{B}允许使用低于<b><color={ACCENT}>60</color></b>以及高于<b><color={ACCENT}>95</color></b>的视野值，而无需修改游戏内视野设置。

{B}当模块开启时，惊吓动画与过场画面也会使用当前的自定义视野值。

{B}使用 UV 手电时，不会引入额外的摄像机副作用。";

            public const string Settings = @"
{B}你可以为打开客户端设置一个自定义<b><color={ACCENT}>快捷键</color></b>，该按键会自动保存。

<b><color={WARN}>提示：</color></b>如果不小心将快捷键设置为鼠标左键，可以通过鼠标右键重新打开界面并修改按键。

{B}你可以使用<b><color={ACCENT}>重置客户端设置</color></b>按钮，将所有设置恢复为默认值。

{B}<b><color={ACCENT}>主题色调</color></b>滑块可以改变整个客户端的主色调，用来匹配你喜欢的配色风格。

{B}在<b><color={ACCENT}>可见标签</color></b>中，你可以启用或隐藏整个功能标签，只保留自己常用的部分。

{B}<b><color={ACCENT}>语言</color></b>可在英文和中文之间切换界面语言。

{B}<b><color={ACCENT}>重置客户端设置</color></b>会将所有设置恢复为默认值。

{B}如果界面不小心移出屏幕，可以按<b><color={ACCENT}>F1</color></b>将窗口重新居中。";

            public const string Anticheat = @"
{B}<b><color={ACCENT}>速度反作弊</color></b>会持续监控所有玩家，并估算他们的<b>平均移动速度</b>。

{B}如果某名玩家的正常移动速度在较长时间内持续明显高于阈值，模块会将其标记为可疑，在屏幕上显示带有玩家名字和平均速度的警告提示，并在本地覆盖层中高亮该玩家，持续到警告结束。

<b><color={WARN}>说明：</color></b>此模块仅提供<b>信息提示</b>，<b>不会踢出或封禁</b>任何玩家。它只是辅助你判断可疑行为，而不是证据本身。在场景切换或使用<b>清除警报</b>后，所有提示与统计都会重置。

{B}<b><color={ACCENT}>清除警报</color></b>会移除当前所有警报，并重置每个玩家的警报标记，使他们在再次持续超速时能够重新被标记。";

            public const string Menu = @"
{B}<b><color={ACCENT}>菜单背景：</color></b>将默认菜单背景替换为任意大厅环境。

<b><color={WARN}>说明：</color></b>在大厅中时，背景按钮仍会显示，但必须返回主菜单才能真正切换背景。在开始游戏或加入大厅时，摄像机也可能出现短暂的异常。

{B}<b><color={ACCENT}>在游戏中禁用音乐：</color></b>关闭所有地图的背景音乐，但保留主菜单的音乐。

{B}<b><color={ACCENT}>静音隧道（嘉年华）：</color></b>只关闭嘉年华旋转蓝色隧道中的音乐。

{B}<b><color={ACCENT}>记住上次音乐：</color></b>重启游戏后仍会记住你上次选择的<b><color={ACCENT}>菜单音乐</color></b>。";
        }
    }
}