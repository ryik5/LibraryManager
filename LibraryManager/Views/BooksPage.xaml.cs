using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class BooksPage : ContentPage
{
    public BooksPage()
    {
        InitializeComponent();
        
        // Initialize a tooltip label to show dynamically
        _tooltipLabel = new Label
        {
            BackgroundColor = Colors.LightGray,
            TextColor = Colors.Black,
            Padding = 5,
            IsVisible = false, // Hide the tooltip initially
            ZIndex = 99
        };
        
        // Add the tooltip to the parent layout
        var mainLayout = this.Content as Grid;
        if (mainLayout != null)
        {
            mainLayout.Children.Add(_tooltipLabel);
        }
    }
    
    // Triggered when the pointer enters an element
    private void OnPointerEntered(object sender, EventArgs e)
    {
        if (sender is HoverableLabel hoveredLabel)
        {
            _tooltipLabel.Text = GetTooltipText(hoveredLabel); // Generate tooltip text dynamically
            _tooltipLabel.IsVisible = true;

            // Adjust the tooltip's position
            var position = hoveredLabel.Bounds.Location;
            _tooltipLabel.TranslationX = position.X + 20; // Offset for better visibility
            _tooltipLabel.TranslationY = position.Y + 10;
        }
    }

    // Triggered when the pointer exits an element
    private void OnPointerExited(object sender, EventArgs e)
    {
        _tooltipLabel.IsVisible = false; // Hide the tooltip when exiting
    }

    
    // TODO : do normal code
    // A helper to define tooltips dynamically for each field
    private string GetTooltipText(Label label)
    {
        // Example: Match label text to tooltip content
        if (label.Text == "1984")
            return "A dystopian novel by George Orwell.";
        if (label.Text == "Pride and Prejudice")
            return "A classic novel by Jane Austen.";

        return $"Tooltip for {label.Text}";
    }

    
    private Label _tooltipLabel;
}