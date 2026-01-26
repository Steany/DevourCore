using UnityEngine;

namespace DevourCore
{
	public static class PT
	{
		public static class GUI
		{
			public const string Tab_Optimize = "Otimizar";
			public const string Tab_HSV = "HSV";
			public const string Tab_Speedrun = "Speedrun";
			public const string Tab_FOV = "FOV";
			public const string Tab_Anticheat = "Anti-cheat";
			public const string Tab_Menu = "Menu";
			public const string Tab_Settings = "Opções";

			public const string PressAnyKey = "Pressione qualquer tecla...";
			public const string ToggleKeyFormat = "Toggle Tecla: {0}";
			public const string InteractKeyFormat = "Interagir Tecla: {0}";
			public const string MenuKeyFormat = "Abrir Client: {0}";
			public const string CurrentClipFormat = "Atual: {0}";
			public const string AreYouSure = "Tem certeza?";
			public const string ResetClientSettings = "Resetarar Client Configurações";

			public const string Header_Optimize = "Performance Otimizaçãos";
			public const string Desc_Optimize = "Personalizadoize render distance and disable weather effects";
			public const string Header_Cull = "Distância de renderização";
			public const string Toggle_CullEnabled = "Distância de renderização";
			public const string Toggle_CullInMenu = "Ativar in Menu";
			public const string CullDistanceFormat = "Distância: {0} m";
			public const string Header_Weather = "Clima";
			public const string Toggle_DisableWeather = "Desativar Efeitos Climáticos";
			public const string Toggle_MuteWeather = "Mutar Clima Sons";

			public const string Header_OutfitColor = "Cor da Roupa";
			public const string Header_IconColor = "Cor do Ícone";
			public const string Desc_OutfitColor = "Altere a cor de todas as roupas dos jogadores";
			public const string Desc_IconColor = "Personalize a cor do ícone 666";
			public const string Toggle_IconEnabled = "Ícone HSV";
			public const string Toggle_OutfitEnabled = "Roupa HSV";
			public const string Swap_ToOutfit = "Roupa ↔";
			public const string Swap_ToIcon = "Ícone ↔";
			public const string Header_Preview = "Pré-visualização";


			public const string Header_Speedrun = "Mods de Speedrun";
			public const string Desc_Speedrun = "Recursos úteis para categorias de speedrun";
			public const string Toggle_InstantInteract = "Interação Instantânea";
			public const string Toggle_AtticSpawn = "Spawn no Sótão";
			public const string Header_AutoStart = "Início Automático";
			public const string Desc_AutoStart = "Inicia o jogo automaticamente ao retornar ao lobby";
			public const string Toggle_ForceStart = "Ativar Início Automático";
			public const string Toggle_UseArm = "Tempo limite do Início Automático";
			public const string ForceStartDelayFormat = "Atraso de início: {0:F1}s";
			public const string ForceStartArmFormat = "Tempo limite do gatilho: {0:F1} min";
			public const string SpeedrunPopupBody =
				"Por favor, use essas modificações de forma responsável. Evite usá-las em partidas casuais com jogadores que você não conhece, pois podem ser vistas como trapaça. Explorar esses recursos para ganhar níveis ou vantagens injustas é considerado trapaça. Esses mods são destinados apenas para diversão e speedrunning, sem causar prejuízos ou desvantagens a outros jogadores. Use com cuidado.";
			public const string SpeedrunPopupConfirm = "Entendido";


			public const string Header_FOV = "FOV Personalizado";
			public const string Desc_FOV = "Personalize seu campo de visão além do limite normal";
			public const string Toggle_FOVEnabled = "Ignorar limite de FOV";
			public const string FOVValueFormat = "FOV: {0}°";

