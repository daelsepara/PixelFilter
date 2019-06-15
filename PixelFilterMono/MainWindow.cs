using Gdk;
using Gtk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public partial class MainWindow : Gtk.Window
{
    Pixbuf InputPixbuf, OutputPixbuf;
    FileChooserDialog ImageSaver, ImageLoader;
    String FileName;

    Dialog Confirm;
    TextIter start, end;

    List<FilterApplication> FilterQueue = new List<FilterApplication>();

    CultureInfo ci = new CultureInfo("en-us");

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        InitializeUserInterface();

        InitializeFilters();
    }

    protected FileFilter AddFilter(string name, params string[] patterns)
    {
        var filter = new FileFilter() { Name = name };

        foreach (var pattern in patterns)
            filter.AddPattern(pattern);

        return filter;
    }

    protected void InitializeUserInterface()
    {
        Title = "Pixel Filter in Mono/C# GTK";

        InputPixbuf = Common.InitializePixbuf(InputImage.WidthRequest, InputImage.HeightRequest);
        OutputPixbuf = Common.InitializePixbuf(OutputImage.WidthRequest, OutputImage.HeightRequest);

        InputImage.Pixbuf = Common.InitializePixbuf(InputImage.WidthRequest, InputImage.HeightRequest);
        OutputImage.Pixbuf = Common.InitializePixbuf(OutputImage.WidthRequest, OutputImage.HeightRequest);

        ResetScrollBars();

        RenderInput();
        RenderOutput();

        ImageSaver = new FileChooserDialog(
            "Save Filtered Image",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Save", ResponseType.Accept
        );

        ImageLoader = new FileChooserDialog(
            "Load Image",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Load", ResponseType.Accept
        );

        ImageSaver.AddFilter(AddFilter("png", "*.png"));
        ImageSaver.AddFilter(AddFilter("jpg", "*.jpg", "*.jpeg"));
        ImageSaver.AddFilter(AddFilter("tif", "*.tif", "*.tiff"));
        ImageSaver.AddFilter(AddFilter("bmp", "*.bmp"));
        ImageSaver.AddFilter(AddFilter("ico", "*.ico"));

        ImageLoader.AddFilter(AddFilter("Image files (png/jpg/jpeg/tif/tiff/bmp/gif/ico/xpm/icns/pgm)", "*.png", "*.jpg", "*.jpeg", "*.tif", "*.tiff", "*.bmp", "*.gif", "*.ico", "*.xpm", "*.icns", "*.pgm"));

        Confirm = new Dialog(
            "Are you sure?",
            this,
            DialogFlags.Modal,
            "Yes", ResponseType.Accept,
            "No", ResponseType.Cancel
        )
        {
            Resizable = false,
            KeepAbove = true,
            TypeHint = WindowTypeHint.Dialog,
            WidthRequest = 250
        };

        Confirm.ActionArea.LayoutStyle = ButtonBoxStyle.Center;
        Confirm.WindowStateEvent += OnWindowStateEvent;
    }

    protected void InitializeFilters()
    {
        Filters.Initialize();

        UpdateFilterBox(FilterBox, Filters.FilterList);
    }

    protected void RenderInput()
    {
        CopyToImage(InputImage, InputPixbuf, Parameters.InputX, Parameters.InputY);
    }

    protected void RenderOutput()
    {
        CopyToImage(OutputImage, OutputPixbuf, Parameters.OutputX, Parameters.OutputY);
    }

    protected void DrawToImage(Gtk.Image image, Pixbuf pixbuf, int OriginX, int OriginY)
    {
        if (pixbuf != null)
        {
            var dest = image.GdkWindow;

            var gc = new Gdk.GC(dest);

            dest.DrawPixbuf(gc, pixbuf, OriginX, OriginY, 0, 0, Math.Min(Parameters.WindowX, pixbuf.Width), Math.Min(Parameters.WindowY, pixbuf.Height), RgbDither.None, 0, 0);
        }
    }

    protected void CopyToImage(Gtk.Image image, Pixbuf pixbuf, int OriginX, int OriginY)
    {
        if (pixbuf != null && image.Pixbuf != null)
        {
            image.Pixbuf.Fill(0);

            pixbuf.CopyArea(OriginX, OriginY, Math.Min(Parameters.WindowX, pixbuf.Width), Math.Min(Parameters.WindowY, pixbuf.Height), image.Pixbuf, 0, 0);

            image.QueueDraw();
        }
    }

    protected void ResetInputScrollBars()
    {
        InputScrollX.Value = 0;
        InputScrollY.Value = 0;

        InputScrollX.Sensitive = InputPixbuf.Width > Parameters.WindowX;
        InputScrollY.Sensitive = InputPixbuf.Height > Parameters.WindowY;

        InputScrollX.Visible = InputPixbuf.Width > Parameters.WindowX;
        InputScrollY.Visible = InputPixbuf.Height > Parameters.WindowY;

        if (InputScrollX.Sensitive)
        {
            InputScrollX.SetRange(0, InputPixbuf.Width - Parameters.WindowX);
        }
        else
        {
            InputScrollX.SetRange(0, Parameters.WindowX);
        }

        if (InputScrollY.Sensitive)
        {
            InputScrollY.SetRange(0, InputPixbuf.Height - Parameters.WindowY);
        }
        else
        {
            InputScrollY.SetRange(0, Parameters.WindowY);
        }

        Parameters.InputX = 0;
        Parameters.InputY = 0;
    }

    protected void ResetOutputScrollBars()
    {
        OutputScrollX.Value = 0;
        OutputScrollY.Value = 0;

        OutputScrollX.Sensitive = OutputPixbuf.Width > Parameters.WindowX;
        OutputScrollY.Sensitive = OutputPixbuf.Height > Parameters.WindowY;

        OutputScrollX.Visible = OutputPixbuf.Width > Parameters.WindowX;
        OutputScrollY.Visible = OutputPixbuf.Height > Parameters.WindowY;

        if (OutputScrollX.Sensitive)
        {
            OutputScrollX.SetRange(0, OutputPixbuf.Width - Parameters.WindowX);
        }
        else
        {
            OutputScrollX.SetRange(0, Parameters.WindowX);
        }

        if (OutputScrollY.Sensitive)
        {
            OutputScrollY.SetRange(0, OutputPixbuf.Height - Parameters.WindowY);
        }
        else
        {
            OutputScrollY.SetRange(0, Parameters.WindowY);
        }

        Parameters.OutputX = 0;
        Parameters.OutputY = 0;
    }

    protected void ResetScrollBars()
    {
        ResetInputScrollBars();
        ResetOutputScrollBars();
    }

    protected void LoadImageFile()
    {
        // Add most recent directory
        if (!string.IsNullOrEmpty(ImageLoader.Filename))
        {
            var directory = System.IO.Path.GetDirectoryName(ImageLoader.Filename);

            if (Directory.Exists(directory))
            {
                ImageLoader.SetCurrentFolder(directory);
            }
        }

        if (ImageLoader.Run() == (int)ResponseType.Accept)
        {
            if (!string.IsNullOrEmpty(ImageLoader.Filename))
            {
                FileName = ImageLoader.Filename;

                try
                {
                    var temp = new Pixbuf(FileName);

                    if (InputPixbuf != null && temp != null)
                    {
                        Common.Free(InputPixbuf);

                        InputPixbuf = Common.InitializePixbuf(temp.Width, temp.Height);

                        temp.Composite(InputPixbuf, 0, 0, temp.Width, temp.Height, 0, 0, 1, 1, InterpType.Nearest, 255);

                        ResetInputScrollBars();

                        RenderInput();

                        LabelInput.LabelProp = String.Format(ci, "<b>Input ({0}x{1})</b>", InputPixbuf.Width, InputPixbuf.Height);
                    }

                    Common.Free(temp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
        }

        ImageLoader.Hide();
    }

    protected string GetFileName(string fullpath)
    {
        return System.IO.Path.GetFileNameWithoutExtension(fullpath);
    }


    protected string GetName(string fullpath)
    {
        return System.IO.Path.GetFileName(fullpath);
    }

    protected string GetDirectory(string fullpath)
    {
        return System.IO.Path.GetDirectoryName(fullpath);
    }

    protected void SaveImageFile()
    {
        ImageSaver.Title = "Save filtered image";

        string directory;

        // Add most recent directory
        if (!string.IsNullOrEmpty(ImageSaver.Filename))
        {
            directory = GetDirectory(ImageSaver.Filename);

            if (Directory.Exists(directory))
            {
                ImageSaver.SetCurrentFolder(directory);
            }
        }

        if (ImageSaver.SelectFilename(FileName) && ImageSaver.Run() == (int)ResponseType.Accept)
        {
            if (!string.IsNullOrEmpty(ImageSaver.Filename))
            {
                FileName = ImageSaver.Filename;

                directory = GetDirectory(FileName);

                var ext = ImageSaver.Filter.Name;

                var fmt = ext;

                switch (ext)
                {
                    case "jpg":

                        if (!FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            FileName = String.Format("{0}.jpg", GetFileName(FileName));
                        }

                        fmt = "jpeg";

                        break;

                    case "tif":

                        if (!FileName.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) && !FileName.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
                        {
                            FileName = String.Format("{0}.tif", GetFileName(FileName));
                        }

                        fmt = "tiff";

                        break;

                    default:

                        FileName = String.Format("{0}.{1}", GetFileName(FileName), ext);

                        break;
                }

                if (OutputPixbuf != null)
                {
                    FileName = GetName(FileName);

                    var fullpath = String.Format("{0}/{1}", directory, FileName);

                    try
                    {
                        OutputPixbuf.Save(fullpath, fmt);

                        FileName = fullpath;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving {0}: {1}", FileName, ex.Message);
                    }
                }
            }
        }

        ImageSaver.Hide();
    }

    protected void UpdateFilterBox(ComboBox combo, List<FilterPlugin> filters)
    {
        combo.Clear();

        var cell = new CellRendererText();
        combo.PackStart(cell, false);
        combo.AddAttribute(cell, "text", 0);
        var store = new ListStore(typeof(string));
        combo.Model = store;

        foreach (var filter in filters)
        {
            store.AppendValues(filter.Name);
        }

        combo.Active = -1;
    }

    protected void UpdateFilterParameterBox(ComboBox combo, List<int> parameters)
    {
        combo.Clear();

        var cell = new CellRendererText();
        combo.PackStart(cell, false);
        combo.AddAttribute(cell, "text", 0);
        var store = new ListStore(typeof(string));
        combo.Model = store;

        foreach (var parameter in parameters.ToArray())
        {
            store.AppendValues(parameter.ToString());
        }

        combo.Active = parameters.Count > 0 ? 0 : -1;
    }

    protected void UpdateFilterQueue(List<FilterApplication> Queue)
    {
        FilterQueueBox.Buffer.Clear();
        FilterQueueBox.Buffer.Text = "";

        if (Queue.Count > 0)
        {
            var line = 0;

            foreach (var item in Queue)
            {
                FilterQueueBox.Buffer.Text += item.Name;

                if (line < Queue.Count - 1)
                    FilterQueueBox.Buffer.Text += "\n";

                line++;
            }
        }
    }

    protected bool GetConfirmation()
    {
        var confirm = Confirm.Run() == (int)ResponseType.Accept;

        Confirm.Hide();

        return confirm;
    }

    protected void CleanShutdown()
    {
        Common.Free(InputPixbuf, OutputPixbuf, InputImage.Pixbuf, OutputImage.Pixbuf);
    }

    protected void Quit()
    {
        CleanShutdown();

        Application.Quit();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        if (GetConfirmation())
        {
            Quit();
        }

        a.RetVal = true;
    }

    protected void OnWindowStateEvent(object sender, WindowStateEventArgs args)
    {
        var state = args.Event.NewWindowState;

        if (state == WindowState.Iconified)
        {
            Confirm.Hide();
        }

        args.RetVal = true;
    }

    void OnQuitButtonClicked(object sender, EventArgs args)
    {
        OnDeleteEvent(sender, new DeleteEventArgs());
    }

    protected void OnOpenButtonClicked(object sender, EventArgs e)
    {
        LoadImageFile();
    }

    protected void OnSaveButtonClicked(object sender, EventArgs e)
    {
        SaveImageFile();
    }

    protected void OnInputScrollXValueChanged(object sender, EventArgs e)
    {
        Parameters.InputX = Convert.ToInt32(InputScrollX.Value);

        RenderInput();
    }

    protected void OnInputScrollYValueChanged(object sender, EventArgs e)
    {
        Parameters.InputY = Convert.ToInt32(InputScrollY.Value);

        RenderInput();
    }

    protected void OnOutputScrollXValueChanged(object sender, EventArgs e)
    {
        Parameters.OutputX = Convert.ToInt32(OutputScrollX.Value);

        RenderOutput();
    }

    protected void OnOutputScrollYValueChanged(object sender, EventArgs e)
    {
        Parameters.OutputY = Convert.ToInt32(OutputScrollY.Value);

        RenderOutput();
    }

    protected void OnFilterBoxChanged(object sender, EventArgs e)
    {
        if (FilterBox.Active >= 0 && FilterBox.Active < Filters.FilterList.Count)
        {
            FilterDescription.Sensitive = true;
            FilterDescriptionWindow.Sensitive = true;
            FilterDescription.Buffer.Text = Filters.FilterList[FilterBox.Active].Description;

            FilterParameters.Sensitive = true;
            LabelParameterName.LabelProp = String.Format("<b>{0}</b>", Filters.FilterList[FilterBox.Active].ParameterName);

            UpdateFilterParameterBox(FilterParameters, Filters.FilterList[FilterBox.Active].FilterParameters);
        }
        else
        {
            FilterParameters.Sensitive = false;
            FilterDescription.Sensitive = false;
            FilterDescriptionWindow.Sensitive = false;

            FilterParameters.Sensitive = false;
            LabelParameterName.LabelProp = "<b>Parameter Name</b>";
        }
    }

    void OnAddFilterButtonClicked(object sender, EventArgs args)
    {
        var filter = FilterBox.Active;
        var param = FilterParameters.Active;

        if (filter >= 0 && filter < Filters.FilterList.Count)
        {
            var item = Filters.FilterList[filter];

            if (param >= 0 && param < item.FilterParameters.Count)
            {
                FilterQueue.Add(new FilterApplication(item.Name, item.Library, item.Apply, item.FilterParameters[param]));

                UpdateFilterQueue(FilterQueue);
            }
        }
    }

    void OnClearQueueButtonClicked(object sender, EventArgs args)
    {
        FilterQueue.Clear();

        UpdateFilterQueue(FilterQueue);
    }

    void OnRemoveFilterButtonClicked(object sender, EventArgs args)
    {
        FilterQueueBox.Buffer.GetSelectionBounds(out start, out end);

        var filter = start.Line;

        var queue = new List<FilterApplication>();

        if (start.Line >= 0 && start.Line < FilterQueue.Count && end.Line >= 0 && end.Line < FilterQueue.Count)
        {
            for (var i = 0; i < FilterQueue.Count; i++)
            {
                if (i < start.Line || i > end.Line)
                    queue.Add(FilterQueue[i]);
            }

            FilterQueue.Clear();

            FilterQueue.AddRange(queue);

            UpdateFilterQueue(FilterQueue);
        }
    }

    void OnFilterUpButtonClicked(object sender, EventArgs args)
    {
        if (FilterQueue.Count <= 0)
            return;

        FilterQueueBox.Buffer.GetSelectionBounds(out start, out end);

        if (start.Line <= 0)
            return;

        var queue = new List<FilterApplication>();

        for (var i = 0; i < start.Line - 1; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        for (var i = start.Line; i < end.Line + 1; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        queue.Add(FilterQueue[start.Line - 1]);

        for (var i = end.Line + 1; i < FilterQueue.Count; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        FilterQueue.Clear();

        FilterQueue.AddRange(queue);

        UpdateFilterQueue(FilterQueue);
    }

    void OnFilterDownButtonClicked(object sender, EventArgs args)
    {
        if (FilterQueue.Count <= 0)
            return;

        FilterQueueBox.Buffer.GetSelectionBounds(out start, out end);

        if (end.Line == FilterQueue.Count - 1)
            return;

        var queue = new List<FilterApplication>();

        for (var i = 0; i < start.Line; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        queue.Add(FilterQueue[end.Line + 1]);

        for (var i = start.Line; i < end.Line + 1; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        for (var i = end.Line + 2; i < FilterQueue.Count; i++)
        {
            queue.Add(FilterQueue[i]);
        }

        FilterQueue.Clear();

        FilterQueue.AddRange(queue);

        UpdateFilterQueue(FilterQueue);
    }

    unsafe void OnApplyFiltersButtonClicked(object sender, EventArgs args)
    {
        ApplyFiltersButton.Sensitive = false;

        if (FilterQueue.Count > 0 && InputPixbuf != null)
        {
            var temp = Common.InitializePixbuf(InputPixbuf.Width, InputPixbuf.Height);

            InputPixbuf.Composite(temp, 0, 0, InputPixbuf.Width, InputPixbuf.Height, 0, 0, 1, 1, InterpType.Nearest, 255);

            foreach (var filter in FilterQueue)
            {
                var output = filter.Apply(filter.Library, temp, filter.Parameter, filter.Threshold);

                if (output != null && temp != null)
                {
                    Common.Free(temp);

                    temp = Common.InitializePixbuf(output.Width, output.Height);

                    output.CopyArea(0, 0, output.Width, output.Height, temp, 0, 0);

                    Console.WriteLine("Applied {0}", filter.Name);
                }

                Common.Free(output);
            }

            if (temp != null && OutputPixbuf != null)
            {
                Common.Free(OutputPixbuf);

                OutputPixbuf = Common.InitializePixbuf(temp.Width, temp.Height);

                temp.Composite(OutputPixbuf, 0, 0, temp.Width, temp.Height, 0, 0, 1, 1, InterpType.Nearest, 255);

                ResetOutputScrollBars();

                RenderOutput();

                LabelOutput.LabelProp = String.Format(ci, "<b>Output ({0}x{1})</b>", OutputPixbuf.Width, OutputPixbuf.Height);
            }

            Common.Free(temp);
        }

        ApplyFiltersButton.Sensitive = true;
    }
}
