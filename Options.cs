using ColossalFramework.UI;
using System;
using UnityEngine;
namespace NewGamePlus
{
    class Options
    {
        private NewGamePanel newGamePanel;

        private const string SPRITE_HIGHWAY = "SubBarRoadsLarge";
        private const string SPRITE_AREA = "ToolbarIconZoning";
        private const string SPRITE_BUS = "SubBarPublicTransportBus";
        private const string SPRITE_METRO = "SubBarPublicTransportMetro";
        private const string SPRITE_TRAIN = "SubBarPublicTransportTrain";
        private const string SPRITE_SHIP = "SubBarPublicTransportShip";
        private const string SPRITE_PLANE = "SubBarPublicTransportPlane";

        public Options(NewGamePanel newGamePanel)
        {
            this.newGamePanel = newGamePanel;

            int x = 1100;
            int spacing = 35;
            CreateCheckbox(SPRITE_HIGHWAY, new Vector2(x += spacing, 240), Base.Config.AllRoads, (e) => Base.Config.AllRoads = e, "all roads enabled from the start");
            CreateCheckbox(SPRITE_AREA, new Vector2(x += spacing, 240), Base.Config.AllAreas, (e) => Base.Config.AllAreas = e, "all areas purchaseable from the start");

            x += 10;

            CreateCheckbox(SPRITE_BUS, new Vector2(x += spacing, 240), Base.Config.Buses, (e) => Base.Config.Buses = e, "busses enabled from the start");
            CreateCheckbox(SPRITE_METRO, new Vector2(x += spacing, 240), Base.Config.Subways, (e) => Base.Config.Subways = e, "metros enabled from the start");
            CreateCheckbox(SPRITE_TRAIN, new Vector2(x += spacing, 240), Base.Config.Trains, (e) => Base.Config.Trains = e, "trains enabled from the start");
            CreateCheckbox(SPRITE_SHIP, new Vector2(x += spacing, 240), Base.Config.Ships, (e) => Base.Config.Ships = e, "ships enabled from the start");
            CreateCheckbox(SPRITE_PLANE, new Vector2(x += spacing, 240), Base.Config.Airplanes, (e) => Base.Config.Airplanes = e, "airports enabled from the start");
        }

        private void CreateCheckbox(string type, Vector2 location, bool isEnabled, Action<bool> setEnabled, string tooltip)
        {
            var btnObject = new GameObject("NewGamePlusUI/" + type, typeof(UIButton));
            btnObject.transform.parent = newGamePanel.transform;
            var button = btnObject.GetComponent<UIButton>();

            button.absolutePosition = location;
            button.width = 30;
            button.height = 30;
            button.textColor = new Color32(255, 255, 255, 255);
            button.tooltip = tooltip;

            button.normalBgSprite = type;
            button.disabledBgSprite = type + "Disabled";
            button.hoveredBgSprite = type + "Hovered";
            button.focusedBgSprite = type;
            button.pressedBgSprite = type + "Pressed";

            if (!isEnabled)
                button.Disable();

            button.playAudioEvents = false;

            button.eventClick += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                if (component != button || eventParam.buttons != UIMouseButton.Left)
                    return;

                button.Disable();

                setEnabled(false);
                Configuration.Serialize(Base.Config);
            };

            button.eventDisabledClick += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                if (component != button || eventParam.buttons != UIMouseButton.Left)
                    return;

                button.Enable();
                
                setEnabled(true);
                Configuration.Serialize(Base.Config);
            };
        }
    }
}
