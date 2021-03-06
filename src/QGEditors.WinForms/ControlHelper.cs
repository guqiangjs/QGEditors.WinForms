﻿// QGEditor
// Copyright (c) 2014-2016 GuQiang - <guqiangjs@gmail.com>
// ALL RIGHTS RESERVED

#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

#endregion

namespace QGEditors.WinForms {
    internal static class ControlHelper {
        internal static Point GetImageLocation(this Control control, ContentAlignment align, Image image) {
            if (control != null && image != null)
                switch (align) {
                    case ContentAlignment.BottomCenter:
                        return new Point((control.ClientSize.Width - image.Width) / 2,
                            control.ClientSize.Height - image.Height);

                    case ContentAlignment.BottomLeft:
                        return new Point(0, control.ClientSize.Height - image.Height);

                    case ContentAlignment.BottomRight:
                        return new Point(control.ClientSize.Width - image.Width,
                            control.ClientSize.Height - image.Height);

                    case ContentAlignment.TopCenter:
                        return new Point((control.ClientSize.Width - image.Width) / 2, 0);

                    case ContentAlignment.TopLeft:
                        return new Point(0, 0);

                    case ContentAlignment.TopRight:
                        return new Point(control.ClientSize.Width - image.Width, 0);

                    case ContentAlignment.MiddleCenter:
                        return new Point((control.ClientSize.Width - image.Width) / 2,
                            (control.ClientSize.Height - image.Height) / 2);

                    case ContentAlignment.MiddleLeft:
                        return new Point(0, (control.ClientSize.Height - image.Height) / 2);

                    case ContentAlignment.MiddleRight:
                        return new Point(control.ClientSize.Width - image.Width,
                            (control.ClientSize.Height - image.Height) / 2);
                }
            return new Point(0, 0);
        }

        internal static void DrawBackgroundImage(this Graphics g, Image backgroundImage, Color backColor,
            ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset,
            RightToLeft rightToLeft) {
            if (g == null)
                throw new ArgumentNullException("g");
            if (backgroundImageLayout == ImageLayout.Tile)
                using (var brush = new TextureBrush(backgroundImage, WrapMode.Tile)) {
                    if (scrollOffset != Point.Empty) {
                        var transform = brush.Transform;
                        transform.Translate(scrollOffset.X, scrollOffset.Y);
                        brush.Transform = transform;
                    }
                    g.FillRectangle(brush, clipRect);
                    return;
                }
            var rect = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
            if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                rect.X += clipRect.Width - rect.Width;
            using (var brush2 = new SolidBrush(backColor)) {
                g.FillRectangle(brush2, clipRect);
            }
            if (!clipRect.Contains(rect))
                if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom) {
                    rect.Intersect(clipRect);
                    g.DrawImage(backgroundImage, rect);
                }
                else if (backgroundImageLayout == ImageLayout.None) {
                    rect.Offset(clipRect.Location);
                    var destRect = rect;
                    destRect.Intersect(clipRect);
                    var rectangle3 = new Rectangle(Point.Empty, destRect.Size);
                    g.DrawImage(backgroundImage, destRect, rectangle3.X, rectangle3.Y, rectangle3.Width,
                        rectangle3.Height, GraphicsUnit.Pixel);
                }
                else {
                    var rectangle4 = rect;
                    rectangle4.Intersect(clipRect);
                    var rectangle5 = new Rectangle(new Point(rectangle4.X - rect.X, rectangle4.Y - rect.Y),
                        rectangle4.Size);
                    g.DrawImage(backgroundImage, rectangle4, rectangle5.X, rectangle5.Y, rectangle5.Width,
                        rectangle5.Height, GraphicsUnit.Pixel);
                }
            else
                using (var imageAttr = new ImageAttributes()) {
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(backgroundImage, rect, 0, 0, backgroundImage.Width, backgroundImage.Height,
                        GraphicsUnit.Pixel, imageAttr);
                }
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage,
            ImageLayout imageLayout) {
            var rectangle = bounds;
            if (backgroundImage != null)
                switch (imageLayout) {
                    case ImageLayout.None:
                        rectangle.Size = backgroundImage.Size;
                        return rectangle;

                    case ImageLayout.Tile:
                        return rectangle;

                    case ImageLayout.Center: {
                        rectangle.Size = backgroundImage.Size;
                        var size = bounds.Size;
                        if (size.Width > rectangle.Width)
                            rectangle.X = (size.Width - rectangle.Width) / 2;
                        if (size.Height > rectangle.Height)
                            rectangle.Y = (size.Height - rectangle.Height) / 2;
                        return rectangle;
                    }
                    case ImageLayout.Stretch:
                        rectangle.Size = bounds.Size;
                        return rectangle;

                    case ImageLayout.Zoom: {
                        var size2 = backgroundImage.Size;
                        var num = bounds.Width / (float) size2.Width;
                        var num2 = bounds.Height / (float) size2.Height;
                        if (num >= num2) {
                            rectangle.Height = bounds.Height;
                            rectangle.Width = (int) (size2.Width * num2 + 0.5);
                            if (bounds.X >= 0)
                                rectangle.X = (bounds.Width - rectangle.Width) / 2;
                            return rectangle;
                        }
                        rectangle.Width = bounds.Width;
                        rectangle.Height = (int) (size2.Height * num + 0.5);
                        if (bounds.Y >= 0)
                            rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                        return rectangle;
                    }
                }
            return rectangle;
        }
    }
}