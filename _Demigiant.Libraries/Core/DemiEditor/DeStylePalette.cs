﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/29 18:54

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiEditor
{
    /// <summary>
    /// Stores a GUIStyle palette, which can be passed to default DeGUI layouts when calling <code>DeGUI.BeginGUI</code>,
    /// and changed at any time by calling <code>DeGUI.ChangePalette</code>.
    /// You can inherit from this class to create custom GUIStyle palettes with more options.
    /// Each of the sub-options require a public Init method to initialize the styles, which will be called via Reflection.
    /// </summary>
    public class DeStylePalette
    {
        public readonly Box box = new Box();
        public readonly Button button = new Button();
        public readonly Label label = new Label();
        public readonly Toolbar toolbar = new Toolbar();
        public readonly Misc misc = new Misc();

        protected bool initialized;

        // ADB path to Imgs directory, final slash included
        static string _adbImgsDir {
            get { if (_fooAdbImgsDir == null) _fooAdbImgsDir = Assembly.GetExecutingAssembly().ADBDir() + "/Imgs/"; return _fooAdbImgsDir; }
        }
        static string _fooAdbImgsDir;

        #region Texture2D

        public static Texture2D whiteSquare { get {
                if (_fooWhiteSquare == null) _fooWhiteSquare = LoadSquareTexture("whiteSquare");
                return _fooWhiteSquare;
        }}
        public static Texture2D whiteSquareAlpha10 { get {
                if (_fooWhiteSquareAlpha10 == null) _fooWhiteSquareAlpha10 = LoadSquareTexture("whiteSquareAlpha10");
                return _fooWhiteSquareAlpha10;
        }}
        public static Texture2D whiteSquareAlpha15 { get {
                if (_fooWhiteSquareAlpha15 == null) _fooWhiteSquareAlpha15 = LoadSquareTexture("whiteSquareAlpha15");
                return _fooWhiteSquareAlpha15;
        }}
        public static Texture2D whiteSquareAlpha25 { get {
                if (_fooWhiteSquareAlpha25 == null) _fooWhiteSquareAlpha25 = LoadSquareTexture("whiteSquareAlpha25");
                return _fooWhiteSquareAlpha25;
        }}
        public static Texture2D whiteSquareAlpha50 { get {
                if (_fooWhiteSquareAlpha50 == null) _fooWhiteSquareAlpha50 = LoadSquareTexture("whiteSquareAlpha50");
                return _fooWhiteSquareAlpha50;
        }}
        public static Texture2D blackSquare { get {
                if (_fooBlackSquare == null) _fooBlackSquare = LoadSquareTexture("blackSquare");
                return _fooBlackSquare;
        }}
        public static Texture2D blackSquareAlpha10 { get {
                if (_fooBlackSquareAlpha10 == null) _fooBlackSquareAlpha10 = LoadSquareTexture("blackSquareAlpha10");
                return _fooBlackSquareAlpha10;
        }}
        public static Texture2D blackSquareAlpha15 { get {
                if (_fooBlackSquareAlpha15 == null) _fooBlackSquareAlpha15 = LoadSquareTexture("blackSquareAlpha15");
                return _fooBlackSquareAlpha15;
        }}
        public static Texture2D blackSquareAlpha25 { get {
                if (_fooBlackSquareAlpha25 == null) _fooBlackSquareAlpha25 = LoadSquareTexture("blackSquareAlpha25");
                return _fooBlackSquareAlpha25;
        }}
        public static Texture2D blackSquareAlpha50 { get {
                if (_fooBlackSquareAlpha50 == null) _fooBlackSquareAlpha50 = LoadSquareTexture("blackSquareAlpha50");
                return _fooBlackSquareAlpha50;
        }}
        public static Texture2D redSquare { get {
                if (_fooRedSquare == null) _fooRedSquare = LoadSquareTexture("redSquare");
                return _fooRedSquare;
        }}
        public static Texture2D orangeSquare { get {
                if (_fooOrangeSquare == null) _fooOrangeSquare = LoadSquareTexture("orangeSquare");
                return _fooOrangeSquare;
        }}
        public static Texture2D yellowSquare { get {
                if (_fooYellowSquare == null) _fooYellowSquare = LoadSquareTexture("yellowSquare");
                return _fooYellowSquare;
        }}
        public static Texture2D greenSquare { get {
                if (_fooGreenSquare == null) _fooGreenSquare = LoadSquareTexture("greenSquare");
                return _fooGreenSquare;
        }}
        public static Texture2D blueSquare { get {
                if (_fooBlueSquare == null) _fooBlueSquare = LoadSquareTexture("blueSquare");
                return _fooBlueSquare;
        }}
        public static Texture2D purpleSquare { get {
                if (_fooPurpleSquare == null) _fooPurpleSquare = LoadSquareTexture("purpleSquare");
                return _fooPurpleSquare;
        }}
        static Texture2D _fooWhiteSquare;
        static Texture2D _fooWhiteSquareAlpha10;
        static Texture2D _fooWhiteSquareAlpha15;
        static Texture2D _fooWhiteSquareAlpha25;
        static Texture2D _fooWhiteSquareAlpha50;
        static Texture2D _fooBlackSquare;
        static Texture2D _fooBlackSquareAlpha10;
        static Texture2D _fooBlackSquareAlpha15;
        static Texture2D _fooBlackSquareAlpha25;
        static Texture2D _fooBlackSquareAlpha50;
        static Texture2D _fooRedSquare;
        static Texture2D _fooOrangeSquare;
        static Texture2D _fooYellowSquare;
        static Texture2D _fooGreenSquare;
        static Texture2D _fooBlueSquare;
        static Texture2D _fooPurpleSquare;

        #endregion

        /// <summary>
        /// Called automatically by <code>DeGUI.BeginGUI</code>.
        /// Override when adding new style subclasses.
        /// </summary>
        internal void Init()
        {
            if (initialized) return;

            initialized = true;

            // Default inits (made manually so they happen before subpalettes, which might be using them)
            box.Init();
            button.Init();
            label.Init();
            toolbar.Init();
            misc.Init();

            // Initialize custome subpalettes from inherited classes
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fieldInfos) {
                if (fi.FieldType.IsSubclassOf(typeof(DeStyleSubPalette))) ((DeStyleSubPalette)fi.GetValue(this)).Init();
            }
        }

        static Texture2D LoadSquareTexture(string name)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath(String.Format("Assets/{0}{1}.png", _adbImgsDir, name), typeof(Texture2D)) as Texture2D;
            tex.SetGUIFormat(FilterMode.Point, 16);
            return tex;
        }
    }

    /// <summary>
    /// Extend any custom subpalettes from this, so they will be initialized correctly
    /// </summary>
    public abstract class DeStyleSubPalette
    {
        public abstract void Init();
    }

    public class Box
    {
        public GUIStyle def,
                        flat, flatAlpha10, flatAlpha25; // Flat with white background
        public GUIStyle sticky, stickyTop; // Without any margin (or only top margin)

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.box).Padding(6, 6, 6, 6);
            flat = new GUIStyle(def).Background(DeStylePalette.whiteSquare);
            flatAlpha10 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha10);
            flatAlpha25 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha25);
            sticky = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(0), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(0));
            stickyTop = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(7), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(7));
        }
    }

    public class Button
    {
        public GUIStyle def,
                        tool, toolL, toolIco,
                        toolFoldoutClosed, toolFoldoutClosedWLabel, toolFoldoutOpen, toolFoldoutOpenWLabel;

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.button);
            tool = new GUIStyle(EditorStyles.toolbarButton).ContentOffsetY(-1);
            toolL = new GUIStyle(EditorStyles.toolbarButton).Height(23).ContentOffsetY(0);
            toolIco = new GUIStyle(tool).StretchWidth(false).Width(22).ContentOffsetX(-1);
