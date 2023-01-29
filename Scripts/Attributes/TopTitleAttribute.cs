using System;

public class TopTitleAttribute : Attribute
{
    public bool IsNameCentered = true;
    public bool LastIsReturn = false;
    public bool BoldName;
    public bool ShowGenericName;
    public bool ShowNameOnPrefix;
    public bool ShowTypeOnSuffix;
    public bool HideNameOnMid;
    public string PerGenericArgString;
    public string NamePrefix;
    public string NameSuffix;
    public bool SetTransform;
    public bool SetName;
    public bool SetParentObject; //Requires public Object parentObject{get;set;}
    public bool HasFoldout = false;
    public string FoldoutPropertyName = "IsVisible";
}