			public const string Header_Anticheat = "Anticheat de Velocidade";
			public const string Desc_Anticheat = "Monitora jogadores com velocidade de movimento suspeita";
			public const string Toggle_AnticheatEnabled = "Ativar Detecção";
			public const string AlertDurationFormat = "Duração do alerta: {0:F1}s";
			public const string Button_ClearAlerts = "Limpar alertas";
			public const string AnticheatStatusFormat = "Monitorando {0} jogadores | {1} alertas ativos";
			public const string Button_SaveAlertPosition = "Salvar posição do alerta";
			public const string Button_EditAlertPosition = "Editar posição do alerta";
			public const string Button_ResetAlertPosition = "Redefinir posição";

			public const string Header_Menu = "Menu Personalizado";
			public const string Desc_Menu = "Personalize o fundo do menu e as configurações de música";
			public const string Header_MenuBackground = "Fundo do Menu";
			public const string Toggle_CustomBackground = "Fundo personalizado";
			public const string Header_MusicSettings = "Configurações de Áudio";
			public const string Toggle_DisableIngameMusic = "Desativar música do jogo";
			public const string Toggle_MuteTunnel = "Mutar túnel do carnaval";
			public const string Toggle_RememberMusic = "Lembrar música do menu";

			public const string Header_Settings = "Configurações do Cliente";
			public const string Desc_Settings = "Personalize atalhos e aparência do menu";
			public const string Header_Hotkeys = "Atalhos do Cliente";
			public const string Header_ThemeColor = "Aparência";
			public const string Desc_ThemeColor = "Ajuste o matiz para alterar todas as cores da interface";
			public const string Header_Miscellaneous = "Avançado";


			public const string ThemeTabsHueFormat = "Abas: {0}°";
			public const string ThemeBackgroundHueFormat = "Fundo: {0}°";
			public const string Toggle_DarkMode = "Modo escuro";
			public const string Toggle_NoBackground = "Sem fundo";
			public const string LanguageLabel = "Idioma: Português (PT-BR)";

			public const string InfoOverlayTitle = "Informações";

			public const string Header_VisibleCategories = "Categorias Visíveis";
			public const string Desc_VisibleCategories = "Escolha quais abas ficam visíveis no cliente.";

			public const string HueFormat = "Matiz: {0}°";
			public const string HudOpacityFormat = "Opacidade: {0}%";
			public const string Toggle_HudChroma = "Croma";
			public const string UnknownSymbol = "?";

			public const string Tab_Gameplay = "Jogo";
			public const string Tab_HUD = "HUD";

			public const string Slider_H = "H";
			public const string Slider_S = "S";
			public const string Slider_V = "V";

			public const string Header_LookBack = "Olhar para Trás";
			public const string Toggle_EnableLookBack = "Ativar Olhar para Trás";
			public const string Toggle_LookBackToggleMode = "Modo Alternar";
			public const string LookBackKeyFormat = "Tecla de olhar para trás: {0}";
			public const string LookBackNotAvailable = "Recurso de olhar para trás não disponível.";

			public const string Header_Audio = "Áudio";
			public const string Toggle_MuteCarnivalClock = "Mutar sons do relógio do Carnaval";

