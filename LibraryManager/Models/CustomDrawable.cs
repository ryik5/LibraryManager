namespace LibraryManager.Models;

public class CustomDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Determine scaling factor based on viewBox (128x128) and dirtyRect (actual GraphicsView dimensions)
        float originalWidth = 128f; // From SVG viewBox width
        float originalHeight = 128f; // From SVG viewBox height
        float scaleX = dirtyRect.Width / originalWidth;
        float scaleY = dirtyRect.Height / originalHeight;
        float scale = Math.Min(scaleX, scaleY); // Uniform scaling to preserve aspect ratio

        // Center the drawing by translating it
        float offsetX = (dirtyRect.Width - (originalWidth * scale)) / 2;
        float offsetY = (dirtyRect.Height - (originalHeight * scale)) / 2;

        canvas.SaveState(); // Save the canvas state before transforming it

        canvas.Translate(offsetX, offsetY); // Center the drawing within the GraphicsView
        canvas.Scale(scale, scale); // Scale the drawing to fit the GraphicsView

        // Set the fill color (white) and stroke properties
        canvas.FillColor = Colors.White;
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;

        // First Path
        PathF path1 = new PathF();
        path1.MoveTo(55.33f, 9.68f);
        path1.LineTo(13.09f, 9.68f);
        path1.QuadTo(7.08f, 9.68f, 2.21f, 14.55f);
        path1.LineTo(2.21f, 96.8f);
        path1.QuadTo(2.21f, 102.81f, 7.08f, 107.68f);
        path1.LineTo(13.09f, 107.68f);
        path1.LineTo(40.95f, 107.68f);
        path1.QuadTo(36.24f, 104.21f, 33.21f, 98.78f);
        path1.QuadTo(33.21f, 92.68f, 42.16f, 73.68f);
        path1.LineTo(53.21f, 73.68f);
        path1.LineTo(56.12f, 73.88f);
        path1.QuadTo(60.99f, 67.59f, 66.21f, 59f);
        path1.LineTo(66.21f, 20.56f);
        path1.QuadTo(66.21f, 14.55f, 55.33f, 9.68f);
        path1.Close();

        path1.MoveTo(53.21f, 33.68f);
        path1.LineTo(15.21f, 33.68f);
        path1.LineTo(15.21f, 25.68f);
        path1.LineTo(53.21f, 25.68f);
        path1.Close();

        canvas.FillPath(path1);
        canvas.DrawPath(path1);

        // Second Path
        PathF path2 = new PathF();
        path2.MoveTo(114.09f, 87.33f);
        path2.LineTo(114.05f, 87.33f);
        path2.QuadTo(114.07f, 87.08f, 114.1f, 86.58f);
        path2.QuadTo(114.2f, 81.09f, 104.33f, 76.45f);
        path2.QuadTo(103.73f, 76.44f, 102.55f, 76.59f);
        path2.QuadTo(91.93f, 58.87f, 81.31f, 58.67f);
        path2.QuadTo(77.05f, 58.59f, 69.63f, 61.76f);
        path2.QuadTo(64.15f, 65.02f, 59.13f, 77.18f);
        path2.QuadTo(38.74f, 101.9f, 55.32f, 110.29f);
        path2.LineTo(113.64f, 111.38f);
        path2.QuadTo(120.28f, 111.5f, 125.87f, 99.29f);
        path2.LineTo(114.09f, 87.33f);
        path2.Close();

        path2.MoveTo(90.78f, 84.38f);
        path2.LineTo(90.78f, 88.94f);
        path2.LineTo(90.78f, 89.55f);
        path2.LineTo(90.78f, 103.67f);
        path2.LineTo(73.08f, 103.67f);
        path2.LineTo(73.08f, 89.55f);
        path2.LineTo(73.08f, 88.94f);
        path2.LineTo(73.08f, 84.38f);
        path2.LineTo(68.54f, 84.38f);
        path2.LineTo(81.93f, 71.34f);
        path2.LineTo(95.32f, 84.38f);
        path2.LineTo(90.78f, 84.38f);
        path2.Close();

        canvas.FillPath(path2);
        canvas.DrawPath(path2);

        canvas.RestoreState(); // Restore canvas state to remove transformations
    }
}
