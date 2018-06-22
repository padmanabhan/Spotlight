using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LossOfColorBug.Services {
    public class ThumbnailService {

        public virtual Image GetThumbnailImage(Image source, int width, int height, ResizeMode mode, Color paddingColor)
        {
            Rectangle canvasSize = CalculateCanvasRectangle(source, width, height, mode);
            Bitmap canvas = new Bitmap(canvasSize.Width, canvasSize.Height, PixelFormat.Format24bppRgb);
            Rectangle sourceRectangle = CalculateCropZoomRectangle(source.Width, source.Height, canvas.Width,
                                                                   canvas.Height);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                using (SolidBrush brush = new SolidBrush(paddingColor))
                {
                    graphics.FillRectangle(brush, 0, 0, canvas.Width, canvas.Height);

                    Point upperLeft = new Point(0, 0);
                    Point upperRight = new Point(canvas.Width, 0);
                    Point lowerLeft = new Point(0, canvas.Height);

                    Point[] destinationPoints = new Point[] { upperLeft, upperRight, lowerLeft };
                    ImageAttributes ia = new ImageAttributes();
                    ia.SetGamma(1.5f);
                    graphics.DrawImage(source, destinationPoints, sourceRectangle, GraphicsUnit.Pixel, ia);
                }
            }
            return (canvas);
        }

        public virtual Image GetImage(string filepath) {
            return new Bitmap(filepath);
        }

        private Rectangle CalculateCanvasRectangle(Image source, int targetWidth, int targetHeight,
                                                          ResizeMode mode)
        {
            Rectangle result = new Rectangle();
            switch (mode)
            {
                case ResizeMode.Crop:
                case ResizeMode.Letterbox:
                    result.Width = targetWidth;
                    result.Height = targetHeight;
                    break;
                case ResizeMode.Loose:
                    float sourceAspectRatio = (float)source.Width / (float)source.Height;
                    float targetAspectRatio = (float)targetWidth / (float)targetHeight;
                    if (sourceAspectRatio > targetAspectRatio)
                    {
                        float scaleFactor = ((float)targetWidth / (float)source.Width);
                        result.Width = targetWidth;
                        result.Height = (int)(source.Height * scaleFactor);
                    }
                    else
                    {
                        float scaleFactor = ((float)targetHeight / (float)source.Height);
                        result.Height = targetHeight;
                        result.Width = (int)(source.Width * scaleFactor);
                    }
                    break;
            }
            return (result);
        }


        private Rectangle CalculateCropZoomRectangle(int sourceWidth, int sourceHeight, int targetWidth,
                                                            int targetHeight)
        {
            // Typical aspect ratios are 1.0 (for a square image), 0.8 (for a 10x8 portrait), 1.77 (for a still from a 16:9 widescreen movie)
            float sourceAspectRatio = (float)sourceWidth / (float)sourceHeight;
            float targetAspectRatio = (float)targetWidth / (float)targetHeight;

            // There are two scenarios here:
            // 1. We're fitting a taller picture into a wider frame - we need to crop the source top/bottom OR pad left/right
            // 2. We're fitting a wider picture into a taller frame - we need to crop the source left/right OR pad top/bottom
            // If the aspect ratio of the source is greater than that of the target, it means we're fitting wide content into a tall frame.

            Rectangle result = new Rectangle();
            float scaleFactor;
            if (sourceAspectRatio > targetAspectRatio)
            {
                scaleFactor = (float)targetHeight / (float)sourceHeight;
                result.Width = (int)(targetWidth / scaleFactor);
                result.Height = sourceHeight;
            }
            else
            {
                scaleFactor = (float)targetWidth / (float)sourceWidth;
                result.Width = sourceWidth;
                result.Height = (int)(targetHeight / scaleFactor);
            }
            result.Location = CenterRectangle(sourceWidth, sourceHeight, result);
            return (result);
        }

        private static Point CenterRectangle(int width, int height, Rectangle result)
        {
            Point p = new Point(0, 0);
            p.Y = (int)((float)(height - result.Height) / 2.0);
            p.X = (int)((float)(width - result.Width) / 2.0);
            return (p);
        }

        public enum ResizeMode
        {
            /// <summary>
            /// Fill the frame by cropping portions of the source image that don't fit.
            /// </summary>
            Crop,

            /// <summary>
            /// Draw the entire image, and fill the frame by padding with a background color.
            /// </summary>
            Letterbox,

            /// <summary>
            /// Resize the image, preserving aspect ratio, to fit within the specified frame.
            /// </summary>
            Loose
        }
    }
}