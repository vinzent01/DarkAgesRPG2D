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
    HorizontalRight,
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

public enum PositionOffset {
    None,
    Right,
    Left,
    top,
    Bottom,
    CenterHorizontal,
    CenterVertical,
    Center,
    CenterBottom,
    CenterRight,
    CenterLeft
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
    public PositionOffset PositionOffset;
    public float RoundBorders;

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
            var marginOffset = new Vector2i(0, 0);

            if (parent != null)
                marginOffset = new Vector2i(parent.margin.left, parent.margin.top);
            
            if (parent != null && this.PositionOffset == PositionOffset.None){
                var parentOffset = new Vector2i(parentOffsetX, parentOffsetY);

                return DragOffset + parent.GlobalPosition + position +parentOffset +marginOffset;
            }
            else if (parent != null && this.PositionOffset != PositionOffset.None) {
                var positionOffset = new Vector2i(0,0);
                var parentOffset = new Vector2i(0,0);

                if (this.PositionOffset == PositionOffset.CenterHorizontal ){
                    positionOffset = new Vector2i(
                        (parent.TotalSize.X / 2) - TotalSize.X / 2, 
                        0
                    );

                    parentOffset = new Vector2i(0, parentOffsetY);
                }
                else if (PositionOffset == PositionOffset.CenterBottom){
                    positionOffset = new Vector2i((parent.TotalSize.X / 2) - TotalSize.X / 2, parent.TotalSize.Y - TotalSize.Y);
                    parentOffset = new Vector2i(0, 0);
                }
                else if (PositionOffset == PositionOffset.CenterLeft){
                    positionOffset = new Vector2i(0, parent.TotalSize.Y /2 - TotalSize.Y /2);
                    parentOffset = new Vector2i(0, 0);
                }
                else if (PositionOffset == PositionOffset.CenterRight){
                    positionOffset = new Vector2i(parent.TotalSize.X - TotalSize.X, parent.TotalSize.Y /2 - TotalSize.Y /2);
                    parentOffset = new Vector2i(0, 0);
                }
                else if (PositionOffset == PositionOffset.Right){
                    positionOffset = new Vector2i(parent.TotalSize.X - TotalSize.X - (parent.margin.right + parent.margin.left), 0);
                    parentOffset = new Vector2i(0, parentOffsetY);
                }

                return (
                    parent.GlobalPosition +
                    positionOffset +
                    parentOffset +
                    marginOffset
                );
            }

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
        PositionOffset = PositionOffset.None;
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
        GrowWidget();
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

    private void GrowWidget(){
        if (Grow.growX == true && parent != null){
            this.TotalSize = new Vector2i(parent.TotalSize.X, TotalSize.Y);
        }
        if (Grow.growY == true && parent != null){
            this.TotalSize = new Vector2i(TotalSize.X, parent.TotalSize.Y);
        }
    }

    private void HandleInteractions(float delta)
    {
        var root = GetRoot();

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

        if (IsClick())
        {
            OnClick();
        }
        else if (IsHolding(delta))
        {
            if (CanDrag && root != null)
            {
                StartDrag();
            }
        }


        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            ResetHolding();
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
        
        if (droppedOn == this.parent){
            CancelDrag();
            return;
        }

        if (droppedOn != null) {
            // Tenta realizar o drop no container identificado
            if (!droppedOn.OnRecieveDropWidget(this)) {
                CancelDrag();
                return;
            }

            parent?.OnSendDropWidget(this);
            CloseWidget();

            return;
        }

        // Caso o widget não esteja sobre nenhum container
        if (parent != null) {
            if (!parent.OnDropOnGround(this)) {
                CancelDrag();
                return;
            }

            // Drop bem-sucedido no chão
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

    public void Click(){
        OnClick();
    }

    public void DrawHud(){
        Raylib.DrawRectangleRounded(
            new Rectangle(
                GlobalPosition.X, 
                GlobalPosition.Y,
                TotalSize.X,
                TotalSize.Y
            ),
            this.RoundBorders,
            1,
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
        if (
            root != null && 
            root.GetWidgetOnMouse() == this  && 
            Raylib.IsMouseButtonReleased(MouseButton.Left) &&
            root.currentDraggingWidget == null
        ){
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


    public void SetWidgetSize(string text, int font){
        this.fontSize = font;
        this.Size.Y = this.fontSize;
        this.Size.X = Raylib.MeasureText(text, this.fontSize);
    }

    public void FitContent() {
        Vector2i FitSize = new Vector2i(0, 0);

        if (flowDirection == FlowDirections.Vertical && PositionOffset == PositionOffset.None) {
            int maxX = 0;

            foreach (var child in Childrens) {
                FitSize.Y += child.TotalSize.Y + gap;
                maxX = Math.Max(maxX, child.TotalSize.X);
            }

            // Remove o último gap apenas se houver filhos
            if (Childrens.Count > 0)
                FitSize.Y = Math.Max(0, FitSize.Y - gap);

            FitSize.X = maxX;
        } else if (PositionOffset == PositionOffset.None) {
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


    public void AddWidgetSize(Widget widget){
        if (DoFlow && PositionOffset == PositionOffset.None){
            if (flowDirection == FlowDirections.Vertical){
                widget.parentOffsetY = totalOffsetY;
                totalOffsetY += (int)widget.TotalSize.Y + gap;
            }
            else if (flowDirection == FlowDirections.Horizontal) {
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX += (int)widget.TotalSize.X + gap;
            }
            else {
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX -= (int)widget.TotalSize.X + gap;
            }

            FitContent();
        }
    }

    public void RemoveWidgetSize(Widget widget){
        if (DoFlow && PositionOffset == PositionOffset.None){
            if (flowDirection == FlowDirections.Vertical){
                widget.parentOffsetY = totalOffsetY;
                totalOffsetY -= (int)widget.TotalSize.Y + gap;
            }
            else if (flowDirection == FlowDirections.Horizontal){
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX -= (int)widget.TotalSize.X + gap;
            }
            else {
                widget.parentOffsetX = totalOffsetX;
                totalOffsetX += (int)widget.TotalSize.X + gap;
            }

            FitContent();
        }
    }

    public void UpdatePositions(){
        totalOffsetX = 0;
        totalOffsetY = 0;

        foreach (var child in Childrens){
            AddWidgetSize(child);
        }
    }

    public void AddChild(Widget widget){
        if (HasChild(widget)){
            return;
        }


        Childrens.Add(widget);
        widget.parent = this;
        AddWidgetSize(widget);

    }

    public void RemoveChild(Widget widget){
        if (!HasChild(widget)){
            return;
        }

        Childrens.Remove(widget);
        widget.parent = null;
        RemoveWidgetSize(widget);
    }

    public void RemoveAllChild(){
        foreach (var child in Childrens.ToList()){
            RemoveChild(child);
        }
    }


    public bool HasChild(Widget widget){
        foreach (var child in Childrens){
            if (child == widget)
                return true;
        }
        return false;
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


        if (Grow.growX == true && parent != null){
            // calculate remaining size of parent

            Vector2i size = parent.Size;
            Vector2i occupedSize = new Vector2i(0,0);

            foreach (var child in parent.Childrens){
                occupedSize.X += child.Size.X + parent.gap;
            }



            return size - occupedSize;;
            
        }
        return new Vector2i(0,0);
    }

}