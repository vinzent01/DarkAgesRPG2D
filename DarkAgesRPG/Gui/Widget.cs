using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using CSScripting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public enum FlowDirections{
    Horizontal,
    Vertical
}

public struct Margin {
    public int top;
    public int bottom;
    public int left;
    public int right;
}

public struct Grow {
    public bool growX;
    public bool growY;
}

public class Widget {

    // Styles
    public int fontSize;
    private Vector2i Size;
    public bool CanDrag = false;
    
    public int totalOffsetY;
    public int parentOffsetY;

    public int totalOffsetX;
    public int parentOffsetX;
    public int gap;

    public Vector2i DragOffset;

    public Color color;
    public Color backgroundColor;

    public FlowDirections flowDirection;
    public bool DoFlow;

    public Margin margin;
    public Grow Grow { get; set; }

    // Properties
    public string? id;
    public Widget? parent;
    public Widget? previousParent;

    private Vector2i position;
    public List<Widget> Childrens;
    protected bool toClose;
    public bool DoMouseCollision;
    public bool IsContainer;
    public float holdTime;

    public Vector2i TotalSize {
        get {
            // Retorna o tamanho ajustado com margens e crescimento
            return new Vector2i(
                Size.X + margin.right + margin.left ,
                Size.Y + margin.top + margin.bottom 
            );
        }
        set {
            int newX = Math.Max(0, value.X - margin.right - margin.left);
            int newY = Math.Max(0, value.Y - margin.top - margin.bottom);
            Size = new Vector2i(newX, newY);
        }
    }

    public Vector2i GlobalPosition {
        get {
            if (parent != null)
                return 
                    (   
                        DragOffset + 
                        parent.GlobalPosition + 
                        new Vector2i(position.X + parentOffsetX + parent.margin.right, position.Y +parentOffsetY + parent.margin.left)
                    );

            return DragOffset + position;
        }
        set {
            position = value;
        }
    }

    public Vector2i LocalPosition {
        get {
            if (parent != null)
                return GlobalPosition - parent.GlobalPosition;
            return GlobalPosition;
        }
        set {
            if (parent != null)
                GlobalPosition = value + parent.GlobalPosition;
            else 
                GlobalPosition = value;
        }
    }

    public Widget(){
        Childrens = new();
        flowDirection =FlowDirections.Vertical;
        gap = 10;
        DoFlow = true;
        DoMouseCollision = true;
        DragOffset = new Vector2i(0,0);
        IsContainer = false;
    }
    
    public void Update(float delta)
    {
        if (toClose)
        {
            Close();
            return;
        }

        UpdateChildren(delta);
        HandleInteractions(delta);
        OnUpdate(delta);
    }

    private void Close()
    {
        parent?.RemoveChild(this);
        OnClose();
        toClose = false;
    }

    private void UpdateChildren(float delta)
    {
        foreach (var child in Childrens.ToList())
        {
            child.Update(delta);
        }
    }

    private void HandleInteractions(float delta)
    {
        var root = GetRoot();

        if (IsHolding(delta))
        {
            if (CanDrag && root != null)
            {
                StartDrag();
            }
        }
        else if (IsClick())
        {
            OnClick();
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            ResetHolding();
        }

        if (CanDrag && root != null && root.currentDraggingWidget == this)
        {
            if (IsDragging())
            {
                Drag();
            }
            else if (IsDrop())
            {
                Drop();
            }
        }
    }
    protected void StartDrag(){
        var root = GetRoot();

        if (root != null){
            root.currentDraggingWidget = this;
            previousParent = parent;
        }
    }

    protected void Drag(){
        DragOffset += new Vector2i(Raylib.GetMouseDelta());
    }

