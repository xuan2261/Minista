﻿using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

namespace Windows.UI.Xaml
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="FrameworkElement"/> <see langword="class"/>
    /// </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Sets the <see cref="Visual.CenterPoint"/> property of the <see cref="Visual"/> behind a given <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="element">The source <see cref="FrameworkElement"/></param>
        /// <param name="x">The center to use on the X axis. If <see langword="null"/>, the center of the <see cref="FrameworkElement"/> will be used</param>
        /// <param name="y">The center to use on the Y axis. If <see langword="null"/>, the center of the <see cref="FrameworkElement"/> will be used</param>
        public static void SetVisualCenterPoint( this FrameworkElement element, double? x, double? y)
        {
            if (double.IsNaN(element.Width) || double.IsNaN(element.Height))
                throw new InvalidOperationException("The target element must have a fixed size");
            element.GetVisual().CenterPoint = new Vector3((float?)x ?? (float)(element.Width / 2), (float?)y ?? (float)(element.Height / 2), 0);
        }

        /// <summary>
        /// Creates a <see cref="DropShadow"/> from the given <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="element">The source <see cref="FrameworkElement"/> for the <see cref="DropShadow"/></param>
        /// <param name="target">The optional target <see cref="FrameworkElement"/> to apply the <see cref="DropShadow"/> to (it can be the same as the source <see cref="FrameworkElement"/>)</param>
        /// <param name="apply">Indicates whether or not to immediately add the <see cref="DropShadow"/> to the visual tree</param>
        /// <param name="width">The optional width of the <see cref="DropShadow"/> (if null, the element width will be used)</param>
        /// <param name="height">The optional height of the <see cref="DropShadow"/> (if null, the element height will be used)</param>
        /// <param name="color">The <see cref="DropShadow"/> color (the default is <see cref="Colors.Black"/></param>
        /// <param name="opacity">The opacity of the <see cref="DropShadow"/></param>
        /// <param name="offsetX">The optional horizontal offset of the <see cref="DropShadow"/></param>
        /// <param name="offsetY">The optional vertical offset of the <see cref="DropShadow"/></param>
        /// <param name="clipMargin">The optional margin of the clip area of the <see cref="DropShadow"/></param>
        /// <param name="clipOffsetX">The optional horizontal offset of the clip area of the <see cref="DropShadow"/></param>
        /// <param name="clipOffsetY">The optional vertical offset of the clip area of the <see cref="DropShadow"/></param>
        /// <param name="blurRadius">The optional explicit <see cref="DropShadow"/> blur radius</param>
        /// <param name="maskElement">The optional <see cref="UIElement"/> to use to create an alpha mask for the <see cref="DropShadow"/></param>
        /// <returns>The <see cref="SpriteVisual"/> that hosts the <see cref="DropShadow"/></returns>
        
        public static SpriteVisual AttachVisualShadow(
             this FrameworkElement element,  UIElement target,
            bool apply,
            float? width, float? height,
            Color color, float opacity,
            float offsetX = 0, float offsetY = 0,
            Thickness? clipMargin = null, float clipOffsetX = 0, float clipOffsetY = 0,
            float? blurRadius = null,
             UIElement maskElement = null)
        {
            // Setup the shadow
            Visual elementVisual = ElementCompositionPreview.GetElementVisual(element);
            Compositor compositor = elementVisual.Compositor;
            SpriteVisual sprite = compositor.CreateSpriteVisual();
            DropShadow shadow = compositor.CreateDropShadow();
            shadow.Color = color;
            shadow.Opacity = opacity;
            shadow.Offset = new Vector3(offsetX, offsetY, 0);
            if (blurRadius != null) shadow.BlurRadius = blurRadius.Value;
            sprite.Shadow = shadow;
            sprite.Size = new Vector2(width ?? (float)element.Width, height ?? (float)element.Height);

            // Clip it (if needed) and add it to the visual tree
            if (clipMargin != null || clipOffsetX > 0 || clipOffsetY > 0)
            {
                InsetClip clip = compositor.CreateInsetClip(
                (float)(clipMargin?.Left ?? 0), (float)(clipMargin?.Top ?? 0),
                (float)(clipMargin?.Right ?? 0), (float)(clipMargin?.Bottom ?? 0));
                clip.Offset = new Vector2(clipOffsetX, clipOffsetY);
                sprite.Clip = clip;
            }

            // Alpha mask
            switch (maskElement)
            {
                case null: break;
                case Shape shape: shadow.Mask = shape.GetAlphaMask(); break;
                case Image image: shadow.Mask = image.GetAlphaMask(); break;
                case TextBlock textBlock: shadow.Mask = textBlock.GetAlphaMask(); break;
            }
            if (apply) ElementCompositionPreview.SetElementChildVisual(target, sprite);
            return sprite;
        }
    }
}
