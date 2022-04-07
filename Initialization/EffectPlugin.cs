using PaintDotNet;
using PaintDotNet.Effects;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using DynamicDraw.Properties;

namespace DynamicDraw
{
    /// <summary>
    /// Called remotely by Paint.Net. In short, a GUI is instantiated by
    /// <see cref="CreateConfigDialog"/> and when the dialog signals OK, Render is called,
    /// passing OnSetRenderInfo to it. The dialog stores its result in an
    /// intermediate class called <see cref="RenderSettings"/>, which is then accessed to
    /// draw the final result in Render.
    /// </summary>
    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Dynamic Draw")]
    public class EffectPlugin : Effect
    {
        #region Properties
        /// <summary>
        /// The icon of the plugin to be displayed next to its menu entry.
        /// </summary>
        public static Bitmap StaticImage
        {
            get
            {
                return Resources.IconPng;
            }
        }

        /// <summary>
        /// The name of the plugin as it appears in Paint.NET.
        /// </summary>
        public static string StaticName
        {
            get
            {
                return Localization.Strings.Title;
            }
        }

        /// <summary>
        /// The name of the menu category the plugin appears under.
        /// </summary>
        public static string StaticSubMenuName
        {
            get
            {
                return "Tools";
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public EffectPlugin()
            : base(
            StaticName,
            StaticImage,
            StaticSubMenuName,
            new EffectOptions() { Flags = EffectFlags.Configurable })
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tells Paint.NET which form to instantiate as the plugin's GUI.
        /// Called remotely by Paint.NET.
        /// </summary>
        public override EffectConfigDialog CreateConfigDialog()
        {
            //Copies necessary user variables for dialog access.
            UserSettings.userPrimaryColor = EnvironmentParameters.PrimaryColor;

            //Static variables are remembered between plugin calls, so clear them.
            RenderSettings.Clear();

            //Creates and returns a new dialog.
            return new WinDynamicDraw();
        }

        /// <summary>
        /// Gets the render information.
        /// </summary>
        /// <param name="parameters">
        /// Saved settings used to restore the GUI to the same settings it was
        /// saved with last time the effect was applied.
        /// </param>
        /// <param name="dstArgs">The destination canvas.</param>
        /// <param name="srcArgs">The source canvas.</param>
        protected override void OnSetRenderInfo(
            EffectConfigToken parameters,
            RenderArgs dstArgs,
            RenderArgs srcArgs)
        {
            //Copies the render information to the base Effect class.
            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        /// <summary>
        /// Renders the effect over rectangular regions automatically
        /// determined and handled by Paint.NET for multithreading support.
        /// </summary>
        /// <param name="parameters">
        /// Saved settings used to restore the GUI to the same settings it was
        /// saved with last time the effect was applied.
        /// </param>
        /// <param name="dstArgs">The destination canvas.</param>
        /// <param name="srcArgs">The source canvas.</param>
        /// <param name="rois">
        /// A list of rectangular regions to split this effect into so it can
        /// be optimized by worker threads. Determined and managed by
        /// Paint.NET.
        /// </param>
        /// <param name="startIndex">
        /// The rectangle to begin rendering with. Used in Paint.NET's effect
        /// multithreading process.
        /// </param>
        /// <param name="length">
        /// The number of rectangles to render at once. Used in Paint.NET's
        /// effect multithreading process.
        /// </param>
        public override void Render(
            EffectConfigToken parameters,
            RenderArgs dstArgs,
            RenderArgs srcArgs,
            Rectangle[] rois,
            int startIndex,
            int length)
        {
            //Renders the effect if the dialog is closed and accepted.
            if (!RenderSettings.EffectApplied &&
                RenderSettings.DoApplyEffect && !IsCancelRequested)
            {
                //The effect should only render once.
                RenderSettings.EffectApplied = true;

                dstArgs.Surface.CopySurface(
                    RenderSettings.SurfaceToRender,
                    EnvironmentParameters.GetSelectionAsPdnRegion());
            }
        }
        #endregion
    }
}