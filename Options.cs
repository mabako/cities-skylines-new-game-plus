using ColossalFramework.UI;
using System;
using System.Collections.Generic;
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

        private UILabel label;
        private const int labelWidth = 140;
        private const long maxMoney = 10000000;

        private IList<UIComponent> components = new List<UIComponent>();

        public Options(NewGamePanel newGamePanel)
        {
            this.newGamePanel = newGamePanel;

            int x = 900;
            int y = 240;
            int spacing = 35;

            if (Base.Config.StartMoney < 0 || Base.Config.StartMoney > maxMoney)
                Base.Config.StartMoney = 70000;

            CreateButton("DecreaseMoney", new Vector2(x += spacing, y), "-", DecreaseMoney);
            label = CreateLabel("StartMoney", new Vector2(x += spacing, y + 3), FormatMoney(Base.Config.StartMoney));
            CreateButton("IncreaseMoney", new Vector2(x += labelWidth, y), "+", IncreaseMoney);

            CreateCheckbox(SPRITE_HIGHWAY, new Vector2(x += spacing, y), Base.Config.AllRoads, (e) => Base.Config.AllRoads = e, "all roads enabled from the start");
            CreateCheckbox(SPRITE_AREA, new Vector2(x += spacing, y), Base.Config.AllAreas, (e) => Base.Config.AllAreas = e, "all areas purchaseable from the start");

            x += 10;

            CreateCheckbox(SPRITE_BUS, new Vector2(x += spacing, y), Base.Config.Buses, (e) => Base.Config.Buses = e, "busses enabled from the start");
            CreateCheckbox(SPRITE_METRO, new Vector2(x += spacing, y), Base.Config.Subways, (e) => Base.Config.Subways = e, "metros enabled from the start");
            CreateCheckbox(SPRITE_TRAIN, new Vector2(x += spacing, y), Base.Config.Trains, (e) => Base.Config.Trains = e, "trains enabled from the start");
            CreateCheckbox(SPRITE_SHIP, new Vector2(x += spacing, y), Base.Config.Ships, (e) => Base.Config.Ships = e, "ships enabled from the start");
            CreateCheckbox(SPRITE_PLANE, new Vector2(x += spacing, y), Base.Config.Airplanes, (e) => Base.Config.Airplanes = e, "airports enabled from the start");
        }

        private UILabel CreateLabel(string type, Vector2 location, string text)
        {

            var lblObject = new GameObject("NewGamePlusUI/" + type, typeof(UILabel));
            lblObject.transform.parent = newGamePanel.transform;
            var label = lblObject.GetComponent<UILabel>();

            label.absolutePosition = location;
            label.width = labelWidth;
            label.height = 30;
            label.text = text;
            label.textScale = 1.1f;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.textColor = new Color32(255, 255, 255, 255);
            label.Invalidate();

            components.Add(label);

            return label;
        }

        private void CreateButton(string type, Vector2 location, string text, Action<object> onClick)
        {
            var btnObject = new GameObject("NewGamePlusUI/" + type, typeof(UIButton));
            btnObject.transform.parent = newGamePanel.transform;
            var button = btnObject.GetComponent<UIButton>();

            button.absolutePosition = location;
            button.width = 30;
            button.height = 30;
            button.text = text;
            button.textScale = 1.2f;
            button.textColor = new Color32(255, 255, 255, 255);

            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.pressedBgSprite = "ButtonMenuPressed";

            components.Add(button);

            button.eventClick += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                if (component != button || eventParam.buttons != UIMouseButton.Left)
                    return;

                onClick(null);
            };
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

            components.Add(button);

            // Doesn't play nicely with enabling/disabling; just changing the sprites produces inconsistent results though.
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

        private string FormatMoney(long value)
        {
            UICurrencyWrapper uicw = new UICurrencyWrapper(long.MaxValue);
            uicw.Check(value * 100, Settings.moneyFormatNoCents);
            return uicw.result;
        }

        private void DecreaseMoney(object _)
        {
            long currentMoney = Base.Config.StartMoney;
            if (currentMoney <= 0)
                currentMoney = 0;
            else if (currentMoney <= 200000)
                currentMoney -= 10000;
            else if (currentMoney <= 500000)
                currentMoney -= 50000;
            else if (currentMoney <= 1000000)
                currentMoney -= 100000;
            else
                currentMoney -= 500000;

            Base.Config.StartMoney = currentMoney;
            Configuration.Serialize(Base.Config);
            label.text = FormatMoney(currentMoney);
            label.Invalidate();
        }
        private void IncreaseMoney(object _)
        {
            long currentMoney = Base.Config.StartMoney;
            if (currentMoney >= maxMoney)
                currentMoney = maxMoney;
            else if (currentMoney >= 1000000)
                currentMoney += 500000;
            else if (currentMoney >= 500000)
                currentMoney += 100000;
            else if (currentMoney >= 200000)
                currentMoney += 50000;
            else
                currentMoney += 10000;

            Base.Config.StartMoney = currentMoney;
            Configuration.Serialize(Base.Config);
            label.text = FormatMoney(currentMoney);
            label.Invalidate();
        }

        internal void Show(bool visible)
        {
            foreach(var component in components)
            {
                component.isVisible = visible;
            }
        }
    }
}
