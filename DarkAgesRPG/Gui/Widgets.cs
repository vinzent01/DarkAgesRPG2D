using DarkAgesRPG.Gui;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class Widgets : List<Widget> {


    public Widget? GetById(string id){
        foreach (var widget in this){
            if (widget.id == id)
                return widget;
        }

        return null;
    }

    public bool RemoveById(string id){
        var toremove = GetById(id);

        if (toremove != null){
            this.Remove(toremove);
            return true;
        }

        return false;
    }

    public bool Hastype<T>(){

        foreach (var widget in this){
            if (widget is T)
                return true;
        }

        return false;
    }

    public bool RemoveType<T>(){

        foreach (var widget in this.ToList()){
            if (widget is T){
                Remove(widget);
                return true;
            }
        }

        return false;

    }


    public bool IsMouseOnWidget(){
        foreach (var widget in this){
            return widget.MouseInWidget();
        }
        return false;
    }
}