    protected void Drop() {
        var droppedOn = GetDropContainer();
        var root = GetRoot();

        // Finaliza a referência ao widget em arrasto
        if (root != null)
            root.currentDraggingWidget = null;

        if (droppedOn != null) {
            // Tenta realizar o drop no container identificado
            if (!droppedOn.OnRecieveDropWidget(this)) {
                CancelDrag();
                return;
            }

            parent?.OnSendDropWidget(this);
            CloseWidget();

            Console.WriteLine("Dropped on " + droppedOn.GetType());
            return;
        }

        // Caso o widget não esteja sobre nenhum container
        if (parent != null) {
            if (!parent.OnDropOnGround(this)) {
                CancelDrag();
                return;
            }

            // Drop bem-sucedido no chão
            Console.WriteLine("Dropped on Ground");
            CloseWidget();
            return;
        }

        // Nenhum local válido para o drop
        CancelDrag();
    }

    protected void CancelDrag(){
        if (parent != GetRoot()){
            DragOffset = new Vector2i(0,0);
        }

        if (previousParent != null){
            previousParent.AddChild(this);
        }
        else {
            Console.WriteLine("CancelDrag: No previous parent found, widget is now orphaned!");
            Debug.Assert(false);
        }
        
        previousParent = null; // Resetar previousParent
        
    }

    // EVENTS //

    protected virtual bool OnRecieveDropWidget(Widget widget){
        return false;
    }

    protected virtual void OnSendDropWidget(Widget widget){
    }

    protected virtual bool OnDropOnGround(Widget widget){
        return false;
    }

    protected virtual void OnUpdate(float delta){
    }

    protected virtual void OnClose(){

    }
    
    protected virtual void OnClick(){

    }

    public void DrawHud(){
        Raylib.DrawRectangle(
            GlobalPosition.X, 
            GlobalPosition.Y,
            TotalSize.X,
            TotalSize.Y, 
            backgroundColor
        );

        foreach (var child in Childrens){
            child.DrawHud();
        }

        OnDrawHud();
    }

    protected virtual void OnDrawHud(){

    }

    public bool Contains(Vector2i Position){
        if (Position.X >= this.GlobalPosition.X && Position.X <= this.GlobalPosition.X + this.TotalSize.X){
            if (Position.Y >= this.GlobalPosition.Y && Position.Y <= this.GlobalPosition.Y + this.TotalSize.Y){
                return true;
            }   
        }
        return false;
    }


    public bool IsDragging(){

        if (
            Raylib.IsMouseButtonDown(MouseButton.Left)) 
        {
            return true;
        }
        return false;
    }

    public bool IsClick(){
        var root = GetRoot();
        if (root != null && root.GetWidgetOnMouse() == this  && Raylib.IsMouseButtonReleased(MouseButton.Left)){
            return true;
        }
        return false;
    }

    public bool IsHolding(float delta){
        var root = GetRoot();
        if (
            root != null && root.GetWidgetOnMouse() == this  && 
            Raylib.IsMouseButtonDown(MouseButton.Left) && 
            root.currentDraggingWidget == null
            ){
            holdTime += delta;
        }

        if (holdTime > 0.1){
            return true;
        }


        return false;
    }

    public void ResetHolding(){
        holdTime = 0;
    }


    protected bool IsDrop(){
        var root = GetRoot();

        if ( Raylib.IsMouseButtonReleased(MouseButton.Left)){
            return true;
        }
        return false;
    }

    protected Widget? GetDropContainer(){
        var root = GetRoot();

        if (root != null ){
            var container = root.GetContainerWidgetOnMouse();
            return container;
        }

        return null;
    }


    public void SetFontSizeSize(string text, int font){
        this.fontSize = font;
        this.Size.Y = this.fontSize;
        this.Size.X = Raylib.MeasureText(text, this.fontSize);
    }