			public const string Header_HUD = "HUD";
			public const string Button_Back = "Voltar";
			public const string Hud_EnableFps = "Ativar FPS";
			public const string Hud_HidePrefix = "Ocultar prefixo";
			public const string Hud_InvertPrefix = "Inverter prefixo";
			public const string Hud_UppercasePrefix = "Prefixo em maiúsculas";
			public const string Hud_EnableCps = "Ativar CPS";
			public const string Hud_DualCps = "CPS duplo";
			public const string Hud_BindAFormat = "Atalho A: {0}";
			public const string Hud_BindBFormat = "Atalho B: {0}";
			public const string Hud_ShowCoordinates = "Mostrar coordenadas";
			public const string Hud_ShowPrefix = "Mostrar prefixo";
			public const string Hud_VerticalPosition = "Posição vertical";
			public const string Hud_ShowEnrageStatus = "Mostrar status de Enrage";
			public const string Hud_NoBackground = "Sem fundo";
			public const string Hud_EnableSpeedDetector = "Ativar detector de velocidade";
			public const string Hud_WarningDurationFormat = "Duração do aviso: {0:0.0}s";
			public const string Hud_AlertsFormat = "Alertas: {0} | Monitorando: {1}";
			public const string Hud_ClearAlerts = "Limpar alertas";
			public const string Hud_ClearAlertsCountFormat = "Limpar alertas ({0})";
			public const string Hud_ShowGameTime = "Mostrar tempo de jogo";
			public const string Hud_ShowLeadingMinutes = "Mostrar zeros nos minutos";
			public const string Hud_ShowDecimals = "Mostrar decimais";
			public const string Hud_DecimalsAmountFormat = "Quantidade de decimais: {0}";
			public const string Hud_FpsCounter = "Contador FPS";
			public const string Hud_CpsCounter = "Contador CPS";
			public const string Hud_Coordinates = "Coordenadas";
			public const string Hud_EnrageStatus = "Estado Enrage";
			public const string Hud_SpeedDetector = "Velocidade";
			public const string Hud_GameTime = "Tempo de jogo";
			public const string Hud_Size = "Tamanho do HUD";
			public const string Hud_SavePositions = "Salvar posições";
			public const string Hud_EditPositions = "Editar posições";
			public const string Hud_ResetPositions = "Redefinir posições";

			public const string Header_Appearence = "Aparência";
			public const string BackgroundOpacityFormat = "Opacidade do fundo: {0}%";
			public const string Toggle_DefaultBackground = "Fundo padrão";
			public const string Toggle_ChromaMode = "Modo Chroma";
			public const string Button_CustomizeAppearence = "Personalizar aparência";
			public const string Toggle_ClickSounds = "Sons de clique";
			public const string Toggle_ButtonShadow = "Sombra dos Botões";

			public const string Stun_Calm = "CALMO";
			public const string Stun_Enraged = "ENFURECIDO";
			public const string Stun_RedEyes = "OLHOS VERMELHOS";

			public const string Hud_FpsUpper = "FPS";
			public const string Hud_FpsLower = "fps";
			public const string AxisZLower = "z";
			public const string AxisZUpper = "Z";
			public const string AxisYLower = "y";
			public const string AxisYUpper = "Y";
			public const string AxisXLower = "x";
			public const string AxisXUpper = "X";
			public const string Hud_CpsLower = "cps";
			public const string Hud_CpsUpper = "CPS";

			public const string WelcomeBodyFormat = "Bem-vindo ao DevourCore!\nPressione '{0}' para abrir o Cliente.";
			public const string WelcomeButtonNext = "Próximo";
			public const string HelpBody = "Para explicações dos mods em cada categoria, passe o mouse sobre o ícone de informações no canto inferior direito da interface. Você também pode arrastar a interface para posicioná-la onde quiser.";
			public const string HelpButtonEnjoy = "Aproveite <3";

			public static class MenuText
			{
				public const string Town = "Circo";
				public const string Manor = "Mansão";
				public const string Farmhouse = "Fazenda";
				public const string Asylum = "Manicômio";
				public const string Inn = "Pousada";
				public const string Slaughterhouse = "Abatedouro";
				public const string Carnival = "Carnaval";
			}

			public static class Anti
			{
				public const string UnknownName = "Desconhecido";
				public const string SuspiciousSpeedFormat = "{0} - velocidade suspeita! média={1:F2} m/s";
				public const string AlertsTitle = "⚠ VelocidadeWatch - Jogadores Suspeitos";

				public const string SuspiciousPlayersTitle = "Jogadores Suspeitos";
				public const string DragBoxHint = "Arraste esta caixa para definir a posição do alerta";
				public const string BulletPrefix = "• ";
			}

			public static class Tabs
			{
				public const string Optimize = "Otimizar";
				public const string HSV = "HSV";
				public const string Speedrun = "Speedrun";
				public const string FOV = "FOV";
				public const string Anticheat = "Antitrapaça";
				public const string Menu = "Menu";
			}


public const string Button_HudTextStyleFormat = "Estilo do Texto: {0}";
public const string Button_UiTextStyleFormat = "Estilo do Texto da UI: {0}";
public const string TextStyle_Outline = "Contorno";
public const string TextStyle_Shadow = "Sombra";
		}

