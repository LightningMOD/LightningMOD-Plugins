namespace Turbo.Plugins.LightningMod
{
    using Turbo.Plugins.Default;
    using SharpDX.DirectInput;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    public class OpenGreatRiftPlugin : BasePlugin, IInGameTopPainter, IKeyEventHandler, INewAreaHandler, IAfterCollectHandler
    {
        public IKeyEvent ToggleKeyEvent { get; set; }
        protected IUiElement uiGRmainPage;
        protected IUiElement uiOnGreaterRift;
        protected IUiElement uiAcceptButton;
        protected IUiElement uiLeaveGame;
        protected IUiElement uiPlayGameButton;
        protected IUiElement uiChangeQuestButton;
        protected IUiElement uiOkButton;
        protected IUiElement uiJoinPaty;
        protected IUiElement uiJoinPatyAcceptButton;
        protected IUiElement uiResumeGame;
        protected IUiElement CloseGR;
        protected IUiElement uiMapname;
        private string OrekName = "";
        public IFont HeaderFont { get; private set; }
        public IFont InfoFont { get; private set; }
        private bool isEnteredGR = false;
        private bool isEntering = false;
        private bool isMultiplayergame = false;
        private bool isFoundObelisk = false;
        private bool isCaptain = false;
        private IWatch Timer;
        private IWatch Timer2;
        private IWatch Timer_clickclose;//关门后的检测
        private IWatch Timer_clickclose2;//点击NPC前的检测
        public bool Running { get; private set; }
        public List<Map> MapList { get; set; } = new List<Map>();
        public List<Map> SelectedMapList { get; set; } = new List<Map>();
        private string str_Info;
        private string str_Info2;
        private string str_Header;
        private string str_ForceMove;
        private string str_Running;
        public OpenGreatRiftPlugin()
        {
            Enabled = true;
        }
        private bool PlaygameButton()
        {
            var playgame = uiPlayGameButton.ReadText(Encoding.UTF8, true);
            return playgame == "开始游戏"//zhCN
                || playgame == "開始遊戲"//zhTW
                || playgame == "Start Game"//enUS
                || playgame == "Начать игру"//ruRU
                || playgame == "게임 시작"//koKR
                || playgame == "SPIEL STARTEN"//deDE
                || playgame == "Commencer"//frFR
                || playgame == "Avvia partita"//itIT
                || playgame == "Iniciar partida"//esES
                || playgame == "Iniciar partida"//esMX
                || playgame == "Rozpocznij grę"//plPL
                || playgame == "Iniciar Jogo"//ptPT
                ;
        }
        public override void Load(IController hud)
        {
            base.Load(hud);
            ToggleKeyEvent = Hud.Input.CreateKeyEvent(true, Key.F3, false, false, false);
            HeaderFont = Hud.Render.CreateFont("tahoma", 10, 255, 200, 200, 100, true, false, 255, 0, 0, 0, true);
            InfoFont = Hud.Render.CreateFont("tahoma", 8, 255, 200, 200, 0, true, false, 255, 0, 0, 0, true);
            uiGRmainPage = Hud.Render.RegisterUiElement("Root.NormalLayer.rift_dialog_mainPage", null, null);
            uiOnGreaterRift = Hud.Render.RegisterUiElement("Root.NormalLayer.rift_dialog_mainPage.LayoutRoot.RiftRadioButtons.GreaterRiftButton", null, null);
            uiAcceptButton = Hud.Render.RegisterUiElement("Root.NormalLayer.rift_dialog_mainPage.LayoutRoot.accept_Button", null, null);
            uiLeaveGame = Hud.Render.RegisterUiElement("Root.NormalLayer.gamemenu_dialog.gamemenu_bkgrnd.ButtonStackContainer.button_leaveGame", null, null);
            uiPlayGameButton = Hud.Render.RegisterUiElement("Root.NormalLayer.BattleNetCampaign_main.LayoutRoot.Menu.PlayGameButton", null, null);
            uiChangeQuestButton = Hud.Render.RegisterUiElement("Root.NormalLayer.BattleNetCampaign_main.LayoutRoot.Menu.ChangeQuestButton", null, null);
            uiOkButton = Hud.Render.RegisterUiElement("Root.TopLayer.BattleNetModalNotifications_main.ModalNotification.Buttons.ButtonList.OkButton", null, null);
            uiJoinPaty = Hud.Render.RegisterUiElement("Root.NormalLayer.rift_join_party_main.LayoutRoot.Background", null, null);
            uiJoinPatyAcceptButton = Hud.Render.RegisterUiElement("Root.NormalLayer.rift_join_party_main.LayoutRoot.Background.buttons.accept", null, null);
            uiResumeGame = Hud.Render.RegisterUiElement("Root.NormalLayer.gamemenu_dialog.gamemenu_bkgrnd.button_resumeGame", null, null);
            CloseGR = Hud.Render.RegisterUiElement("Root.NormalLayer.interact_dialog_mainPage.interact_dialog_Background.stack.interact_button_2", null, null);
            uiMapname = Hud.Render.RegisterUiElement("Root.NormalLayer.minimap_dialog_backgroundScreen.minimap_dialog_pve.area_name", null, null);
            Timer = hud.Time.CreateWatch();
            Timer2 = hud.Time.CreateWatch();
            Timer_clickclose = hud.Time.CreateWatch();
            Timer_clickclose2 = hud.Time.CreateWatch();
            MapList.Add(new Map(1, "Halls of Agony", "苦痛刑牢", "苦痛大厅", "Залы Агоний", "a1dun_leor"));
            MapList.Add(new Map(2, "Desolate Sands", "淒涼沙地", "凄凉沙漠", "Бескрайние пески", "caout_boneyard_"));
            MapList.Add(new Map(3, "Flooded Cave", "積水的洞穴", "积水的洞穴", "Затопленная пещера", "a2dun_cave_flooded"));
            MapList.Add(new Map(4, "Cave of the Betrayer", "背叛者的洞穴", "背叛者的洞穴", "Пещера предателя", "a2dun_cave"));
            MapList.Add(new Map(5, "Tidal Cave", "湧潮洞穴", "涌潮洞穴", "Затопленная пещера", "px_cave_a"));
            MapList.Add(new Map(6, "Cave", "洞穴", "洞穴", "Пещера", "trdun_cave"));     //Act 1
            MapList.Add(new Map(7, "Winding Cave", "曲折洞穴", "曲折洞穴", "Извилистый грот", "x1_bogcave"));
            MapList.Add(new Map(8, "Caverns of Araneae", "艾瑞妮洞窟（蜘蛛洞）", "蛛后洞窟（蜘蛛洞）", "Пещеры Араны", "a2dun_spider"));
            MapList.Add(new Map(9, "Fields of Misery", "悲慘之原", "苦难旷野", "Гиблые поля", "px_tristramfields_"));
            MapList.Add(new Map(10, "Vault of the Assassin", "刺客地庫（沒有光幕）", "刺客密室（没有光幕）", "Гробница наемника", "a2dun_zolt_random"));
            MapList.Add(new Map(11, "Archives of Zultun Kulle", "佐敦庫勒秘庫（有光幕）", "佐敦库勒密库（有光幕）", "Архивы Золтуна Кулла", "a2dun_zolt"));
            MapList.Add(new Map(12, "Hell Rift", "地獄之門（小地核）", "地狱裂隙（小地核）", "Демонический Разлом", "a4dun_hellportal"));
            MapList.Add(new Map(13, "Hell Rift", "地獄之門（小地核）", "地狱裂隙（小地核）", "Демонический Разлом", "a3dun_crater_e_dead_end"));
            MapList.Add(new Map(14, "Hell Rift", "地獄之門（小地核）", "地狱裂隙（小地核）", "Демонический Разлом", "a3dun_crater_s_dead_end"));
            MapList.Add(new Map(15, "Arreat Crater", "亞瑞特巨坑（大地核）", "亚瑞特巨坑（大地核）", "Ареатский Кратер", "a3dun_crater"));
            MapList.Add(new Map(16, "Icefall Cave", "落冰洞穴", "寒冰洞穴", "Ледяная Пещера", "a3dun_icecaves"));
            MapList.Add(new Map(17, "The Keep Depths", "要塞（兵營）", "要塞（兵营）", "Нижние Этажи Крепости", "a3dun_keep"));
            MapList.Add(new Map(18, "The Silver Spire", "銀光尖塔", "银色高塔（银光尖塔）", "Серебрянный Шпиль", "a4dun_spire_corrupt"));
            MapList.Add(new Map(19, "Greyhollow Island", "灰荒島", "灰洞岛", "Серый Остров", "p4_forest_coast_border"));
            MapList.Add(new Map(20, "Eternal Woods", "永恆之林", "永恒森林", "Вечный Лес", "p4_forest_snow_border"));
            MapList.Add(new Map(21, "Temple of the Firstborn", "初民神殿", "先民神殿", "Храм Перворождённых", "p6_church"));
            MapList.Add(new Map(22, "Shrouded Moors", "迷霧荒原", "迷雾荒原", "Свечка", "p6_moor"));
            MapList.Add(new Map(23, "Desert", "沙漠", "沙漠", "Пустыня", "px_desert_120_border"));
            MapList.Add(new Map(24, "The Festering Woods", "腐潰之林", "烂木林", "Гниющий Лес", "px_festeringwoods"));
            MapList.Add(new Map(25, "Cathedral", "大教堂", "大教堂", "Собор", "trdun_cath"));
            MapList.Add(new Map(26, "Crypt", "墓穴", "墓穴", "Склеп", "trdun_crypt"));
            MapList.Add(new Map(27, "Plague Tunnels", "瘟疫地道（老鼠洞）", "瘟疫地道（老鼠洞）", "Чумные Катакомбы", "x1_abattoir"));
            MapList.Add(new Map(28, "Ruins of Corvus", "寇佛斯遺跡（愛德莉亞）", "科乌斯废墟（A5版兵营）", "Руины Корвуса", "x1_catacombs"));
            MapList.Add(new Map(29, "Pandemonium Fortress", "混沌界要塞", "混沌要塞", "Крепость Пандемония", "x1_fortress"));
            MapList.Add(new Map(30, "Battlefields of Eternity", "永恆戰場", "永恒战场", "Поля Вечного Боя", "x1_pand_ext_120_edge"));
            MapList.Add(new Map(31, "Realm of the Banished", "遺逐之境（懸崖）", "放逐之境（悬崖）", "Мир Изгнанников", "x1_pand_hexmaze"));
            MapList.Add(new Map(32, "Briarthorn Cemetery", "荊棘墓園", "棘草墓园", "Кладбище Бриарторн", "x1_westm_graveyard_"));
            MapList.Add(new Map(33, "Westmarch Heights", "衛斯馬屈山城區", "威斯特玛上城区", "Верхний Вестмарш", "x1_westm", "fire"));
            MapList.Add(new Map(34, "Westmarch Commons", "衛斯馬屈城中區", "威斯特玛城中区", "Вестмарш Торговый Квартал", "x1_westm"));
            
        }
        public void AddToSelectedById(params uint[] ids)
        {
            foreach (var id in ids)
            {
                SelectedMapList.Add(MapList.Find(x => x.ID == id));
            }
        }
        public void OnNewArea(bool newGame, ISnoArea area)
        {
            if (newGame)
            {
                StopAll();
            }
        }
        public void OnKeyEvent(IKeyEvent keyEvent)
        {
            if (ToggleKeyEvent.Matches(keyEvent) && keyEvent.IsPressed && Hud.Interaction.IsHotKeySet(ActionKey.Move) && SelectedMapList.Count != 0)
            {
                if (Running == true)
                    Running = false;
                if (uiGRmainPage.Visible)
                {
                    if (Running == false)
                        isCaptain = true;
                        Running = true;
                }
                if (uiJoinPaty.Visible)
                {
                    if (Running == false)
                        isCaptain = false;
                    Running = true;
                }
                if (!Running)
                {
                    isCaptain = false;
                    StopAll();
                }
            }
        }
        private void StopAll()
        {
            isEnteredGR = false;
            isFoundObelisk = false;
            isEntering = false;
            isEnteredGR = false;
            Timer.Stop();
            Timer2.Stop();
            Timer_clickclose.Stop();
            Timer_clickclose2.Stop();
        }
        public void AfterCollect()
        {
            if (!Running) return;
            if (Hud.Game.IsLoading || !Hud.Window.IsForeground) return;
            if (Timer.IsRunning && Timer.ElapsedMilliseconds < 100) return;
            if (Hud.Game.Me.Materials.GreaterRiftKeystone <= 0) return;
            if (Timer2.IsRunning && Timer2.ElapsedMilliseconds < (isMultiplayergame ? 3000 : 200)) return;
            if (OrekName == "" && Hud.Game.Me.IsInTown)
            {
                var _Orek = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_lr_nephalem).FirstOrDefault();
                if (_Orek != null)
                {
                    OrekName = _Orek.SnoActor.NameLocalized;
                }


            }
            if (uiOkButton.Visible)
            {
                Hud.Interaction.PressOkOnGenericModalDialog();//点击确定，可能是操作过快导致断开连接的提示，以防万一
            }

            if(!Hud.Game.IsInGame && !Timer2.IsRunning && uiPlayGameButton.Visible && uiPlayGameButton.AnimState == 58 && uiChangeQuestButton.AnimState == 16 && PlaygameButton())
            {
                Timer2.Restart();
            }
            if (Timer2.IsRunning && Timer2.ElapsedMilliseconds >= (isMultiplayergame ? 3000 : 200) && uiPlayGameButton.Visible && uiPlayGameButton.AnimState == 58 && uiChangeQuestButton.AnimState == 16 && PlaygameButton())
            {
                if(uiResumeGame.Visible)
                {
                    Hud.Interaction.ClickUiElement(MouseButtons.Left, uiResumeGame); ;//返回游戏关闭菜单
                }
                else
                {
                    Hud.Interaction.NewGame();
                }
                Timer.Restart();
            }
            if(isEnteredGR && Hud.Game.Me.IsInTown)
            {

                Timer2.Stop();
                //重置大秘境
                if(Hud.Game.NumberOfPlayersInGame == 1)//单人模式对话欧瑞克关闭
                {
                    var Orek = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_lr_nephalem).FirstOrDefault();
                    if(Orek?.IsOnScreen == true)
                    {
                        bool isGR = glq.PublicClassPlugin.IsGreaterRift(Hud);
                        var OpenPortal = Hud.Game.Actors.Where(x => x.SnoActor.Sno == ActorSnoEnum._x1_openworld_tiered_rifts_portal).FirstOrDefault();
                        if (OpenPortal != null)//大秘境还没关门
                        {
                            if(isGR)
                            {
                                if (CloseGR?.Visible == true)
                                {
                                    Timer_clickclose2.Stop();
                                    if (!Timer_clickclose.IsRunning)
                                    {
                                        Timer_clickclose.Restart();
                                    }
                                    if(Timer_clickclose.ElapsedMilliseconds >= 100)
                                    {
                                        Hud.Interaction.MoveMouseOverUiElement(CloseGR);
                                        if (Timer_clickclose.ElapsedMilliseconds >= 200)
                                        {
                                            Hud.Interaction.ClickUiElement(MouseButtons.Left, CloseGR);
                                        }
                                    }
                                }
                                else
                                {
                                    if(Timer_clickclose.IsRunning && Timer_clickclose.ElapsedMilliseconds >= 500)
                                    {
                                        Timer_clickclose.Stop();
                                    }
                                    if(!Timer_clickclose.IsRunning && (!Timer_clickclose2.IsRunning || Timer_clickclose2.ElapsedMilliseconds >= 500))
                                    {
                                        Hud.Interaction.TalkTownActor(Orek);//与欧瑞克对话
                                        Timer_clickclose2.Restart();
                                    }
                                    
                                }
                            }
                        }
                        else if(!isGR)
                        {
                            StopAll();
                        } 
                    }
                }
                else//组队模式小退游戏关闭大秘境
                {
                    if (!uiLeaveGame.Visible)
                    {
                        Hud.Interaction.PressEsc();
                    }
                    else
                    {
                        Hud.Interaction.ClickUiElement(MouseButtons.Left, uiLeaveGame);//离开游戏
                    }
                    Timer.Restart();
                }
            }
            if(Hud.Game.IsInGame)
            {
                if(isEntering == false)
                {
                    if (!isEnteredGR)//未进入过大秘境
                    {
                        if(isCaptain)
                        {
                            if (!uiGRmainPage.Visible && Hud.Game.Me.IsInTown && Hud.Game.NumberOfPlayersInGame == Hud.Game.Players.Count())//等待所有人加入游戏
                            {
                                int CurrentAct = Hud.Game.CurrentAct;
                                if (!isFoundObelisk && (CurrentAct == 3 || CurrentAct == 4 || CurrentAct == 5))
                                {
                                    Hud.Interaction.MouseMove(20, Hud.Window.Size.Height - 20);
                                    Hud.Interaction.DoAction(ActionKey.Move, false, 50, 50, 50);//纠正位置
                                }
                                foreach (var actor in Hud.Game.Actors)
                                {
                                    if (actor.SnoActor.Sno == ActorSnoEnum._x1_openworld_lootrunobelisk_b)
                                    {
                                        isFoundObelisk = true;
                                        if (actor.NormalizedXyDistanceToMe > 20)
                                        {
                                            Hud.Interaction.MouseMove(actor.ScreenCoordinate.X, actor.ScreenCoordinate.Y);
                                            Hud.Interaction.DoAction(ActionKey.Move, false);//向方尖碑移动
                                        }
                                        else if (actor.NormalizedXyDistanceToMe > 0)
                                        {
                                            Hud.Interaction.TalkTownActor(actor);//打开方尖碑
                                        }
                                        break;
                                    }
                                }
                            }
                            else if (uiOnGreaterRift.AnimState == 3)
                            {
                                Hud.Interaction.ClickUiElement(MouseButtons.Left, uiOnGreaterRift);
                            }
                            if (uiOnGreaterRift.AnimState == 5)
                            {
                                Hud.Interaction.ClickUiElement(MouseButtons.Left, uiAcceptButton);//进入大秘境
                                isEntering = true;
                            }
                        }
                        if (uiJoinPatyAcceptButton.Visible && !Hud.Render.IsAnyBlockingUiElementVisible)
                        {
                            Hud.Interaction.ClickUiElement(MouseButtons.Left, uiJoinPatyAcceptButton);//接受进入大秘境请求
                        }
                        Timer.Restart();
                    }
                }
                if (!Hud.Game.Me.IsInTown)
                {
                    isEntering = false;
                    isEnteredGR = true;
                    //判断当前地形
                    var Scene = Hud.Game.Me.Scene;
                    if (Scene == null) return;//容错null，否则偶尔会出错
                    var curScene = Scene.SnoScene.Code;
                    if (curScene == null) return;//容错null，否则偶尔会出错
                    var curMapName = "";
                    var curMapLocname = uiMapname.ReadText(Encoding.UTF8, true);
                    foreach (var map in SelectedMapList)
                        if (map.Match(curScene) || curMapLocname.Contains(OrekName))
                        {
                            curMapName = map.Name(Hud.CurrentLanguage);
                            Running = false;
                            StopAll();
                            if(curMapLocname.Contains(OrekName))
                            {
                                Hud.Sound.Speak(curMapLocname);
                            }
                            else
                            {
                                Hud.Sound.Speak(curMapName);
                            }
                            if (Hud.Game.NumberOfPlayersInGame == 1)
                            {

                                for (int i = 0; i <= 50;)
                                {
                                    if (!Hud.Game.IsPaused)
                                    {
                                        Hud.Interaction.PressEsc();
                                        Hud.ReCollect();
                                        Hud.Wait(100);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                
                            }
                            break;
                        }
                }
                if (isEnteredGR && Running)//进入过大秘境
                {
                    if (!Hud.Game.Me.IsInTown)
                    {
                        //回到城镇
                        foreach (var port in Hud.Game.Portals)
                        {
                            if (port.TargetArea != null && port.TargetArea.IsTown && port.IsOnScreen)
                            {
                                float x = port.ScreenCoordinate.X;
                                float y = port.ScreenCoordinate.Y;
                                if (x <= 0)
                                {
                                    x = 3;
                                }
                                if (y <= 0)
                                {
                                    y = 3;
                                }
                                if (x >= Hud.Window.Size.Width)
                                {
                                    x = Hud.Window.Size.Width - 3;
                                }
                                if (y >= Hud.Window.Size.Height)
                                {
                                    y = Hud.Window.Size.Height - 3;
                                }
                                Hud.Interaction.MouseMove(x, y);
                                Hud.Interaction.DoAction(ActionKey.LeftSkill);
                                Timer.Restart();
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void PaintTopInGame(ClipState clipState)
        {
            isMultiplayergame = Hud.Game.NumberOfPlayersInGame > 1;//判断是否组队
            string MapName = "";
            foreach (var map in SelectedMapList)
            {
                MapName += map.Name(Hud.CurrentLanguage) + "、";
            }
            if (Hud.CurrentLanguage == Language.zhCN)
            {
                str_Header = "【雷电宏-自动撕票】";
                str_Info = MapName == "" ? "未选择地图类型，无法开始\r\n请在“雷电宏相关中”设置自动撕票地图类型" : "先选择大秘境层数\r\n单击 " + ToggleKeyEvent.ToString() + " 开始自动撕票\r\n" + MapName + "时自动停止";
                str_Info2 = "单击 " + ToggleKeyEvent.ToString() + " 开始自动撕票\r\n" + MapName + "时自动停止";
                str_Running = "自动撕票中...\r\n按住 " + ToggleKeyEvent.ToString() + " 停止";
                str_ForceMove = "请先在“游戏选项”“按键绑定”中设置“强制移动”热键";
            }
            else if (Hud.CurrentLanguage == Language.zhTW)
            {
                str_Header = "【雷電宏-自動撕票】";
                str_Info = MapName == "" ? "未選擇地圖類型，無法開始\r\n請在“雷電宏相關中”設置自動撕票地圖類型" : "先選擇大秘境層數\r\n單擊 " + ToggleKeyEvent.ToString() + " 開始自動撕票\r\n" + MapName + "時自動停止";
                str_Info2 = "單擊 " + ToggleKeyEvent.ToString() + " 開始自動撕票\r\n" + MapName + "時自動停止";
                str_Running = "自動撕票中...\r\n按住 " + ToggleKeyEvent.ToString() + " 停止";
                str_ForceMove = "請先在“設定”“按鍵設定”中設置“強制移動”熱鍵";
            }
            else if (Hud.CurrentLanguage == Language.ruRU)
            {
                str_Header = "【МОД-Поиск ВП】";
                str_Info = MapName == "" ? "Для АвтоПоиска ВП необходимо выбрать карты на закладке Макросы" : "Выбрать уровень ВП.\r\nНажать " + ToggleKeyEvent.ToString() + " запустив авто поиск.\r\nПоиск остановится на одной из следующей карт:\r\n" + MapName;
                str_Info2 = "Нажать " + ToggleKeyEvent.ToString() + " для запуска Поиска ВП\r\nСтоп на карте " + MapName;
                str_Running = "АвтоПоиск ВП...\r\nНажать " + ToggleKeyEvent.ToString() + " для остановки";
                str_ForceMove = "Назначте клавишу для < ТОЛЬКО ПЕРЕДВИЖЕНИЕ > в <НАСТРОЙКИ><ГОРЯЧИИ КЛАВИШИ>";
            }
            else
            {
                str_Header = "【OpenGreatRift-Mod】";
                str_Info = MapName == "" ? "You need to select the map type of AutoOpenGreatRift in Macros first" : "Select GR level first\r\nPress " + ToggleKeyEvent.ToString() + " to start OpenGR\r\nStop when " + MapName;
                str_Info2 = "Press " + ToggleKeyEvent.ToString() + " to start OpenGR\r\nStop when " + MapName;
                str_Running = "Auto searching GR...\r\nPress " + ToggleKeyEvent.ToString() + " to stop";
                str_ForceMove = "Plese set < FORCE MOVE > key in < OPTIONS >";

            }
            var layout = HeaderFont.GetTextLayout(str_Header);
            var y = uiGRmainPage.Rectangle.Y + uiGRmainPage.Rectangle.Height * 0.02f;
            if (uiGRmainPage.Visible)
            {
                HeaderFont.DrawText(layout, uiGRmainPage.Rectangle.X + (uiGRmainPage.Rectangle.Width * 0.44f - layout.Metrics.Width) / 2, y);
                y += layout.Metrics.Height * 1.3f;
            }
            
            if (Running)
            {
                layout = InfoFont.GetTextLayout(str_Running);
                InfoFont.DrawText(layout, Hud.Window.Size.Width / 2 - layout.Metrics.Width / 2, Hud.Window.Size.Height / 2 - layout.Metrics.Height / 2);
                if (uiGRmainPage.Visible)
                {
                    layout = InfoFont.GetTextLayout(str_Running);
                    InfoFont.DrawText(layout, uiGRmainPage.Rectangle.X + uiGRmainPage.Rectangle.Width * 0.1f, y);
                }
            }
            else
            {
                if (uiGRmainPage.Visible)
                {
                    if(Hud.Interaction.IsHotKeySet(ActionKey.Move))
                    {
                        layout = InfoFont.GetTextLayout(str_Info);
                    }
                    else
                    {
                        layout = InfoFont.GetTextLayout(str_ForceMove);
                    }
                    InfoFont.DrawText(layout, uiGRmainPage.Rectangle.X + uiGRmainPage.Rectangle.Width * 0.1f, y);
                }
                if (uiJoinPaty.Visible)
                {

                    layout = HeaderFont.GetTextLayout(str_Header);
                    HeaderFont.DrawText(layout, uiJoinPaty.Rectangle.X, uiJoinPaty.Rectangle.Y - layout.Metrics.Height);

                    if (Hud.Interaction.IsHotKeySet(ActionKey.Move))
                    {
                        layout = InfoFont.GetTextLayout(str_Info2);
                    }
                    else
                    {
                        layout = InfoFont.GetTextLayout(str_ForceMove);
                    }
                    InfoFont.DrawText(layout, uiJoinPaty.Rectangle.X, uiJoinPaty.Rectangle.Y);
                }
            }
        }
        public class Map
        {
            public uint ID { get; set; }
            public string NameEnglish { get; set; }
            public string NameTW { get; set; }
            public string NameCN { get; set; }
            public string NameRU { get; set; }
            public string MapAffix { get; set; }
            public string MapAffix2 { get; set; }

            public Map(uint id, string nameEnglish, string nameTW, string nameCN, string nameRU, string start, string end = "")
            {
                ID = id;
                NameEnglish = nameEnglish;
                NameTW = nameTW;
                NameCN = nameCN;
                NameRU = nameRU;
                MapAffix = start;
                MapAffix2 = end;
            }
            public string Name(Language Language)
            {
                var name = "";
                if (Language == Language.zhCN)
                {
                    name += NameCN;
                }
                else if (Language == Language.zhTW)
                {
                    name += NameTW;
                }
                else if (Language == Language.ruRU)
                {
                    name += NameRU;
                }
                else
                {
                    name += NameEnglish;
                }
                return name;
            }
            public bool Match(string sceneName)
            {
                if (!sceneName.StartsWith(MapAffix)) return false;

                if (MapAffix2 == "" || sceneName.EndsWith(MapAffix2))
                    return true;
                return false;
            }
        }
    }
}