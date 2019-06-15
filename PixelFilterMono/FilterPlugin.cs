using Gdk;
using System;
using System.Collections.Generic;

public class FilterApplication
{
    public string Name;
    public Func<string, Pixbuf, int, bool, Pixbuf> Apply;
    public int Parameter;
    public bool Threshold;
    public string Library;

    public FilterApplication(string name, string library, Func<string, Pixbuf, int, bool, Pixbuf> apply, int parameter, bool threshold = false)
    {
        Name = String.Format("{0}: ({1})", name, parameter.ToString());
        Library = library;
        Apply = apply;
        Parameter = parameter;
        Threshold = threshold;
    }
}

public class FilterPlugin
{
    public string Name;
    public string Description;
    public readonly string Library;
    public string ParameterName;
    public List<int> FilterParameters = new List<int>();
    public Func<string, Pixbuf, int, bool, Pixbuf> Apply;

    public FilterPlugin(string name, string description, string library, string parameterName, int[] filterParameters, Func<string, Pixbuf, int, bool, Pixbuf> apply)
    {
        Name = name;
        Description = description;
        Library = library;
        ParameterName = parameterName;
        FilterParameters.AddRange(filterParameters);
        Apply = apply;
    }
}