		public static class MenuText
		{
			public const string Town = GUI.MenuText.Town;
			public const string Manor = GUI.MenuText.Manor;
			public const string Farmhouse = GUI.MenuText.Farmhouse;
			public const string Asylum = GUI.MenuText.Asylum;
			public const string Inn = GUI.MenuText.Inn;
			public const string Slaughterhouse = GUI.MenuText.Slaughterhouse;
			public const string Carnival = GUI.MenuText.Carnival;
		}

		public static class Anti
		{
			public const string UnknownName = GUI.Anti.UnknownName;
			public const string SuspiciousSpeedFormat = GUI.Anti.SuspiciousSpeedFormat;
			public const string AlertsTitle = GUI.Anti.AlertsTitle;
			public const string SuspiciousPlayersTitle = GUI.Anti.SuspiciousPlayersTitle;
			public const string DragBoxHint = GUI.Anti.DragBoxHint;
			public const string BulletPrefix = GUI.Anti.BulletPrefix;
		}

		public static class Tabs
		{
			public const string Optimize = GUI.Tabs.Optimize;
			public const string HSV = GUI.Tabs.HSV;
			public const string Speedrun = GUI.Tabs.Speedrun;
			public const string FOV = GUI.Tabs.FOV;
			public const string Anticheat = GUI.Tabs.Anticheat;
			public const string Menu = GUI.Tabs.Menu;
		}

		public static class InfoText
		{
			public const string Optimize = @"
{B}<b><color={ACCENT}>Distância de Renderização:</color></b> Ajusta o quão longe os objetos do mapa são renderizados. Funciona dentro do jogo e no menu.

<b><color={WARN}>Aviso:</color></b> Qualquer perk que destaque itens (por exemplo, <b>Inspired</b>) só aparecerá quando você estiver próximo a eles.
Recomenda-se atribuir uma <b>tecla</b> para alternar rapidamente a distância de renderização e restaurar temporariamente a visibilidade total por alguns segundos.

{B}<b><color={ACCENT}>Desativar Efeitos Climáticos:</color></b> Remove todos os efeitos de clima (chuva, neve, vento). Só pode ser ativado pelo menu ou lobby.

<b><color={WARN}>Aviso:</color></b> Se você permanecer em cutscenes por muito tempo, o clima não será desativado.";

			public const string HSV = @"
{B}<b>Você pode alternar entre <b><color={ACCENT}>Roupa:</color></b> e <b><color={ACCENT}>Ícone:</color></b> HSV usando o botão acima da pré-visualização.</b>

{B}<b><color={ACCENT}>Ícone:</color></b> Personaliza a cor do seu ícone e permite selecionar (com base no seu nível) entre 70 e 666. Use o botão acima do botão de troca para alternar entre ícones.

{B}<b><color={ACCENT}>Roupa:</color></b> Ajusta o <b>HSV</b> (matiz, saturação e valor) de todas as roupas dos jogadores ao mesmo tempo.

<b><color={WARN}>Aviso:</color></b> Para melhorar drasticamente o desempenho, a varredura de roupas pode demorar um pouco mais em alguns mapas.

{B}Essas são sobreposições visuais de matiz em vez de recolorizações permanentes, então certas cores podem parecer ligeiramente diferentes do esperado.

{B}<b>Todas as alterações são puramente visuais e não são visíveis para outros jogadores.</b>";