//            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
//                alignment = TextAnchor.UpperLeft,
//                active = { background = null },
//                fixedWidth = 14,
//                normal = { background = EditorStyles.foldout.normal.background },
//                border = EditorStyles.foldout.border,
//                padding = new RectOffset(14, 0, 0, 0)
//            }.MarginTop(2);
            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleLeft,
                active = { background = null },
                fixedWidth = 14,
                normal = { background = EditorStyles.foldout.normal.background },
                border = EditorStyles.foldout.border,
                padding = new RectOffset(14, 0, 2, 0)
            }.MarginTop(3);
            toolFoldoutClosedWLabel = toolFoldoutClosed.Clone(9).Width(0).StretchWidth(false);
            toolFoldoutOpen = new GUIStyle(toolFoldoutClosed) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
            toolFoldoutOpenWLabel = new GUIStyle(toolFoldoutClosedWLabel) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
        }
    }

    public class Label
    {
        public GUIStyle bold,
                        wordwrap, wordwrapRichtText,
                        toolbar, toolbarL, toolbarBox;

        internal void Init()
        {
            bold = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold);
            wordwrap = new GUIStyle(GUI.skin.label).Add(Format.WordWrap);
            wordwrapRichtText = wordwrap.Clone(Format.RichText);
            toolbar = new GUIStyle(GUI.skin.label).Add(9).ContentOffset(new Vector2(-2, 0));
            toolbarL = new GUIStyle(toolbar).ContentOffsetY(2);
            toolbarBox = new GUIStyle(toolbar).ContentOffsetY(0);
        }
    }

    public class Toolbar
    {
        public GUIStyle def,
                        large,
                        stickyTop,
                        box,
                        flat; // Flat with white background

        internal void Init()
        {
            def = new GUIStyle(EditorStyles.toolbar).Height(18).StretchWidth();
            large = new GUIStyle(def).Height(23);
            stickyTop = new GUIStyle(def).MarginTop(0);
            box = new GUIStyle(GUI.skin.box).Height(20).StretchWidth().Padding(5, 6, 1, 0).Margin(0, 0, 0, 0);
            flat = new GUIStyle(GUI.skin.box).Height(18).StretchWidth().Padding(5, 6, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }

    public class Misc
    {
        public GUIStyle line; // Flat line with no margin

        internal void Init()
        {
            line = new GUIStyle(GUI.skin.box).Padding(0, 0, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }
}