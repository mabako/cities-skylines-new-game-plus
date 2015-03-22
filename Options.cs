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

            UIPanel ngpPanel = newGamePanel.component.AddUIComponent<UIPanel>();
            UIComponent newgameCaption = newGamePanel.component.Find("Caption");
            UIComponent closeButton = newGamePanel.component.Find("Close");

			// uncomment if you want it visible
//            ngpPanel.backgroundSprite = "";
//            ngpPanel.color = new Color32(255,255,255,255);

            ngpPanel.width = newGamePanel.component.width;
            ngpPanel.height = newgameCaption.height * 0.9f;

            ngpPanel.autoLayout = true;
            ngpPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            ngpPanel.autoLayoutStart = LayoutStart.TopLeft;
            ngpPanel.autoLayoutPadding = new RectOffset(4,4,2,2);

            if (Base.Config.StartMoney < 0 || Base.Config.StartMoney > maxMoney)
                Base.Config.StartMoney = 70000;

            CreateButton("DecreaseMoney", ngpPanel, "-", DecreaseMoney);
            label = CreateLabel("StartMoney", ngpPanel, FormatMoney(Base.Config.StartMoney));
            label.width = newgameCaption.width/6;
            CreateButton("IncreaseMoney", ngpPanel, "+", IncreaseMoney);

            CreateCheckbox(SPRITE_HIGHWAY, ngpPanel, Base.Config.AllRoads, (e) => Base.Config.AllRoads = e, "all roads enabled from the start");
            CreateCheckbox(SPRITE_AREA, ngpPanel, Base.Config.AllAreas, (e) => Base.Config.AllAreas = e, "all areas purchaseable from the start");

            CreateCheckbox(SPRITE_BUS,   ngpPanel, Base.Config.Buses, (e) => Base.Config.Buses = e, "busses enabled from the start");
            CreateCheckbox(SPRITE_METRO, ngpPanel, Base.Config.Subways, (e) => Base.Config.Subways = e, "metros enabled from the start");
            CreateCheckbox(SPRITE_TRAIN, ngpPanel, Base.Config.Trains, (e) => Base.Config.Trains = e, "trains enabled from the start");
            CreateCheckbox(SPRITE_SHIP,  ngpPanel, Base.Config.Ships, (e) => Base.Config.Ships = e, "ships enabled from the start");
            CreateCheckbox(SPRITE_PLANE, ngpPanel, Base.Config.Airplanes, (e) => Base.Config.Airplanes = e, "airports enabled from the start");

            ngpPanel.pivot = UIPivotPoint.TopRight;
            ngpPanel.transformPosition = new Vector3(closeButton.GetBounds().max.x, closeButton.GetBounds().min.y,0);
            ngpPanel.relativePosition += new Vector3(0, 3, 0);

			// I don't know why you can resize the panel after it has been placed, but it works and resizing it seems to be necessary
            ngpPanel.width = getChildrenWidth(ngpPanel);
        }

        float getChildrenWidth (UIPanel ngpPanel)
        {
            float width = 0;
            UIComponent[] children = ngpPanel.GetComponentsInChildren<UIComponent>();
            foreach (UIComponent child in children) {
                if(child == ngpPanel) continue;

                width += child.width + ngpPanel.autoLayoutPadding.left + ngpPanel.autoLayoutPadding.right;
            }
            return width;
        }

        private UILabel CreateLabel(string type, UIPanel panel, string text)
        {

            UILabel label = panel.AddUIComponent<UILabel>();

            label.autoSize = true;
            label.text = text;
            label.textScale = 1.1f;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.textColor = new Color32(255, 255, 255, 255);
            label.absolutePosition = new Vector3(0,50,0);

			// same height as parent = middle alignment works
            label.autoSize = false;
            label.height = panel.height;

            return label;
        }

        private void CreateButton(string type, UIPanel panel, string text, Action<object> onClick)
        {
            UIButton button = panel.AddUIComponent<UIButton>();

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

            button.eventClick += (UIComponent component, UIMouseEventParameter eventParam) =>
            {
                if (component != button || eventParam.buttons != UIMouseButton.Left)
                    return;

                onClick(null);
            };
        }

        private void CreateCheckbox(string type, UIPanel panel, bool isEnabled, Action<bool> setEnabled, string tooltip)
        {
            UIButton button = panel.AddUIComponent<UIButton>();

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