			public const string Speedrun = @"
<b><color={DANGER}>Aviso</color></b>: Por favor, use essas modificações de forma responsável. Evite usá-las em partidas casuais com jogadores que você não conhece, pois podem ser consideradas trapaça. Subir de nível rapidamente ou obter vantagens injustas com esses recursos é trapaça. Esses mods são destinados apenas para diversão e speedrunning legítimo, não para prejudicar ou oprimir outros jogadores.

{B}<b><color={ACCENT}>Interação Instantânea:</color></b> Remove todas as interações longas (reviver, rituais, gaiolas, etc).

{B}Certifique-se de que sua <b>tecla de interação</b> configurada corresponda exatamente à tecla de interação do jogo, ou o recurso pode não funcionar.

{B}<b><color={ACCENT}>Spawn no Sótão:</color></b> Recria o antigo bug do Farmhouse onde usar a Anna fazia você nascer no sótão, além de permitir que funcione com <b>qualquer personagem</b>. Também restaura o comportamento antigo das portas, ou seja, portas abertas antes de ativar a Anna permanecerão abertas em vez de fechar automaticamente.

{B}<b><color={ACCENT}>Início Automático:</color></b> Inicia partidas automaticamente a partir do lobby, com um <b><color={ACCENT}>Atraso de Início</color></b> para aguardar antes de começar e um <b><color={ACCENT}>Tempo Limite de Início Automático</color></b> que desativa o Início Automático após o tempo do slider ter passado, evitando que o jogo inicie automaticamente na próxima vez que você entrar em um lobby.

<b><color={WARN}>Aviso:</color></b> Se o lobby demorar muito para carregar (dependendo do desempenho), o <b><color={ACCENT}>Início Automático</color></b> pode ser acionado antes que o lobby esteja totalmente carregado, fazendo com que o mod quebre. Se isso acontecer, basta reentrar no lobby ou recarregar o modo solo se estiver jogando sozinho. O mod funciona em singleplayer e como host. Se você não for o host, ele quebrará e exigirá a mesma correção acima.";

			public const string FOV = @"
{B}Quando ativado, alterna a substituição personalizada de FOV sem alterar sua configuração normal de FOV dentro do jogo.

{B}Permite valores abaixo de <b><color={ACCENT}>60</color></b> e acima de <b><color={ACCENT}>95</color></b> sem alterar o slider de FOV do jogo.

{B}Quando ativado, jumpscares e cutscenes também usam seu FOV personalizado.

{B}Usar UV não causa efeitos colaterais adicionais na câmera.";

			public const string Settings = @"
{B}Você pode definir uma <b>tecla personalizada</b> para abrir o cliente.

<b><color={WARN}>Nota:</color></b> Se você acidentalmente vinculá-la ao <b>Botão Esquerdo do Mouse</b>, pode alterá-la novamente usando o <b>Botão Direito do Mouse</b>.

<b><color={ACCENT}>Aparência:</color></b> Personalize o visual do cliente com cores para abas, caixas de seleção e controles deslizantes, cor e opacidade do fundo personalizadas, <b><color={ACCENT}>Fundo Padrão</color></b>, <b><color={ACCENT}>Abas Escuras</color></b>, <b><color={ACCENT}>Modo Chroma</color></b>, <b><color={ACCENT}>Sons de Clique</color></b>, <b><color={ACCENT}>Sombra dos Botões</color></b> do jogo, e a opção entre estilos de texto com <b><color={ACCENT}>Contorno</color></b> ou <b><color={ACCENT}>Sombra</color></b>.

{B}Em <b><color={ACCENT}>Categorias Visíveis</color></b>, você pode ativar ou desativar abas inteiras para que apenas as que você deseja permaneçam visíveis.

{B}<b><color={ACCENT}>Idioma</color></b> alterna entre Inglês, Chinês e Português.

{B}<b><color={ACCENT}>Redefinir Configurações do Cliente</color></b> restaura todas as configurações para seus valores padrão.

{B}Se a interface sair da tela, pressione <b>F1</b> para recentralizá-la.";

