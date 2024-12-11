using System.Collections;
using System.Reflection;
using Raylib_cs;

namespace DarkAgesRPG.GUI;

public enum paddingDirecton {
    vertical,
    horizontal
}

public enum AnchorX {
    none,
    left,
    right,
    center
}

public enum AnchorY {
    none,
    top,
    bottom,
    center
}

public class Styles : IEnumerable{
    private Dictionary<string, dynamic> dict;

    public IEnumerator<KeyValuePair<string, dynamic>> GetEnumerator(){
        return dict.GetEnumerator();
    }

    public Styles(){
        dict = new();
    }

    public Styles(List<KeyValuePair<string, dynamic>> default_){
        dict = new(default_);
    }

    public static Styles Default(){
        Styles defaultStyles = new();

        defaultStyles.Set("PaddingDirection",  paddingDirecton.vertical);
        defaultStyles.Set("PaddingIncrease", 0);
        defaultStyles.Set("Margin", new Vector2i(0,0));

        defaultStyles.Set("BackgroundColor", new Color(0,0,0,0));
        defaultStyles.Set("Color", Color.Blue);
        defaultStyles.Set("OnHoverColor",Color.DarkBlue);

        defaultStyles.Set("FontSize", 24);
        defaultStyles.Set("AnchorX", AnchorX.none);
        defaultStyles.Set("AnchorY", AnchorY.none);
        defaultStyles.Set("RoundNess", 0.0f);
        defaultStyles.Set("Grow", 0);

        return defaultStyles;
    }

    public dynamic? Get(string key){
        if (dict.ContainsKey(key)){
            return dict[key];
        }
        return null;
    }

    public T? Get<T>(string key){
        if (dict.ContainsKey(key) && key is T){
            return dict[key];
        }

        return default(T);
    }

    public void Set(string key, dynamic value){
        dict[key] = value;
    }

    public void Override(Styles newStyle){
        foreach (var keyValue in newStyle){
            Set(keyValue.Key, keyValue.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}