    public void FitContent() {
        Vector2i FitSize = new Vector2i(0, 0);

        if (flowDirection == FlowDirections.Vertical) {
            int maxX = 0;

            foreach (var child in Childrens) {
                FitSize.Y += child.TotalSize.Y + gap;
                maxX = Math.Max(maxX, child.TotalSize.X);
            }

            // Remove o último gap apenas se houver filhos
            if (Childrens.Count > 0)
                FitSize.Y = Math.Max(0, FitSize.Y - gap);

            FitSize.X = maxX;
        } else {
            int maxY = 0;

            foreach (var child in Childrens) {
                FitSize.X += child.TotalSize.X + gap;
                maxY = Math.Max(maxY, child.TotalSize.Y);
            }

            // Remove o último gap apenas se houver filhos
            if (Childrens.Count > 0)
                FitSize.X = Math.Max(0, FitSize.X - gap);

            FitSize.Y = maxY;
        }

        // Ajuste final para incluir margens
        FitSize.X += margin.left + margin.right;
        FitSize.Y += margin.top + margin.bottom;

        // Ajusta o tamanho do elemento pai diretamente
        Size = new Vector2i(
            Math.Max(Size.X, FitSize.X - margin.left - margin.right),
            Math.Max(Size.Y, FitSize.Y - margin.top - margin.bottom)
        );
    }

    public void AddChild(Widget widget){
        if (HasChild(widget)){
            return;
        }


        Childrens.Add(widget);
        widget.parent = this;

        if (DoFlow){
            if (flowDirection == FlowDirections.Vertical){
                widget.parentOffsetY = totalOffsetY;
                totalOffsetY += (int)widget.TotalSize.Y + gap;
            }
            else {
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX += (int)widget.TotalSize.X + gap;
            }

            FitContent();
        }
    }

    public bool HasChild(Widget widget){
        foreach (var child in Childrens){
            if (child == widget)
                return true;
        }
        return false;
    }

    public void RemoveChild(Widget widget){
        if (!HasChild(widget)){
            return;
        }

        Childrens.Remove(widget);
        widget.parent = null;

        if (DoFlow){
            if (flowDirection == FlowDirections.Vertical){
                widget.parentOffsetY = totalOffsetY;
                totalOffsetY -= (int)widget.TotalSize.Y + gap;
            }
            else {
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX -= (int)widget.TotalSize.X + gap;
            }

            FitContent();
        }
    }

    public void RemoveAllChild(){
        foreach (var child in Childrens.ToList()){
            RemoveChild(child);
        }
    }

    public RootWidget? GetRoot()
    {
        Widget current = this;
        while (current.parent != null)
        {
            current = current.parent;
        }


        if (current is RootWidget root)
        {
            return root;
        }

        return null;
    }

    public Widget? GetById(string id){
        foreach (var widget in Childrens){
            if (widget.id == id)
                return widget;
        }

        return null;
    }

    public bool RemoveById(string id){
        var toremove = GetById(id);

        if (toremove != null){
            Childrens.Remove(toremove);
            return true;
        }

        return false;
    }

    public bool Hastype<T>(){

        foreach (var widget in Childrens){
            if (widget is T)
                return true;
        }

        return false;
    }

    public bool HasId(string id){
        foreach (var widget in Childrens){
            if (widget.id == id)
                return true;
        }
        return false;
    }

    public bool RemoveType<T>(){

        foreach (var widget in Childrens.ToList()){
            if (widget is T){
                Childrens.Remove(widget);
                return true;
            }
        }

        return false;

    }

    public void CloseWidget(){
        toClose = true;
    }

    public void CenterScreen(){
        GlobalPosition = new Vector2i(
            Raylib.GetScreenWidth() / 2 - TotalSize.X / 2, 
            Raylib.GetScreenHeight() / 2 - TotalSize.Y / 2
        );
    }

    public Vector2i GrowSize(){

        Console.WriteLine(Grow.growX + " " + GetType());

        if (Grow.growX == true && parent != null){
            // calculate remaining size of parent

            Vector2i size = parent.Size;
            Vector2i occupedSize = new Vector2i(0,0);

            foreach (var child in parent.Childrens){
                occupedSize.X += child.Size.X + parent.gap;
            }

            Console.WriteLine(occupedSize);


            return size - occupedSize;;
            
        }
        return new Vector2i(0,0);
    }

}