			public const string Anticheat = @"
{B}<b><color={ACCENT}>Anticheat de Velocidade:</color></b> Monitora a <b>velocidade média de movimento</b> dos jogadores e sinaliza aqueles que excedem o limite padrão do jogo, exibindo um alerta na tela com seu nome e velocidade.

{B}<b><color={ACCENT}>Limpar Alertas</color></b> remove todos os avisos atuais e redefine os marcadores de alerta por jogador.

{B}<b><color={ACCENT}>Duração do Alerta:</color></b> Define por quanto tempo cada aviso permanece na tela e por quanto tempo um jogador suspeito permanece destacado.

{B}<b><color={ACCENT}>Editar Posição do Alerta:</color></b> Permite arrastar a caixa de alerta para qualquer lugar da tela e salvar sua nova posição.

{B}<b><color={ACCENT}>Redefinir Posição:</color></b> Restaura a caixa de alerta para sua localização padrão.

<b><color={WARN}>Nota:</color></b> Este mod é apenas <b>informativo</b>. Ele <b>nunca expulsa</b> jogadores; use-o como uma ferramenta para ajudar a julgar comportamentos suspeitos. Alertas e rastreamento são redefinidos ao trocar de cena ou quando você usa <b>Limpar Alertas</b>.";

			public const string Menu = @"
{B}<b><color={ACCENT}>Fundo do Menu:</color></b> Substitui o fundo padrão do menu por qualquer um dos ambientes do lobby.

<b><color={WARN}>Nota:</color></b> Enquanto você estiver em um lobby, a seleção de fundo não pode ser alterada até que você retorne ao <b>menu principal</b>.

{B}<b><color={ACCENT}>Lembrar Música do Menu:</color></b> Após reiniciar o jogo, a última <b>música do menu</b> escolhida será usada novamente.";

			public const string Gameplay = @"
{B}<b><color={ACCENT}>Ignorar Limite de FOV:</color></b> Permite ir abaixo do mínimo normal e acima do máximo normal sem tocar no slider de FOV do jogo.

{B}Quando ativado, seu FOV escolhido também se aplica durante cutscenes e jumpscares.

{B}<b><color={ACCENT}>Olhar para Trás:</color></b> Adiciona uma forma rápida de olhar para trás enquanto corre.

{B}Você pode usá-lo como <b>Segurar</b> (ativo apenas enquanto mantém a tecla pressionada) ou <b>Modo Alternar</b> (pressione uma vez para travar, pressione novamente para voltar).

{B}<b><color={ACCENT}>Áudio</color></b> oferece algumas opções de qualidade de vida para som.

{B}<b>Silenciar Sons de Clima</b> remove a ambientação de chuva/vento.

{B}<b>Desativar Música no Jogo</b> desliga a música dos mapas, mantendo a música do menu.

{B}<b>Silenciar Túnel do Carnaval</b> remove o loop do túnel azul no Carnaval.

{B}<b>Silenciar Relógio do Carnaval</b> desativa o som ambiente do relógio no Carnaval.";

			public const string HUD = @"
{B}<b><color={ACCENT}>HUD:</color></b> Sobreposições personalizáveis na tela que você pode posicionar livremente.

{B}<b><color={ACCENT}>Contador de FPS:</color></b> Exibe sua taxa de quadros.

{B}<b><color={ACCENT}>Contador de CPS:</color></b> Exibe seus cliques por segundo, que você pode vincular a duas teclas de sua escolha (geralmente <b>ação</b> e <b>soltar</b>).

{B}<b><color={ACCENT}>Coordenadas:</color></b> Mostra sua posição dentro do jogo.

{B}<b><color={ACCENT}>Status de Enrage:</color></b> Mostra o estado atual de Anna (enfurecida, olhos vermelhos, calma).

{B}<b><color={ACCENT}>Detector de Velocidade:</color></b> Monitora a velocidade média de movimento dos jogadores em tempo real e envia um aviso quando os valores parecem suspeitos.

{B}<b><color={ACCENT}>Tempo de Jogo:</color></b> Mostra há quanto tempo a partida está em andamento.

{B}<b><color={ACCENT}>Tamanho do HUD:</color></b> Escala global para todos os elementos do HUD.";
		}
	}
}